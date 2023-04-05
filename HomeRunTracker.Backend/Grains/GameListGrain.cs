using System.Diagnostics;
using HomeRunTracker.Backend.Services;
using HomeRunTracker.Common.Models.Details;
using HomeRunTracker.Common.Models.Internal;

namespace HomeRunTracker.Backend.Grains;

public class GameListGrain : Grain, IGameListGrain
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<GameListGrain> _logger;
    private readonly IServiceProvider _serviceProvider;

    public GameListGrain(IClusterClient clusterClient, ILogger<GameListGrain> logger, IServiceProvider serviceProvider)
    {
        _clusterClient = clusterClient;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task<List<HomeRunRecord>> GetHomeRunsAsync()
    {
        _logger.LogInformation("Getting all home runs");
        var pollingService = _serviceProvider.GetService<MlbApiPollingService>();
        if (pollingService is null)
            throw new InvalidOperationException("MlbApiPollingService not found");
        
        var gameIds = pollingService.TrackedGameIds;
        
        var gameGrains = gameIds
            .Select(id => _clusterClient.GetGrain<IGameGrain>(id))
            .ToList();
        
        var allHomeRuns = new List<HomeRunRecord>();
        foreach (var grain in gameGrains)
        {
            var game = await grain.GetGameAsync();

            _logger.LogInformation("Getting home runs for game {GameId}", game.Id.ToString());
            var homeRuns = game.LiveData.Plays.AllPlays
                .Where(p => p.Result.Result is EPlayResult.HomeRun)
                .Select(play =>
                {
                    var homeRunEvent = play.Events.Single(e => e.HitData is not null).HitData;
                    Debug.Assert(homeRunEvent != null, nameof(homeRunEvent) + " != null");
                    
                    return new HomeRunRecord
                    {
                        Id = Guid.NewGuid(),
                        GameId = game.Id,
                        DateTime = play.DateTime,
                        BatterId = play.PlayerMatchup.Batter.Id,
                        BatterName = play.PlayerMatchup.Batter.FullName,
                        Description = play.Result.Description,
                        Rbi = play.Result.Rbi,
                        LaunchSpeed = homeRunEvent.LaunchSpeed,
                        LaunchAngle = homeRunEvent.LaunchAngle,
                        TotalDistance = homeRunEvent.TotalDistance,
                        Inning = play.About.Inning,
                        IsTopInning = play.About.IsTopInning,
                        PitcherId = play.PlayerMatchup.Pitcher.Id,
                        PitcherName = play.PlayerMatchup.Pitcher.FullName,
                    };
                })
                .ToList();
            
            allHomeRuns.AddRange(homeRuns);
        }

        _logger.LogInformation("Returning {HomeRunCount} home runs", allHomeRuns.Count.ToString());
        return allHomeRuns;
    }
}