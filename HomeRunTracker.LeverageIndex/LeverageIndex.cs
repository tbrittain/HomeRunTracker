using System.Collections.Immutable;
using CommunityToolkit.Diagnostics;

namespace HomeRunTracker.LeverageIndex;

public static class LeverageIndex
{
    private static readonly ImmutableDictionary<(byte inning, bool isTopOfInning, sbyte outs, int runnerPositions, sbyte
            homeTeamRunDifference), float>
        Indices = ImmutableDictionary
            .Create<(byte inning, bool isTopOfInning, sbyte OutAttribute, int runnerPositions, sbyte
                homeTeamRunDifference), float>()
            // Top of 1st
            // 0 outs
            .Add((1, true, 0, 0x000, -4), 0.4f)
            .Add((1, true, 0, 0x000, -3), 0.6f)
            .Add((1, true, 0, 0x000, -2), 0.7f)
            .Add((1, true, 0, 0x000, -1), 0.8f)
            .Add((1, true, 0, 0x000, 0), 0.9f)
            .Add((1, true, 0, 0x100, -4), 0.7f)
            .Add((1, true, 0, 0x100, -3), 0.9f)
            .Add((1, true, 0, 0x100, -2), 1.1f)
            .Add((1, true, 0, 0x100, -1), 1.3f)
            .Add((1, true, 0, 0x100, 0), 1.4f)
            .Add((1, true, 0, 0x010, -4), 0.6f)
            .Add((1, true, 0, 0x010, -3), 0.7f)
            .Add((1, true, 0, 0x010, -2), 0.9f)
            .Add((1, true, 0, 0x010, -1), 1.0f)
            .Add((1, true, 0, 0x010, 0), 1.2f)
            .Add((1, true, 0, 0x001, -4), 0.5f)
            .Add((1, true, 0, 0x001, -3), 0.6f)
            .Add((1, true, 0, 0x001, -2), 0.8f)
            .Add((1, true, 0, 0x001, -1), 0.9f)
            .Add((1, true, 0, 0x001, 0), 1.0f)
            .Add((1, true, 0, 0x110, -4), 0.8f)
            .Add((1, true, 0, 0x110, -3), 1.1f)
            .Add((1, true, 0, 0x110, -2), 1.3f)
            .Add((1, true, 0, 0x110, -1), 1.6f)
            .Add((1, true, 0, 0x110, 0), 1.8f)
            .Add((1, true, 0, 0x101, -4), 0.6f)
            .Add((1, true, 0, 0x101, -3), 0.8f)
            .Add((1, true, 0, 0x101, -2), 1.1f)
            .Add((1, true, 0, 0x101, -1), 1.3f)
            .Add((1, true, 0, 0x101, 0), 1.5f)
            .Add((1, true, 0, 0x011, -4), 0.6f)
            .Add((1, true, 0, 0x011, -3), 0.8f)
            .Add((1, true, 0, 0x011, -2), 1.0f)
            .Add((1, true, 0, 0x011, -1), 1.2f)
            .Add((1, true, 0, 0x011, 0), 1.3f)
            .Add((1, true, 0, 0x111, -4), 0.8f)
            .Add((1, true, 0, 0x111, -3), 1.1f)
            .Add((1, true, 0, 0x111, -2), 1.4f)
            .Add((1, true, 0, 0x111, -1), 1.7f)
            .Add((1, true, 0, 0x111, 0), 2.0f)

            // 1 out
            .Add((1, false, 1, 0x000, -4), 0.3f)
            .Add((1, false, 1, 0x000, -3), 0.4f)
            .Add((1, false, 1, 0x000, -2), 0.5f)
            .Add((1, false, 1, 0x000, -1), 0.6f)
            .Add((1, false, 1, 0x000, 0), 0.6f)
            .Add((1, false, 1, 0x100, -4), 0.6f)
            .Add((1, false, 1, 0x100, -3), 0.7f)
            .Add((1, false, 1, 0x100, -2), 0.9f)
            .Add((1, false, 1, 0x100, -1), 1.0f)
            .Add((1, false, 1, 0x100, 0), 1.1f)
            .Add((1, false, 1, 0x010, -4), 0.6f)
            .Add((1, false, 1, 0x010, -3), 0.8f)
            .Add((1, false, 1, 0x010, -2), 0.9f)
            .Add((1, false, 1, 0x010, -1), 1.1f)
            .Add((1, false, 1, 0x010, 0), 1.2f)
            .Add((1, false, 1, 0x001, -4), 0.7f)
            .Add((1, false, 1, 0x001, -3), 0.9f)
            .Add((1, false, 1, 0x001, -2), 1.0f)
            .Add((1, false, 1, 0x001, -1), 1.2f)
            .Add((1, false, 1, 0x001, 0), 1.3f)
            .Add((1, false, 1, 0x110, -4), 0.9f)
            .Add((1, false, 1, 0x110, -3), 1.2f)
            .Add((1, false, 1, 0x110, -2), 1.5f)
            .Add((1, false, 1, 0x110, -1), 1.7f)
            .Add((1, false, 1, 0x110, 0), 1.9f)
            .Add((1, false, 1, 0x101, -4), 0.9f)
            .Add((1, false, 1, 0x101, -3), 1.1f)
            .Add((1, false, 1, 0x101, -2), 1.3f)
            .Add((1, false, 1, 0x101, -1), 1.6f)
            .Add((1, false, 1, 0x101, 0), 1.7f)
            .Add((1, false, 1, 0x011, -4), 0.7f)
            .Add((1, false, 1, 0x011, -3), 0.9f)
            .Add((1, false, 1, 0x011, -2), 1.1f)
            .Add((1, false, 1, 0x011, -1), 1.3f)
            .Add((1, false, 1, 0x011, 0), 1.4f)
            .Add((1, false, 1, 0x111, -4), 1.1f)
            .Add((1, false, 1, 0x111, -3), 1.5f)
            .Add((1, false, 1, 0x111, -2), 1.8f)
            .Add((1, false, 1, 0x111, -1), 2.1f)
            .Add((1, false, 1, 0x111, 0), 2.4f)

            // 2 outs
            .Add((1, false, 2, 0x000, -4), 0.2f)
            .Add((1, false, 2, 0x000, -3), 0.3f)
            .Add((1, false, 2, 0x000, -2), 0.3f)
            .Add((1, false, 2, 0x000, -1), 0.4f)
            .Add((1, false, 2, 0x000, 0), 0.4f)
            .Add((1, false, 2, 0x100, -4), 0.4f)
            .Add((1, false, 2, 0x100, -3), 0.5f)
            .Add((1, false, 2, 0x100, -2), 0.6f)
            .Add((1, false, 2, 0x100, -1), 0.7f)
            .Add((1, false, 2, 0x100, 0), 0.8f);
    // TODO: pick up here with the rest of the table

    public static float GetLeverageIndex(byte inning, bool isTopOfInning, sbyte outs, bool runnerOnFirst, bool runnerOnSecond,
        bool runnerOnThird, sbyte homeTeamRunDifference)
    {
        Guard.IsInRange(inning, (byte) 1, (byte) 9, nameof(inning));
        Guard.IsInRange(outs, (sbyte) 0, (sbyte) 2, nameof(outs));

        if (homeTeamRunDifference is > 4 or < -4)
        {
            return 0.0f;
        }

        var baseState = 0;
        if (runnerOnFirst) {
            baseState |= 1 << 2;
        }
        if (runnerOnSecond) {
            baseState |= 1 << 1;
        }
        if (runnerOnThird) {
            baseState |= 1;
        }
        
        var tuple = (inning, isTopOfInning, outs, baseState, homeTeamRunDifference);
        if (Indices.TryGetValue(tuple, out var leverageIndex))
        {
            return leverageIndex;
        }

        return 0.0f;
        // throw new ArgumentException($"No leverage index found for tuple {tuple}");
    }
}