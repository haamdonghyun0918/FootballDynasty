using System;
using System.Collections.Generic;

public class LeaguePlayerSetting
{
    private Random rand = new Random();

    public void ApplyDifficulty(List<TeamSquad> teamSquads, List<SkillData> allSkills, int difficultyLevel, Dictionary<PlayerData, List<SkillData>> outSkillMapping)
    {
        int statBonus = (difficultyLevel - 7) * 5;

        List<SkillData> attackPool = new List<SkillData>();
        List<SkillData> midPool = new List<SkillData>();
        List<SkillData> defensePool = new List<SkillData>();

        for (int i = 0; i < allSkills.Count; i++)
        {
            SkillData skill = allSkills[i];

            if (skill.Pos == "FW")
            {
                attackPool.Add(skill);
            }

            else if (skill.Pos == "MF")
            {
                midPool.Add(skill);
            }

            else if (skill.Pos == "DF")
            {
                defensePool.Add(skill);
            }
        }

        for (int i = 0; i < teamSquads.Count; i++)
        {
            TeamSquad squad = teamSquads[i];

            ProcessPlayers(squad.StartingLineup, statBonus, attackPool, midPool, defensePool, outSkillMapping);
            ProcessPlayers(squad.SubPlayers, statBonus, attackPool, midPool, defensePool, outSkillMapping);
        }
    }

    private void ProcessPlayers(List<PlayerData> players, int statBonus, List<SkillData> attackPool, List<SkillData> midPool, List<SkillData> defensePool, Dictionary<PlayerData, List<SkillData>> outSkillMapping)
    {
        for (int i = 0; i < players.Count; i++)
        {
            PlayerData player = players[i];

            player.Att = Math.Max(1, player.Att + statBonus);
            player.Mid = Math.Max(1, player.Mid + statBonus);
            player.Def = Math.Max(1, player.Def + statBonus);

            List<SkillData> assignedSkills = new List<SkillData>();

            if (attackPool.Count > 0)
            {
                assignedSkills.Add(attackPool[rand.Next(0, attackPool.Count)]);
            }

            if (midPool.Count > 0)
            {
                assignedSkills.Add(midPool[rand.Next(0, midPool.Count)]);
            }

            if (defensePool.Count > 0)
            {
                assignedSkills.Add(defensePool[rand.Next(0, defensePool.Count)]);
            }

            if (outSkillMapping.ContainsKey(player) == false)
            {
                outSkillMapping.Add(player, assignedSkills);
            }
        }
    }
}