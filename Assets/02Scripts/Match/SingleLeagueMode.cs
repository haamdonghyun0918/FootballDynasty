using UnityEngine;
using System.Collections.Generic;

public class FormationRule
{
    public string formationName;
    public int requiredGK;
    public int requiredDF;
    public int requiredMF;
    public int requiredFW;

    public FormationRule(string name, int gk, int df, int mf, int fw)
    {
        formationName = name;
        requiredGK = gk;
        requiredDF = df;
        requiredMF = mf;
        requiredFW = fw;
    }
}

public class OpponentPlayerCard
{
    public string playerId;
    public string playerName;
    public string position;
    public string teamName;
    public int attack;
    public int mid;
    public int defense;
    public SkillData assignedSkill;
}

public class SingleLeagueMode
{
    private readonly int[] statModifiers = new int[]
    {
        -30, -25, -20, -15, -10, -5, 0, 5, 10, 15, 20, 25, 30, 35, 40
    };

    public List<FormationRule> GetDefaultFormations()
    {
        List<FormationRule> pool = new List<FormationRule>();
        pool.Add(new FormationRule("4-5-1", 1, 4, 5, 1));
        pool.Add(new FormationRule("4-3-3", 1, 4, 3, 3));
        pool.Add(new FormationRule("4-4-2", 1, 4, 4, 2));
        pool.Add(new FormationRule("3-5-2", 1, 3, 5, 2));
        pool.Add(new FormationRule("3-4-3", 1, 3, 4, 3));
        return pool;
    }

    public class OpponentSquadResult
    {
        public string teamName;
        public FormationRule assignedFormation;
        public List<OpponentPlayerCard> startingLineup = new List<OpponentPlayerCard>();
        public List<OpponentPlayerCard> benchList = new List<OpponentPlayerCard>();
    }

    public List<string> GenerateRandomTeams()
    {
        var allPlayers = GameDataManager.Instance.GetAllData<PlayerData>();

        if (allPlayers == null)
        {
            Debug.LogError("PlayerData가 비었습니다.");
            return new List<string>();
        }

        if (allPlayers.Count == 0)
        {
            Debug.LogError("PlayerData가 비어있습니다.");
            return new List<string>();
        }

        HashSet<string> uniqueTeamSet = new HashSet<string>();
        for (int i = 0; i < allPlayers.Count; i++)
        {
            PlayerData player = allPlayers[i];
            if (player != null)
            {
                string team = player.Team;
                bool isAlreadyContained = uniqueTeamSet.Contains(team);
                if (isAlreadyContained == false)
                {
                    uniqueTeamSet.Add(team);
                }
            }
        }

        List<string> uniqueTeams = new List<string>(uniqueTeamSet);
        List<string> selectedTeams = new List<string>();

        int loopCount = Mathf.Min(15, uniqueTeams.Count);
        for (int i = 0; i < loopCount; i++)
        {
            int randomIndex = Random.Range(0, uniqueTeams.Count);
            selectedTeams.Add(uniqueTeams[randomIndex]);
            uniqueTeams.RemoveAt(randomIndex);
        }

        return selectedTeams;
    }

    private void ExtractPlayers(List<OpponentPlayerCard> sourceList, int requiredCode, List<OpponentPlayerCard> destinationList)
    {
        for (int i = 0; i < requiredCode; i++)
        {
            if (sourceList.Count > 0)
            {
                int randomIndex = Random.Range(0, sourceList.Count);
                destinationList.Add(sourceList[randomIndex]);
                sourceList.RemoveAt(randomIndex);
            }
        }
    }

    public OpponentSquadResult BuildOpponentSquad(string targetTeamName, int stageIndex)
    {
        OpponentSquadResult result = new OpponentSquadResult();
        result.teamName = targetTeamName;

        int modifier = 0;
        if (stageIndex >= 0)
        {
            if (stageIndex < statModifiers.Length)
            {
                modifier = statModifiers[stageIndex];
            }
        }

        List<FormationRule> formations = GetDefaultFormations();
        int randomFormationIndex = Random.Range(0, formations.Count);
        result.assignedFormation = formations[randomFormationIndex];

        var allPlayers = GameDataManager.Instance.GetAllData<PlayerData>();
        List<PlayerData> teamPlayers = new List<PlayerData>();

        for (int i = 0; i < allPlayers.Count; i++)
        {
            PlayerData p = allPlayers[i];

            if (p != null)
            {
                if (p.Team == targetTeamName)
                {
                    teamPlayers.Add(p);
                }
            }
        }

        var allSkills = GameDataManager.Instance.GetAllData<SkillData>();

        List<OpponentPlayerCard> processedPlayers = new List<OpponentPlayerCard>();

        for (int i = 0; i < teamPlayers.Count; i++)
        {
            PlayerData original = teamPlayers[i];
            OpponentPlayerCard card = new OpponentPlayerCard();

            card.playerId = original.Id;
            card.playerName = original.Name;
            card.position = original.Pos;
            card.teamName = original.Team;

            card.attack = Mathf.Max(1, original.Att + modifier);
            card.mid = Mathf.Max(1, original.Mid + modifier);
            card.defense = Mathf.Max(1, original.Def + modifier);

            if (allSkills != null)
            {
                if (allSkills.Count > 0)
                {
                    List<SkillData> candidateSkills = new List<SkillData>();
                    for (int j = 0; j < allSkills.Count; j++)
                    {
                        SkillData skill = allSkills[j];

                        if (skill != null)
                        {
                            bool isPositionEmpty = string.IsNullOrEmpty(skill.Pos);
                            bool isPositionMatch = (skill.Pos == original.Pos);

                            if (isPositionEmpty || isPositionMatch)
                            {
                                candidateSkills.Add(skill);
                            }
                        }
                    }

                    if (candidateSkills.Count == 0)
                    {
                        candidateSkills = allSkills;
                    }

                    int randomSkillIndex = Random.Range(0, candidateSkills.Count);
                    card.assignedSkill = candidateSkills[randomSkillIndex];
                }
            }

            processedPlayers.Add(card);
        }

        List<OpponentPlayerCard> gks = new List<OpponentPlayerCard>();
        List<OpponentPlayerCard> dfs = new List<OpponentPlayerCard>();
        List<OpponentPlayerCard> mfs = new List<OpponentPlayerCard>();
        List<OpponentPlayerCard> fws = new List<OpponentPlayerCard>();

        for (int i = 0; i < processedPlayers.Count; i++)
        {
            OpponentPlayerCard player = processedPlayers[i];
            if (player.position == "GK")
            {
                gks.Add(player);
            }

            else if (player.position == "DF")
            {
                dfs.Add(player);
            }

            else if (player.position == "MF")
            {
                mfs.Add(player);
            }

            else if (player.position == "FW")
            {
                fws.Add(player);
            }

            else
            {
                dfs.Add(player);
            }
        }

        ExtractPlayers(gks, result.assignedFormation.requiredGK, result.startingLineup);
        ExtractPlayers(dfs, result.assignedFormation.requiredDF, result.startingLineup);
        ExtractPlayers(mfs, result.assignedFormation.requiredMF, result.startingLineup);
        ExtractPlayers(fws, result.assignedFormation.requiredFW, result.startingLineup);

        List<OpponentPlayerCard> remainingPlayers = new List<OpponentPlayerCard>();
        remainingPlayers.AddRange(gks);
        remainingPlayers.AddRange(dfs);
        remainingPlayers.AddRange(mfs);
        remainingPlayers.AddRange(fws);

        int benchCount = Mathf.Min(10, remainingPlayers.Count);
        for (int i = 0; i < benchCount; i++)
        {
            int randomIndex = Random.Range(0, remainingPlayers.Count);
            result.benchList.Add(remainingPlayers[randomIndex]);
            remainingPlayers.RemoveAt(randomIndex);
        }

        return result;
    }
}