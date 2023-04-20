namespace HomeRunTracker.Common.Enums;

public enum EPlayResult
{
    Unknown,
    Strikeout,
    Walk,
    FieldOut,
    ForceOut,
    SacrificeFly,
    Single,
    Double,
    Triple,
    HomeRun,
}

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
