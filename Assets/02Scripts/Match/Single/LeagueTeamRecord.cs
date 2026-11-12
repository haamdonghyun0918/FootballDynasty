public class LeagueTeamRecord
{
    public string teamName;
    public int wins;
    public int draws;
    public int losses;
    public int points;
    public bool isUserTeam;
}

public class LeagueMatchData
{
    public int roundIndex;
    public string homeTeam;
    public string awayTeam;
    public int homeScore;
    public int awayScore;
    public bool isPlayerHome;
}