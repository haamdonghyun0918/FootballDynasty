using UnityEngine;
using System;
using MySqlConnector;

public class DBManager : MonoBehaviour
{
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

                            Debug.Log($"가져온 유저 ID: {userId}, 유저 이름: {userName}");
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
}