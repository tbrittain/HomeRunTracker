using HomeRunTracker.Backend.Grains;
using HomeRunTracker.Backend.Models;
using HomeRunTracker.Backend.Models.Details;
using HomeRunTracker.Backend.Models.Schedule;
using HomeRunTracker.Core.Interfaces;
using HomeRunTracker.SharedKernel.Enums;
using Mapster;

namespace HomeRunTracker.Backend.Services;

public class MlbCurrentDayGamePollingService : BackgroundService
{
    private readonly IMlbApiService _mlbApiService;
    private readonly IGrainFactory _grainFactory;
    private readonly ILogger<MlbCurrentDayGamePollingService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromHours(4);
    private readonly List<int> _trackedCurrentDayGameIds = new(15);

    public MlbCurrentDayGamePollingService(IGrainFactory grainFactory, ILogger<MlbCurrentDayGamePollingService> logger,
        IMlbApiService mlbApiService)
    {
        _grainFactory = grainFactory;
        _logger = logger;
        _mlbApiService = mlbApiService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Starting {nameof(MlbCurrentDayGamePollingService)}");
        while (!cancellationToken.IsCancellationRequested)
        {
            var fetchGamesResponse = await _mlbApiService.FetchGames(DateTime.Now);

            if (fetchGamesResponse.TryPickT2(out var error, out var rest))
            {
                _logger.LogError("Failed to fetch games from MLB API: {Error}", error.Value);
            }

            if (rest.TryPickT1(out var failureStatusCode, out var scheduleDto))
            {
                _logger.LogError("Failed to fetch games from MLB API; status code: {StatusCode}",
                    failureStatusCode.ToString());
            }

            var schedule = scheduleDto.Adapt<Schedule>();

            if (schedule.TotalGames == 0)
            {
                _logger.LogInformation("No games scheduled for today");
                await Task.Delay(_pollingInterval, cancellationToken);
                continue;
            }

            var games = schedule.Games
                .Where(g => !_trackedCurrentDayGameIds.Contains(g.Id))
                .Where(g => g.Status is not (EMlbGameStatus.Final or EMlbGameStatus.Unknown))
                .ToList();

            var trackedGameIds = await FanOutGameGrains(games);
            _trackedCurrentDayGameIds.AddRange(trackedGameIds);

            await Task.Delay(_pollingInterval, cancellationToken);
        }

        foreach (var trackedGameId in _trackedCurrentDayGameIds)
        {
            var gameGrain = _grainFactory.GetGrain<IGameGrain>(trackedGameId);
            await gameGrain.Stop();
        }

        _logger.LogInformation("Stopping MLB API polling service");
    }

    private async Task<List<int>> FanOutGameGrains(List<GameSummary> games)
    {
        games = games.Where(x => x.Id == 718462).ToList();
        _logger.LogInformation("Fanning out {Count} game grains", games.Count.ToString());
        List<Task<GameDetails>> initializedGameTasks = new();
        foreach (var game in games)
        {
            var grain = _grainFactory.GetGrain<IGameGrain>(game.Id);
            var task = grain.GetGame();
            initializedGameTasks.Add(task);
        }

        await Task.WhenAll(initializedGameTasks);

        return initializedGameTasks.Select(t => t.Result.Id).ToList();
    }

    public void UntrackGame(int gameId)
    {
        _trackedCurrentDayGameIds.Remove(gameId);
    }
}