﻿namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public record Matchup
{
    [Id(0)]
    public Batter Batter { get; set; } = new Batter();

    [Id(1)]
    public Pitcher Pitcher { get; set; } = new Pitcher();
}