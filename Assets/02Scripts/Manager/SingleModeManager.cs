using UnityEngine;

using System.Collections.Generic;

public class SingleModeManager : MonoBehaviour
{
    public static SingleModeManager Instance { get; private set; }

    private SingleLeagueMode _leagueMode = new SingleLeagueMode();

    public SingleLeagueData CurrentSeasonData { get; private set; }

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

    public void StartNewLeagueSeason(int difficultyIndex)
    {
        string userId = DBManager.Instance.CurrentUserId;
        
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("[SingleModeManager] 로그인된 유저 ID가 없습니다.");
            return;
        }

        List<string> randomTeams = _leagueMode.GenerateRandomTeams();
        if (randomTeams == null)
        {
            Debug.LogError("[SingleModeManager] 팀 생성 실패");
            return;
        }

        if (randomTeams.Count == 0)
        {
            Debug.LogError("[SingleModeManager] 생성된 팀이 없습니다.");
            return;
        }

        string teamsCsv = string.Join(",", randomTeams);

        bool isSuccess = DBManager.Instance.InsertSingleLeagueSeason(userId, difficultyIndex, teamsCsv);
        if (isSuccess)
        {
            CurrentSeasonData = new SingleLeagueData();
            CurrentSeasonData.userId = userId;
            CurrentSeasonData.difficultyIndex = difficultyIndex;
            CurrentSeasonData.currentRound = 0;
            CurrentSeasonData.selectedTeamsCsv = teamsCsv;
            CurrentSeasonData.wins = 0;
            CurrentSeasonData.draws = 0;
            CurrentSeasonData.losses = 0;
            CurrentSeasonData.points = 0;

            Debug.Log("[SingleModeManager] 시즌 시작 및 DB 저장 완료!");
        }
    }
}