﻿using HomeRunTracker.Backend.Grains;
using HomeRunTracker.Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeRunTracker.Backend.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ScoringPlayController : ControllerBase
{
    private readonly IClusterClient _clusterClient;

    public ScoringPlayController(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<ScoringPlayRecord>>> GetScoringPlays([FromQuery] DateTime? date)
    {
        var grain = _clusterClient.GetGrain<IGameListGrain>(0);
        var scoringPlays = await grain.GetScoringPlays(date ?? DateTime.Now);
        return Ok(scoringPlays);
    }
}