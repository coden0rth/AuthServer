using MySql.Data.MySqlClient;
using System;

namespace AuthServer
{
    public static class MySQLHelper
    {
        private static string connectionString = "Server=;Database=;Uid=;Pwd=;";

        public static bool AuthenticateUser(string username, string password)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE username=@username AND password=SHA2(@password, 256)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        int result = Convert.ToInt32(cmd.ExecuteScalar());
                        return result > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
