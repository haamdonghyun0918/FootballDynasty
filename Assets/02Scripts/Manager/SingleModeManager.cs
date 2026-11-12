using MySqlConnector;
using System;
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

        string userId = DBManager.Instance.CurrentUserId;
        string userTeamName = DBManager.Instance.CurrentUserName;

        DBManager.Instance.SaveSingleModeData(userId, difficultyLevel, currentLeagueSquads);
        GenerateLeagueSchedule(userId, userTeamName, currentLeagueSquads);
        Debug.Log($"[SingleModeManager] 싱글모드 세팅 및 38라운드 대진표 생성 완료! (난이도: {difficultyLevel}단계)");
    }

    public void ClearSingleModeData()
    {
        currentLeagueSquads.Clear();
        playerAssignedSkills.Clear();
        Debug.Log("[SingleModeManager] 싱글모드 리그 데이터가 초기화되었습니다.");
    }

    public List<RankedTeamData> GetRankedTeamList()
    {
        string userId = DBManager.Instance.CurrentUserId;
        List<LeagueTeamRecord> recordList = DBManager.Instance.GetLeagueRankRecords(userId);

        if (recordList == null || recordList.Count == 0)
        {
            return new List<RankedTeamData>();
        }

        recordList.Sort(new LeagueRecordComparator());

        List<RankedTeamData> rankedList = new List<RankedTeamData>();
        int currentRank = 1;

        for (int i = 0; i < recordList.Count; i++)
        {
            if (i > 0)
            {
                bool isSamePoints = (recordList[i].points == recordList[i - 1].points);
                bool isSameWins = (recordList[i].wins == recordList[i - 1].wins);

                if (!isSamePoints || !isSameWins)
                {
                    currentRank = i + 1;
                }
            }

            RankedTeamData rankedData = new RankedTeamData();
            rankedData.rank = currentRank;
            rankedData.record = recordList[i];
            rankedList.Add(rankedData);
        }
        return rankedList;
    }

    private void GenerateLeagueSchedule(string userId, string userTeamName, List<TeamSquad> aiTeams)
    {
        List<string> allTeams = new List<string>();
        allTeams.Add(userTeamName);

        for (int i = 0; i < aiTeams.Count; i++)
        {
            allTeams.Add(aiTeams[i].TeamName);
        }

        int totalTeams = allTeams.Count;
        int halfTeams = totalTeams / 2;
        int singleRoundCount = totalTeams - 1;

        List<string> rotatingTeams = new List<string>();
        for (int i = 1; i < totalTeams; i++)
        {
            rotatingTeams.Add(allTeams[i]);
        }

        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            conn.Open();

            using (MySqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    for (int round = 0; round < singleRoundCount; round++)
                    {
                        int roundIndex = round + 1;

                        string teamA = allTeams[0];
                        string teamB = rotatingTeams[round % rotatingTeams.Count];

                        bool isZeroHome = (round % 2 == 0);
                        string homeTeam = isZeroHome ? teamA : teamB;
                        string awayTeam = isZeroHome ? teamB : teamA;
                        bool isPlayerHome = (homeTeam == userTeamName);

                        DBManager.Instance.SaveMatchSchedule(userId, roundIndex, homeTeam, awayTeam, isPlayerHome, conn, transaction);

                        for (int i = 1; i < halfTeams; i++)
                        {
                            int team1Idx = (round + i) % rotatingTeams.Count;
                            int team2Idx = (round + singleRoundCount - i) % rotatingTeams.Count;

                            string t1 = rotatingTeams[team1Idx];
                            string t2 = rotatingTeams[team2Idx];

                            bool isT1Home = (i % 2 == 1);
                            string hTeam = isT1Home ? t1 : t2;
                            string aTeam = isT1Home ? t2 : t1;
                            bool isPlayerMatchHome = (hTeam == userTeamName);

                            DBManager.Instance.SaveMatchSchedule(userId, roundIndex, hTeam, aTeam, isPlayerMatchHome, conn, transaction);
                        }
                    }

                    for (int round = 0; round < singleRoundCount; round++)
                    {
                        int firstHalfRound = round + 1;
                        int secondHalfRound = firstHalfRound + singleRoundCount;

                        string getFirstHalfQuery = "SELECT HomeTeam, AwayTeam, IsPlayerHome FROM singleleague_matches WHERE UserId = @userId AND RoundIndex = @firstRound";

                        List<string[]> matchesToInvert = new List<string[]>();
                        List<bool> playerHomeList = new List<bool>();

                        using (MySqlCommand cmd = new MySqlCommand(getFirstHalfQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@userId", userId);
                            cmd.Parameters.AddWithValue("@firstRound", firstHalfRound);
                            
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string h = reader.GetString("HomeTeam");
                                    string a = reader.GetString("AwayTeam");
                                    bool pHome = reader.GetBoolean("IsPlayerHome");
                                    matchesToInvert.Add(new string[] { h, a });
                                    playerHomeList.Add(pHome);
                                }
                            }
                        }

                        for (int m = 0; m < matchesToInvert.Count; m++)
                        {
                            string newHome = matchesToInvert[m][1];
                            string newAway = matchesToInvert[m][0];
                            bool newIsPlayerHome = (newHome == userTeamName);

                            DBManager.Instance.SaveMatchSchedule(userId, secondHalfRound, newHome, newAway, newIsPlayerHome, conn, transaction);
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.LogError($"대진표 생성 및 저장 트랜잭션 실패: {ex.Message}");
                }
            }
        }
    }
}