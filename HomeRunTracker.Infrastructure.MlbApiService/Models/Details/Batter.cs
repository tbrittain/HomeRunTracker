﻿namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public record Batter
{
    public int Id { get; set; }
    
    public string FullName { get; set; } = string.Empty;
    
    public string Link { get; set; } = string.Empty;
}