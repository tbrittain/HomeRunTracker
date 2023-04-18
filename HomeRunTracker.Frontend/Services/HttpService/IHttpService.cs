﻿using HomeRunTracker.Common.Models.Internal;

namespace HomeRunTracker.Frontend.Services.HttpService;

public interface IHttpService
{
    Task<List<ScoringPlayRecord>> GetHomeRunsAsync(DateTime? dateTime);
}