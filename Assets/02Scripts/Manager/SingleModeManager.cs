using System.Collections.Generic;
using UnityEngine;

public class SingleModeManager : MonoBehaviour
{
    public static SingleModeManager Instance { get; private set; }

    private List<TeamSquad> currentLeagueSquads = new List<TeamSquad>();
    private Dictionary<PlayerData, List<SkillData>> playerAssignedSkills = new Dictionary<PlayerData, List<SkillData>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartSingleMode(int difficultyLevel)
    {
        currentLeagueSquads.Clear();
        playerAssignedSkills.Clear();

        List<PlayerData> allPlayers = GameDataManager.Instance.GetAllData<PlayerData>();
        List<SkillData> allSkills = GameDataManager.Instance.GetAllData<SkillData>();

        if (allPlayers == null || allPlayers.Count == 0 || allSkills == null || allSkills.Count == 0)
        {
            Debug.LogError("[SingleModeManager] 선수 또는 스킬 데이터를 불러오지 못했습니다.");
            return;
        }

        LeagueRandomTeam teamGenerator = new LeagueRandomTeam();
        currentLeagueSquads = teamGenerator.GenerateLeagueTeams(allPlayers);
        LeaguePlayerSetting playerSetting = new LeaguePlayerSetting();
        playerSetting.ApplyDifficulty(currentLeagueSquads, allSkills, difficultyLevel, playerAssignedSkills);
        DBManager.Instance.SaveSingleModeData(DBManager.Instance.CurrentUserId, difficultyLevel, currentLeagueSquads);
        Debug.Log($"[SingleModeManager] 싱글모드 세팅 완료! (난이도: {difficultyLevel}단계, 팀 수: {currentLeagueSquads.Count})");
    }

    public void ClearSingleModeData()
    {
        currentLeagueSquads.Clear();
        playerAssignedSkills.Clear();
        Debug.Log("[SingleModeManager] 싱글모드 리그 데이터가 초기화되었습니다.");
    }
}