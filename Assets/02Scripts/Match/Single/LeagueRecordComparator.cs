using System.Collections.Generic;
public class RankedTeamData
{
    public int rank;
    public LeagueTeamRecord record;
}

public class LeagueRecordComparator : IComparer<LeagueTeamRecord>
{
    public int Compare(LeagueTeamRecord x, LeagueTeamRecord y)
    {
        int pointCompare = y.points.CompareTo(x.points);

        if (pointCompare != 0)
        {
            return pointCompare;
        }

        int winCompare = y.wins.CompareTo(x.wins);

        if (winCompare != 0)
        {
            return winCompare;
        }

        return 0;
    }
}