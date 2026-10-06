using UnityEngine;
using System;
using MySqlConnector;

public class DBManager : MonoBehaviour
{
    public static DBManager Instance { get; private set; }

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
                            string userId = reader["UserId"].ToString();
                            string userName = reader["UserName"].ToString();

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

                string query = "SELECT COUNT(*) FROM userinfo WHERE UserId = @userId AND UserPw = @userPw";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@userPw", userPw);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"로그인 실패: {ex.Message}");
                return false;
            }
        }
    }
}