using HomeRunTracker.Core.Models.Details;

namespace HomeRunTracker.Core.Interfaces;

public interface ILeverageIndexService
{
    public float GetLeverageIndex(PlayDto play);
}