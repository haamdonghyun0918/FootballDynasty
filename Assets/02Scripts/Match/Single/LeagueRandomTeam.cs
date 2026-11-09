using System;
using System.Collections.Generic;

public class TeamSquad
{
    public string TeamName;
    public string FormationType;
    public List<PlayerData> StartingLineup = new List<PlayerData>(11);
    public List<PlayerData> SubPlayers = new List<PlayerData>();
}

public class LeagueRandomTeam
{
    private string[] formations = new string[] { "4-3-3", "4-4-2", "4-5-1", "3-5-2", "3-4-3" };
    private Random rand = new Random();

    public List<TeamSquad> GenerateLeagueTeams(List<PlayerData> allPlayers)
    {
        List<string> allTeamNames = new List<string>();
        for (int i = 0; i < allPlayers.Count; i++)
        {
            string team = allPlayers[i].Team;
            bool isDuplicate = false;

            for (int j =0; j < allTeamNames.Count; j++)
            {
                if (allTeamNames[j] == team)
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (isDuplicate == false)
            {
                allTeamNames.Add(team);
            }
        }

        List<string> selectedTeams = new List<string>();
        int targetCount = Math.Min(19, allTeamNames.Count);

        for (int i = 0; i <targetCount; i++)
        {
            int index = rand.Next(0, allTeamNames.Count);
            selectedTeams.Add(allTeamNames[index]);
            allTeamNames.RemoveAt(index);
        }

        List<TeamSquad> leagueSquads = new List<TeamSquad>();
        for (int i = 0; i < selectedTeams.Count; i++)
        {
            TeamSquad squad = BuildTeamSquad(selectedTeams[i], allPlayers);

            if (squad != null)
            {
                leagueSquads.Add(squad);
            }
        }
        return leagueSquads;
    }

    private TeamSquad BuildTeamSquad(string teamName, List<PlayerData> allPlayers)
    {
        List<PlayerData> teamPlayers = new List<PlayerData>();

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].Team == teamName)
            {
                teamPlayers.Add(allPlayers[i]);
            }
        }

        if (teamPlayers.Count < 11)
        {
            return null;
        }

        TeamSquad squad = new TeamSquad();
        squad.TeamName = teamName;

        string chosenFormation = formations[rand.Next(0, formations.Length)];
        squad.FormationType = chosenFormation;

        int reqDF = 4, reqMF = 3, reqFW = 3;

        if (chosenFormation == "4-3-3") 
        { 
            reqDF = 4; reqMF = 3; reqFW = 3;
        }

        else if (chosenFormation == "4-4-2") 
        { 
            reqDF = 4; reqMF = 4; reqFW = 2; 
        }

        else if (chosenFormation == "4-5-1") 
        { 
            reqDF = 4; reqMF = 5; reqFW = 1; 
        }

        else if (chosenFormation == "3-5-2") 
        { 
            reqDF = 3; reqMF = 5; reqFW = 2; 
        }

        else if (chosenFormation == "3-4-3") 
        { 
            reqDF = 3; reqMF = 4; reqFW = 3; 
        }

        List<PlayerData> gks = new List<PlayerData>();
        List<PlayerData> dfs = new List<PlayerData>();
        List<PlayerData> mfs = new List<PlayerData>();
        List<PlayerData> fws = new List<PlayerData>();

        for (int i = 0; i < teamPlayers.Count; i++)
        {
            string pos = teamPlayers[i].Pos;
            if (pos == "GK")
            {
                gks.Add(teamPlayers[i]);
            }

            else if (pos == "DF")
            {
                dfs.Add(teamPlayers[i]);
            }

            else if (pos == "MF")
            {
                mfs.Add(teamPlayers[i]);
            }

            else if (pos == "FW")
            {
                fws.Add(teamPlayers[i]);
            }
        }

        PickRandomPlayers(gks, squad.StartingLineup, 1);
        PickRandomPlayers(dfs, squad.StartingLineup, reqDF);
        PickRandomPlayers(mfs, squad.StartingLineup, reqMF);
        PickRandomPlayers(fws, squad.StartingLineup, reqFW);

        for (int i = 0; i < gks.Count; i++)
        {
            squad.SubPlayers.Add(gks[i]);
        }

        for (int i = 0; i < dfs.Count; i++)
        {
            squad.SubPlayers.Add(dfs[i]);
        }

        for (int i = 0; i < mfs.Count; i++)
        {
            squad.SubPlayers.Add(mfs[i]);
        }

        for (int i = 0; i < fws.Count; i++)
        {
            squad.SubPlayers.Add(fws[i]);
        }

        return squad;
    }

    private void PickRandomPlayers(List<PlayerData> sourcePool, List<PlayerData> targetList, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (sourcePool.Count > 0)
            {
                int index = rand.Next(0, sourcePool.Count);
                targetList.Add(sourcePool[index]);
                sourcePool.RemoveAt(index);
            }
        }
    }
}