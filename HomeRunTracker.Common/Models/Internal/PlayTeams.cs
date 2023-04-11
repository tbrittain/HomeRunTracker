using HomeRunTracker.Common.Models.Details;

namespace HomeRunTracker.Common.Models.Internal;

public readonly record struct PlayTeams
{
    public PlayTeams(MlbPlay play, MlbGameDetails gameDetails)
    {
        var isTopInning = play.About.IsTopInning;

        if (isTopInning)
        {
            BatterTeamId = gameDetails.GameData.TeamMatchup.AwayTeam.Id;
            BatterTeamName = gameDetails.GameData.TeamMatchup.AwayTeam.Name;

            PitcherTeamId = gameDetails.GameData.TeamMatchup.HomeTeam.Id;
            PitcherTeamName = gameDetails.GameData.TeamMatchup.HomeTeam.Name;
        }
        else
        {
            BatterTeamId = gameDetails.GameData.TeamMatchup.HomeTeam.Id;
            BatterTeamName = gameDetails.GameData.TeamMatchup.HomeTeam.Name;

            PitcherTeamId = gameDetails.GameData.TeamMatchup.AwayTeam.Id;
            PitcherTeamName = gameDetails.GameData.TeamMatchup.AwayTeam.Name;
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