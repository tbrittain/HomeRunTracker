﻿using System.Diagnostics;
using HomeRunTracker.Backend.Hubs;
using HomeRunTracker.Backend.Services;
using HomeRunTracker.Backend.Services.HttpService;
using HomeRunTracker.Common.Enums;
using HomeRunTracker.Common.Models.Internal;
using HomeRunTracker.Common.Models.Notifications;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace HomeRunTracker.Backend.Grains;

// ReSharper disable once UnusedType.Global
public class GameListGrain : Grain, IGameListGrain
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<GameListGrain> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<HomeRunHub> _hubContext;
    private readonly IHttpService _httpService;

    public GameListGrain(IClusterClient clusterClient, ILogger<GameListGrain> logger, IServiceProvider serviceProvider,
        IHubContext<HomeRunHub> hubContext, IHttpService httpService)
    {
        _clusterClient = clusterClient;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
        _httpService = httpService;
    }

    public async Task<List<HomeRunRecord>> GetHomeRuns(DateTime dateTime)
    {
        _logger.LogInformation("Getting all home runs");
        var pollingService = _serviceProvider.GetService<MlbCurrentDayGamePollingService>();
        if (pollingService is null)
            throw new InvalidOperationException("MlbApiPollingService not found");

        var fetchGamesResponse = await _httpService.FetchGames(dateTime);

        if (fetchGamesResponse.TryPickT2(out var error, out var rest))
        {
            _logger.LogError("Failed to fetch games from MLB API: {Error}", error.Value);
        }

        if (rest.TryPickT1(out var failureStatusCode, out var gameSchedule))
        {
            _logger.LogError("Failed to fetch games from MLB API; status code: {StatusCode}",
                failureStatusCode.ToString());
        }

        if (gameSchedule.TotalGames == 0)
            return new List<HomeRunRecord>();

        var gameIds = gameSchedule.Dates
            .SelectMany(x => x.Games
                .Select(y => y.Id))
            .ToList();

        var gameGrains = gameIds
            .Select(id => _clusterClient.GetGrain<IGameGrain>(id))
            .ToList();

        var allHomeRuns = new List<HomeRunRecord>();
        foreach (var grain in gameGrains)
        {
            var game = await grain.GetGame();
            var gameContent = await grain.GetGameContent();

            _logger.LogInformation("Getting home runs for game {GameId}", game.Id.ToString());
            var homeRuns = game.LiveData.Plays.AllPlays
                .Where(p => p.Result.Result is EPlayResult.HomeRun)
                .Select(play =>
                {
                    var homeRunEvent = play.Events.Single(e => e.HitData is not null);
                    Debug.Assert(homeRunEvent != null, nameof(homeRunEvent) + " != null");

                    var highlightUrl = gameContent.HighlightsOverview.Highlights.Items
                        .FirstOrDefault(item => item.Guid is not null && item.Guid == homeRunEvent.PlayId)
                        ?.Playbacks.FirstOrDefault(p => p.PlaybackType is EPlaybackType.Mp4)
                        ?.Url;

                    return new HomeRunRecord
                    {
                        Hash = HomeRunRecord.GetHash(play.Result.Description, game.Id),
                        GameId = game.Id,
                        DateTime = play.DateTime,
                        BatterId = play.PlayerMatchup.Batter.Id,
                        BatterName = play.PlayerMatchup.Batter.FullName,
                        Description = play.Result.Description,
                        Rbi = play.Result.Rbi,
                        LaunchSpeed = homeRunEvent.HitData!.LaunchSpeed,
                        LaunchAngle = homeRunEvent.HitData!.LaunchAngle,
                        TotalDistance = homeRunEvent.HitData!.TotalDistance,
                        Inning = play.About.Inning,
                        IsTopInning = play.About.IsTopInning,
                        PitcherId = play.PlayerMatchup.Pitcher.Id,
                        PitcherName = play.PlayerMatchup.Pitcher.FullName,
                        HighlightUrl = highlightUrl
                    };
                })
                .ToList();

            allHomeRuns.AddRange(homeRuns);
        }

        _logger.LogInformation("Returning {HomeRunCount} home runs", allHomeRuns.Count.ToString());
        return allHomeRuns;
    }

    public async Task PublishHomeRun(HomeRunNotification notification)
    {
        _logger.LogInformation("Publishing home run {Hash} for game {GameId}", notification.HomeRun.Hash,
            notification.GameId.ToString());

        await _hubContext.Clients.All.SendAsync("ReceiveHomeRun", JsonConvert.SerializeObject(notification.HomeRun));

        _logger.LogInformation("Finished publishing home run {Hash} for game {GameId}", notification.HomeRun.Hash,
            notification.GameId.ToString());
    }

    public async Task PublishHomeRunUpdated(HomeRunUpdatedNotification notification)
    {
        _logger.LogInformation("Publishing home run modified {Hash} for game {GameId}", notification.HomeRunHash,
            notification.GameId.ToString());
        
        await _hubContext.Clients.All.SendAsync("UpdateHomeRun", JsonConvert.SerializeObject(notification));
        
        _logger.LogInformation("Finished publishing home run modified {Hash} for game {GameId}", notification.HomeRunHash,
            notification.GameId.ToString());
    }
}