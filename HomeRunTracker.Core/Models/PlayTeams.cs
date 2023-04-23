using HomeRunTracker.Core.Models.Details;

namespace HomeRunTracker.Core.Models;

public readonly record struct PlayTeams
{
    public PlayTeams(PlayDto play, GameDetailsDto gameDetails)
    {
        var isTopInning = play.IsTopInning;

        if (isTopInning)
        {
            BatterTeamId = gameDetails.AwayTeam.Id;
            BatterTeamName = gameDetails.AwayTeam.Name;

            PitcherTeamId = gameDetails.HomeTeam.Id;
            PitcherTeamName = gameDetails.HomeTeam.Name;
        }
        else
        {
            BatterTeamId = gameDetails.HomeTeam.Id;
            BatterTeamName = gameDetails.HomeTeam.Name;

            PitcherTeamId = gameDetails.AwayTeam.Id;
            PitcherTeamName = gameDetails.AwayTeam.Name;
        }
    }

    public int BatterTeamId { get; }

    public string BatterTeamName { get; }

    public int PitcherTeamId { get; }

    public string PitcherTeamName { get; }

    public void Deconstruct(out int batterTeamId, out int pitchTeamId, out string batterTeamName,
        out string pitcherTeamName, out bool isTopInning)
    {
        batterTeamId = BatterTeamId;
        pitchTeamId = PitcherTeamId;
        batterTeamName = BatterTeamName;
        pitcherTeamName = PitcherTeamName;
        isTopInning = BatterTeamId == PitcherTeamId;
    }
}