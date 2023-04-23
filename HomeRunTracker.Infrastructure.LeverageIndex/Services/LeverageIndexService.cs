﻿using System.Text;
using HomeRunTracker.Core.Interfaces;
using HomeRunTracker.Core.Models.Details;
using HomeRunTracker.SharedKernel.Enums;
using Microsoft.Data.Analysis;

namespace HomeRunTracker.Infrastructure.LeverageIndex.Services;

public class LeverageIndexService : ILeverageIndexService
{
    private readonly DataFrame _leverageIndexData;

    public LeverageIndexService()
    {
        // This is an adaptation of the leverage index data from Tangotiger's Baseball Leverage Index
        // http://www.insidethebook.com/li.shtml
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "leverage_indices.csv");
        _leverageIndexData = DataFrame.LoadCsv(path);
    }

    public float GetLeverageIndex(PlayDto play)
    {
        var inning = play.Inning;
        if (inning > 9) inning = 9;
        
        var isTopInning = play.IsTopInning;
        var outs = play.Outs;

        var homeTeamRunDiff = play.HomeScoreBefore -  play.AwayScoreBefore;

        if (homeTeamRunDiff is > 4 or < -4) return 0.0f;

        var isRunnerOnFirst = play.Runners.Any(r => r.Base is EBase.First);
        var isRunnerOnSecond = play.Runners.Any(r => r.Base is EBase.Second);
        var isRunnerOnThird = play.Runners.Any(r => r.Base is EBase.Third);

        var baseState = new StringBuilder();
        baseState.Append(isRunnerOnFirst ? "1" : "_");
        baseState.Append(isRunnerOnSecond ? "2" : "_");
        baseState.Append(isRunnerOnThird ? "3" : "_");
        var baseStateString = baseState.ToString();

        var row = _leverageIndexData.Rows
            .Where(row => int.Parse(row[0].ToString()!) == inning)
            .Where(row => (bool) row[1] == isTopInning)
            .Where(row => (string) row[2] == baseStateString)
            .SingleOrDefault(row => int.Parse(row[3].ToString()!) == outs);

        if (row is null) return 0.0f;

        // Index 8 is the location of the leverage index for a run differential of 0
        var column = row[8 + homeTeamRunDiff];
        var leverageIndex = float.Parse(column.ToString()!);

        return leverageIndex;
    }
}