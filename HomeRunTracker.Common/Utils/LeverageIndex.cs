using HomeRunTracker.Common.Enums;
using HomeRunTracker.Common.Models.Details;

namespace HomeRunTracker.Common.Utils;

public static class LeverageIndex
{
    public static float GetLeverageIndex(MlbPlay play)
    {
        var inning = play.About.Inning;
        var isTopInning = play.About.IsTopInning;
        var outs = play.Count.Outs;
        
        var (homeScoreStart, awayScoreStart) = play.GetScoreStart();
        var homeTeamRunDiff = homeScoreStart - awayScoreStart;
        
        var isRunnerOnFirst = play.Runners.Any(r => r.Movement.Base is EBase.First);
        var isRunnerOnSecond = play.Runners.Any(r => r.Movement.Base is EBase.Second);
        var isRunnerOnThird = play.Runners.Any(r => r.Movement.Base is EBase.Third);

        var leverageIndex = HomeRunTracker.LeverageIndex.LeverageIndex.GetLeverageIndex(inning, isTopInning, outs, isRunnerOnFirst,
            isRunnerOnSecond, isRunnerOnThird, homeTeamRunDiff);
        
        return leverageIndex;
    }
}