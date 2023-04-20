namespace HomeRunTracker.Frontend.Models;

public readonly record struct RgbColor(byte R, byte G, byte B)
{
    public override string ToString()
    {
        return $"rgb({R}, {G}, {B})";
    }
}