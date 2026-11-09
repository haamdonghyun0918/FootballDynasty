using System;
using System.Collections.Generic;

public class LeaguePlayerSetting
{
    private Random rand = new Random();

    public void ApplyDifficulty(List<TeamSquad> teamSquads, List<SkillData> allSkills, int difficultyLevel)
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

            ProcessPlayers(squad.StartingLineup, statBonus, attackPool, midPool, defensePool);
            ProcessPlayers(squad.SubPlayers, statBonus, attackPool, midPool, defensePool);
        }
    }

    private void ProcessPlayers(List<PlayerData> players, int statBonus, List<SkillData> attackPool, List<SkillData> midPool, List<SkillData> defensePool)
    {
        for (int i = 0; i < players.Count; i++)
        {
            PlayerData player = players[i];

            player.Att = Math.Max(1, player.Att + statBonus);
            player.Mid = Math.Max(1, player.Mid + statBonus);
            player.Def = Math.Max(1, player.Def + statBonus);

            if (attackPool.Count > 0)
            {
                int attIndex = rand.Next(0, attackPool.Count);
                SkillData assignedAttackSkill = attackPool[attIndex];
            }

            if (midPool.Count > 0)
            {
                int midIndex = rand.Next(0, midPool.Count);
                SkillData assignedMidSkill = midPool[midIndex];
            }

            if (defensePool.Count > 0)
            {
                int defIndex = rand.Next(0, defensePool.Count);
                SkillData assignedDefenseSkill = defensePool[defIndex];
            }
        }
    }
}