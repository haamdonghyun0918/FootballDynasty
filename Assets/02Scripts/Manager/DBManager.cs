using UnityEngine;
using System;
using MySqlConnector;
using System.Collections.Generic;

public class DBManager : MonoBehaviour
{
    public static DBManager Instance { get; private set; }
    public string CurrentUserName { get; private set; }
    public string CurrentUserId { get; private set; }

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

    private void Start()
    {
        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                Debug.Log("MYSQL 연결 성공");

                string query = "SELECT * FROM userinfo";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string userId = reader.GetString("UserId");
                            string userName = reader.GetString("UserName");

                            Debug.Log($"가져온 유저 ID: {userId}, 유저 닉네임: {userName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"MYSQL 연결 또는 조회 실패: {ex.Message}");
            }
        }
    }

    public bool InsertUser(string userId, string userPw, string userName)
    {
        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string query = "INSERT INTO userinfo (UserId, UserPw, UserName) VALUES (@userId, @userPw, @userName)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@userPw", userPw);
                    cmd.Parameters.AddWithValue("@userName", userName);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"유저 추가 실패: {ex.Message}");
                return false;
            }
        }
    }

    public bool DeleteUser(string userId)
    {
        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string query = "DELETE FROM userinfo WHERE UserId = @userId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"유저 삭제 실패: {ex.Message}");
                return false;
            }
        }
    }

    public bool ValidateUser(string userId, string userPw)
    {
        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string query = "SELECT UserName FROM userinfo WHERE UserId = @userId AND UserPw = @userPw";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@userPw", userPw);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        CurrentUserName = result.ToString();
                        CurrentUserId = userId;
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"로그인 실패: {ex.Message}");
                return false;
            }
        }
    }

    public bool IsUserIdExists(string userId)
    {
        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM userinfo WHERE UserId = @userId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"ID 존재 여부 확인 실패: {ex.Message}");
                return false;
            }
        }
    }

    public List<UserCardData> GetUserCards(string userId)
    {
        List<UserCardData> cardList = new List<UserCardData>();
        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string query = "SELECT InstanceId, UserId, PlayerId, SkillFWId, SkillMFId, SkillDFId FROM usercards WHERE UserId = @userId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserCardData card = new UserCardData
                            {
                                InstanceId = reader.GetInt32("InstanceId"),
                                UserId = reader.GetString("UserId"),
                                PlayerId = reader.GetString("PlayerId"),
                                SkillFWId = reader.GetString("SkillFWId"),
                                SkillMFId = reader.GetString("SkillMFId"),
                                SkillDFId = reader.GetString("SkillDFId")
                            };

                            cardList.Add(card);
                        }
                    }
                }
                Debug.Log($"[DBManager] 유저({userId})의 카드 {cardList.Count}장 로드 완료");
            }
            catch (Exception ex)
            {
                Debug.LogError($"보유 카드 조회 실패: {ex.Message}");
            }
        }
        return cardList;
    }

    public bool InsertUserCard(string userId, string playerId)
    {
        var allSkills = GameDataManager.Instance.GetAllData<SkillData>();

        List<SkillData> fwSkills = new List<SkillData>();
        List<SkillData> mfSkills = new List<SkillData>();
        List<SkillData> dfSkills = new List<SkillData>();

        if (allSkills != null)
        {
            foreach (var skill in allSkills)
            {
                if (skill.Pos == "FW")
                {
                    fwSkills.Add(skill);
                }
                else if (skill.Pos == "MF")
                {
                    mfSkills.Add(skill);
                }
                else if (skill.Pos == "DF")
                {
                    dfSkills.Add(skill);
                }
            }
        }

        string randomFwId = string.Empty;
        if (fwSkills.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, fwSkills.Count);
            randomFwId = fwSkills[index].Id;
        }

        string randomMfId = string.Empty;
        if (mfSkills.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, mfSkills.Count);
            randomMfId = mfSkills[index].Id;
        }

        string randomDfId = string.Empty;
        if (dfSkills.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, dfSkills.Count);
            randomDfId = dfSkills[index].Id;
        }

        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string query = "INSERT INTO usercards (UserId, PlayerId, SkillFWId, SkillMFId, SkillDFId) VALUES (@userId, @playerId, @skillFW, @skillMF, @skillDF)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@playerId", playerId);
                    cmd.Parameters.AddWithValue("@skillFW", randomFwId);
                    cmd.Parameters.AddWithValue("@skillMF", randomMfId);
                    cmd.Parameters.AddWithValue("@skillDF", randomDfId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Debug.Log($"[DBManager] 카드 및 랜덤 스킬 획득 성공! 유저: {userId}, 선수: {playerId}");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"카드 획득 저장 실패: {ex.Message}");
            }
        }
        return false;
    }

    public bool SaveUserFormation(string userId, string formation)
    {
        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string query = "UPDATE userinfo SET Formation = @formation WHERE UserId = @userId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@formation", formation);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"포메이션 저장 실패: {ex.Message}");
                return false;
            }
        }
    }

    public string GetUserFormation(string userId)
    {
        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string query = "SELECT Formation FROM userinfo WHERE UserId = @userId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    object result = cmd.ExecuteScalar();

                    return result != null ? result.ToString() : string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"포메이션 조회 실패: {ex.Message}");
                return string.Empty;
            }
        }
    }

    public bool SaveUserSquadSlot(string userId, string slotKey, int instanceId)
    {
        string connString = DBConfig.GetConnection();
        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string query = @"INSERT INTO usersquad (UserId, SlotKey, InstanceId) VALUES (@userId, @slotKey, @instanceId) ON DUPLICATE KEY UPDATE InstanceId = @instanceId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@slotKey", slotKey);
                    cmd.Parameters.AddWithValue("@instanceId", instanceId);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"스쿼드 슬롯 저장 실패 ({slotKey}): {ex.Message}");
                return false;
            }
        }
    }

    public Dictionary<string, int> GetUserSquadSlots(string userId)
    {
        Dictionary<string, int> squadDict = new Dictionary<string, int>();

        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string query = "SELECT SlotKey, InstanceId FROM usersquad WHERE UserId = @userId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string slotKey = reader.GetString("SlotKey");
                            int instanceId = reader.IsDBNull(reader.GetOrdinal("InstanceId")) ? -1 : reader.GetInt32("InstanceId");
                            squadDict[slotKey] = instanceId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"스쿼드 슬롯 조회 실패: {ex.Message}");
            }
        }
        return squadDict;
    }

    public int GetUserLeagueDifficulty(string userId)
    {
        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string query = "SELECT LeagueDifficulty FROM userinfo WHERE UserId = @userId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    object result = cmd.ExecuteScalar();

                    return result != null ? Convert.ToInt32(result) : 1;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"리그 난이도 조회 실패: {ex.Message}");
                return 1;
            }
        }
    }

    public bool SaveSingleModeData(string userId, int difficultyLevel, List<TeamSquad> leagueSquads)
    {
        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string[] clearQueries = {
                            "DELETE FROM singleleague_aisquad WHERE UserId = @userId",
                            "DELETE FROM singleleague_aiteams WHERE UserId = @userId",
                            "DELETE FROM singleleague_matches WHERE UserId = @userId",
                            "DELETE FROM singleleague_season WHERE UserId = @userId"
                        };

                        foreach (string q in clearQueries)
                        {
                            using (MySqlCommand cmd = new MySqlCommand(q, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@userId", userId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        string insertSeason = "INSERT INTO singleleague_season (UserId, LeagueDifficulty, CurrentRound, Wins, Draws, Losses, Points) VALUES (@userId, @difficulty, 1, 0, 0, 0, 0)";
                        
                        using (MySqlCommand cmd = new MySqlCommand(insertSeason, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@userId", userId);
                            cmd.Parameters.AddWithValue("@difficulty", difficultyLevel);
                            cmd.ExecuteNonQuery();
                        }

                        for (int i = 0; i < leagueSquads.Count; i++)
                        {
                            TeamSquad squad = leagueSquads[i];

                            string insertTeam = "INSERT INTO singleleague_aiteams (UserId, TeamName, Wins, Draws, Losses, Points) VALUES (@userId, @teamName, 0, 0, 0, 0)";
                            
                            using (MySqlCommand cmd = new MySqlCommand(insertTeam, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@userId", userId);
                                cmd.Parameters.AddWithValue("@teamName", squad.TeamName);
                                cmd.ExecuteNonQuery();
                            }

                            for (int s = 0; s < squad.StartingLineup.Count; s++)
                            {
                                PlayerData player = squad.StartingLineup[s];
                                string slotKey = $"Start_{s}";
                                
                                int.TryParse(player.Id, out int playerId);

                                string insertSquad = "INSERT INTO singleleague_aisquad (UserId, TeamName, SlotKey, PlayerId) VALUES (@userId, @teamName, @slotKey, @playerId)";
                                
                                using (MySqlCommand cmd = new MySqlCommand(insertSquad, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@userId", userId);
                                    cmd.Parameters.AddWithValue("@teamName", squad.TeamName);
                                    cmd.Parameters.AddWithValue("@slotKey", slotKey);
                                    cmd.Parameters.AddWithValue("@playerId", playerId);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            for (int sub = 0; sub < squad.SubPlayers.Count; sub++)
                            {
                                PlayerData player = squad.SubPlayers[sub];
                                string slotKey = $"Sub_{sub}";
                                
                                int.TryParse(player.Id, out int playerId);

                                string insertSquad = "INSERT INTO singleleague_aisquad (UserId, TeamName, SlotKey, PlayerId) VALUES (@userId, @teamName, @slotKey, @playerId)";
                               
                                using (MySqlCommand cmd = new MySqlCommand(insertSquad, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@userId", userId);
                                    cmd.Parameters.AddWithValue("@teamName", squad.TeamName);
                                    cmd.Parameters.AddWithValue("@slotKey", slotKey);
                                    cmd.Parameters.AddWithValue("@playerId", playerId);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                        Debug.Log($"[DBManager] 유저({userId})의 싱글리그 시즌 데이터 DB 저장 성공!");
                        return true;
                    }
                    catch (Exception innerEx)
                    {
                        transaction.Rollback();
                        Debug.LogError($"싱글리그 데이터 트랜잭션 롤백 발생: {innerEx.Message}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"싱글리그 데이터 DB 연결 실패: {ex.Message}");
                return false;
            }
        }
    }

    public List<LeagueTeamRecord> GetLeagueRankRecords(string userId)
    {
        List<LeagueTeamRecord> recordList = new List<LeagueTeamRecord>();
        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string userQuery = @"SELECT u.UserName, s.Wins, s.Draws, s.Losses, s.Points FROM singleleague_season s JOIN userinfo u ON s.UserId = u.UserId WHERE s.UserId = @userId";

                using (MySqlCommand cmd = new MySqlCommand(userQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LeagueTeamRecord userRecord = new LeagueTeamRecord();
                            userRecord.teamName = reader.GetString("UserName");
                            userRecord.wins = reader.GetInt32("Wins");
                            userRecord.draws = reader.GetInt32("Draws");
                            userRecord.losses = reader.GetInt32("Losses");
                            userRecord.points = reader.GetInt32("Points");
                            userRecord.isUserTeam = true;

                            recordList.Add(userRecord);
                        }
                    }
                }

                string aiQuery = "SELECT TeamName, Wins, Draws, Losses, Points FROM singleleague_aiteams WHERE UserId = @userId";
                
                using (MySqlCommand cmd = new MySqlCommand(aiQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LeagueTeamRecord aiRecord = new LeagueTeamRecord();
                            aiRecord.teamName = reader.GetString("TeamName");
                            aiRecord.wins = reader.GetInt32("Wins");
                            aiRecord.draws = reader.GetInt32("Draws");
                            aiRecord.losses = reader.GetInt32("Losses");
                            aiRecord.points = reader.GetInt32("Points");
                            aiRecord.isUserTeam = false;

                            recordList.Add(aiRecord);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"리그 순위 데이터 조회 실패: {ex.Message}");
            }
        }
        return recordList;
    }

    public bool SaveMatchSchedule(string userId, int roundIndex, string homeTeam, string awayTeam, bool isPlayerHome, MySqlConnection conn, MySqlTransaction transaction)
    {
        string query = "INSERT INTO singleleague_matches (UserId, RoundIndex, HomeTeam, AwayTeam, HomeScore, AwayScore, IsPlayerHome) VALUES (@userId, @roundIndex, @homeTeam, @awayTeam, 0, 0, @isPlayerHome)";

        using (MySqlCommand cmd = new MySqlCommand(query, conn, transaction))
        {
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@roundIndex", roundIndex);
            cmd.Parameters.AddWithValue("@homeTeam", homeTeam);
            cmd.Parameters.AddWithValue("@awayTeam", awayTeam);
            cmd.Parameters.AddWithValue("@isPlayerHome", isPlayerHome);

            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public LeagueMatchData GetCurrentMatchInfo(string userId)
    {
        LeagueMatchData matchData = null;
        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string query = @"SELECT m.RoundIndex, m.HomeTeam, m.AwayTeam, m.HomeScore, m.AwayScore, m.IsPlayerHome FROM singleleague_season s JOIN singleleague_matches m ON s.UserId = m.UserId AND s.CurrentRound = m.RoundIndex WHERE s.UserId = @userId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            matchData = new LeagueMatchData
                            {
                                roundIndex = reader.GetInt32("RoundIndex"),
                                homeTeam = reader.GetString("HomeTeam"),
                                awayTeam = reader.GetString("AwayTeam"),
                                homeScore = reader.GetInt32("HomeScore"),
                                awayScore = reader.GetInt32("AwayScore"),
                                isPlayerHome = reader.GetBoolean("IsPlayerHome")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"현재 경기 정보 조회 실패: {ex.Message}");
            }
        }
        return matchData;
    }

    public List<LeagueMatchData> GetUserMatchHistory(string userId, string userTeamName)
    {
        List<LeagueMatchData> historyList = new List<LeagueMatchData>();
        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string query = @"SELECT m.RoundIndex, m.HomeTeam, m.AwayTeam, m.HomeScore, m.AwayScore, m.IsPlayerHome FROM singleleague_matches m JOIN singleleague_season s ON m.UserId = s.UserId WHERE m.UserId = @userId AND (m.HomeTeam = @teamName OR m.AwayTeam = @teamName) AND m.RoundIndex <= s.CurrentRound ORDER BY m.RoundIndex ASC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@teamName", userTeamName);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LeagueMatchData match = new LeagueMatchData
                            {
                                roundIndex = reader.GetInt32("RoundIndex"),
                                homeTeam = reader.GetString("HomeTeam"),
                                awayTeam = reader.GetString("AwayTeam"),
                                homeScore = reader.GetInt32("HomeScore"),
                                awayScore = reader.GetInt32("AwayScore"),
                                isPlayerHome = reader.GetBoolean("IsPlayerHome")
                            };
                            historyList.Add(match);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"유저 경기 기록 조회 실패: {ex.Message}");
            }
        }
        return historyList;
    }
}