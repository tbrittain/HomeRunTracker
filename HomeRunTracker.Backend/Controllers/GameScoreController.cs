﻿using HomeRunTracker.Backend.Grains;
using HomeRunTracker.Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeRunTracker.Backend.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class GameScoreController : ControllerBase
{
    private readonly IClusterClient _clusterClient;

    public GameScoreController(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    [HttpGet]
    public async Task<ActionResult<List<GameScoreRecord>>> GetGameScores([FromQuery] DateTime? date)
    {
        var grain = _clusterClient.GetGrain<IGameListGrain>(0);
        var gameScores = await grain.GetGameScores(date ?? DateTime.Now);
        return Ok(gameScores);
    }
}