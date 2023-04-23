﻿using HomeRunTracker.Backend.Models;
using HomeRunTracker.Backend.Models.Notifications;

namespace HomeRunTracker.Backend.Grains;

public interface IGameListGrain : IGrainWithIntegerKey
{
    Task<List<ScoringPlayRecord>> GetScoringPlays(DateTime dateTime);
    Task<List<GameScoreRecord>> GetGameScores(DateTime dateTime);

    Task PublishScoringPlay(ScoringPlayNotification notification);
    Task PublishScoringPlayUpdated(ScoringPlayUpdatedNotification notification);
    Task PublishGameScore(GameScoreNotification notification);
}