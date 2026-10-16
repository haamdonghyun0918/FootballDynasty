using UnityEngine;
using System;
using MySqlConnector;
using System.Collections.Generic;

public class DBManager : MonoBehaviour
{
    public static DBManager Instance { get; private set; }

    public string CurrentUserName { get; private set; }

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

                string query = "SELECT InstanceId, UserId, PlayerId FROM usercards WHERE UserId = @userId";

                using ( MySqlCommand cmd = new MySqlCommand(query, conn))
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
                                PlayerId = reader.GetString("PlayerId")
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
        string connString = DBConfig.GetConnection();

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();

                string query = "INSERT INTO usercards (UserId, PlayerId) VALUES (@userId, @playerId)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@playerId", playerId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Debug.Log($"[DBManager] 카드 획득 성공! 유저: {userId}, 선수: {playerId}");
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
}