﻿using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.SharedKernel.Extensions;

public static class EPlayResultExtensions
{
    public static bool IsHit(this EPlayResult playResult)
    {
        return playResult switch
        {
            EPlayResult.Single => true,
            EPlayResult.Double => true,
            EPlayResult.Triple => true,
            EPlayResult.HomeRun => true,
            _ => false
        };
    }
}