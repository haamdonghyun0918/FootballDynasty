using System.Collections.Generic;

public class TeamStanding
{
    public string teamName;
    public bool isPlayer;
    public int wins;
    public int draws;
    public int losses;
    public int points;

    public int GetMatchesPlayed()
    {
        return wins + draws + losses;
    }
}

public class SingleLeagueAi
{
    public static List<TeamStanding> CreateStandings(string playerTeamName, int playerWins, int playerDraws, int playerLosses, int playerPoints, string selectedTeamsCsv)
    {
        List<TeamStanding> standings = new List<TeamStanding>();

        TeamStanding playerStanding = new TeamStanding();
        playerStanding.teamName = playerTeamName;
        playerStanding.isPlayer = true;
        playerStanding.wins = playerWins;
        playerStanding.draws = playerDraws;
        playerStanding.losses = playerLosses;
        playerStanding.points = playerPoints;
        standings.Add(playerStanding);

        if (selectedTeamsCsv != null)
        {
            if (selectedTeamsCsv != string.Empty)
            {
                string[] aiTeams = selectedTeamsCsv.Split(',');
                for (int i = 0; i < aiTeams.Length; i++)
                {
                    TeamStanding aiStanding = new TeamStanding();
                    aiStanding.teamName = aiTeams[i];
                    aiStanding.isPlayer = false;
                    aiStanding.wins = 0;
                    aiStanding.draws = 0;
                    aiStanding.losses = 0;
                    aiStanding.points = 0;

                    standings.Add(aiStanding);
                }
            }
        }

        return standings;
    }

    public static void SortStandings(List<TeamStanding> standings)
    {
        for (int i = 0; i < standings.Count - 1; i++)
        {
            for (int j = 0; j < standings.Count - 1 - i; j++)
            {
                bool needSwap = false;
                if (standings[j].points < standings[j + 1].points)
                {
                    needSwap = true;
                }

                else if (standings[j].points == standings[j + 1].points)
                {
                    if (standings[j].wins < standings[j + 1].wins)
                    {
                        needSwap = true;
                    }
                }

                if (needSwap == true)
                {
                    TeamStanding temp = standings[j];
                    standings[j] = standings[j + 1];
                    standings[j + 1] = temp;
                }
            }
        }
    }

    public static void SimulateRoundAiMatches(string selectedTeamsCsv, ref Dictionary<string, int> aiWins, ref Dictionary<string, int> aiDraws, ref Dictionary<string, int> aiLosses, ref Dictionary<string, int> aiPoints)
    {
        if (selectedTeamsCsv == null)
        {
            return;
        }

        if (selectedTeamsCsv == string.Empty)
        {
            return;
        }

        string[] aiTeams = selectedTeamsCsv.Split(',');

        for (int i = 0; i < aiTeams.Length - 1; i = i + 2)
        {
            string homeTeam = aiTeams[i];
            string awayTeam = aiTeams[i + 1];

            int homeScore = UnityEngine.Random.Range(0, 4);
            int awayScore = UnityEngine.Random.Range(0, 4);

            if (aiWins.ContainsKey(homeTeam) == false)
            {
                aiWins[homeTeam] = 0;
                aiDraws[homeTeam] = 0;
                aiLosses[homeTeam] = 0;
                aiPoints[homeTeam] = 0;
            }

            if (aiWins.ContainsKey(awayTeam) == false)
            {
                aiWins[awayTeam] = 0;
                aiDraws[awayTeam] = 0;
                aiLosses[awayTeam] = 0;
                aiPoints[awayTeam] = 0;
            }

            if (homeScore > awayScore)
            {
                aiWins[homeTeam] = aiWins[homeTeam] + 1;
                aiPoints[homeTeam] = aiPoints[homeTeam] + 3;

                aiLosses[awayTeam] = aiLosses[awayTeam] + 1;
            }

            else if (homeScore < awayScore)
            {
                aiWins[awayTeam] = aiWins[awayTeam] + 1;
                aiPoints[awayTeam] = aiPoints[awayTeam] + 3;

                aiLosses[homeTeam] = aiLosses[homeTeam] + 1;
            }

            else
            {
                aiDraws[homeTeam] = aiDraws[homeTeam] + 1;
                aiPoints[homeTeam] = aiPoints[homeTeam] + 1;

                aiDraws[awayTeam] = aiDraws[awayTeam] + 1;
                aiPoints[awayTeam] = aiPoints[awayTeam] + 1;
            }
        }
    }
}