﻿using HomeRunTracker.Common;
using HomeRunTracker.Common.Models.Details;
using HomeRunTracker.Common.Models.Summary;
using Newtonsoft.Json;

namespace HomeRunTracker.Backend.Grains;

public class GameGrain : Grain, IGameGrain
{
    private int _gameId;
    private MlbGameDetails _gameDetails = new();
    private string _gameContentLink = string.Empty;
    private HashSet<MlbPlay> _homeRuns = new();
    private IDisposable _timer = null!;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GameGrain> _logger;

    public GameGrain(HttpClient httpClient, ILogger<GameGrain> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task InitializeAsync(MlbGameSummary game)
    {
        _gameId = game.Id;
        _gameContentLink = game.Content.Link;

        _logger.LogInformation("Initializing game grain {GameId}", game.Id.ToString());
        _timer = RegisterTimer(async _ =>
        {
            _logger.LogDebug("Polling game {GameId}", game.Id.ToString());

            var gameDetails = await FetchGameDataAsync();

            if (gameDetails.GameData.Status.Status is EMlbGameStatus.PreGame)
            {
                _logger.LogInformation("Game {GameId} is in pre-game", _gameId.ToString());
                await Task.Delay(TimeSpan.FromMinutes(15));
                return;
            }

            if (gameDetails.GameData.Status.Status is EMlbGameStatus.Warmup)
            {
                _logger.LogInformation("Game {GameId} is warming up", _gameId.ToString());
                await Task.Delay(TimeSpan.FromMinutes(5));
                return;
            }

            var homeRuns = gameDetails.LiveData.Plays.AllPlays
                .Where(p => p.Result.Result is EPlayResult.HomeRun)
                .ToList();

            foreach (var homeRun in homeRuns.Where(homeRun => !_homeRuns.Contains(homeRun)))
            {
                _logger.LogInformation("Game {GameId} has a new home run", _gameId.ToString());
                _homeRuns.Add(homeRun);
            }

            if (gameDetails.GameData.Status.Status is not EMlbGameStatus.InProgress)
            {
                _logger.LogInformation("Game {GameId} is no longer in progress", _gameId.ToString());
                await StopAsync();
                return;
            }

            _gameDetails = gameDetails;
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

        await base.OnActivateAsync(default);
    }

    public async Task<MlbGameDetails> GetGameAsync()
    {
        return await Task.FromResult(_gameDetails);
    }

    public async Task StopAsync()
    {
        _logger.LogInformation("Stopping game grain {GameId}", _gameId.ToString());
        DeactivateOnIdle();
        await Task.CompletedTask;
    }

    private async Task<MlbGameDetails> FetchGameDataAsync()
    {
        _logger.LogDebug("Fetching game data for game {GameId}", _gameId.ToString());
        var url = $"https://statsapi.mlb.com/api/v1.1/game/{_gameId}/feed/live";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch game data for game {GameId}", _gameId.ToString());
            throw new Exception($"Failed to fetch game data for game {_gameId}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var updatedGame = JsonConvert.DeserializeObject<MlbGameDetails>(content);

        if (updatedGame is null)
        {
            _logger.LogError("Failed to deserialize game data for game {GameId}", _gameId.ToString());
            throw new Exception($"Failed to deserialize game data for game {_gameId}");
        }

        return updatedGame;
    }
}