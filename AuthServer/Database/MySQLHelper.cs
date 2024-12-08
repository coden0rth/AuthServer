using MySql.Data.MySqlClient;
using System;

namespace AuthServer
{
    public static class MySQLHelper
    {
        private static string connectionString = "Server=your_server;Database=your_db;Uid=your_user;Pwd=your_password;";

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
                    Logger.C($"Database error: {ex.Message}", Logger.MessageType.Alert);
                    return false;
                }
            }
        }

        public static bool Init()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    Logger.C("Connection to the database successful.", Logger.MessageType.Info);

                    string tableCheckQuery = "SHOW TABLES LIKE 'users'";
                    using (var cmd = new MySqlCommand(tableCheckQuery, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Logger.C("The 'users' table exists.", Logger.MessageType.Info);
                                return true;
                            }
                            else
                            {
                                Logger.C("The 'users' table does not exist.", Logger.MessageType.Info);
                                //TODO: Add function to create table
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.C($"Connection or table check error: {ex.Message}", Logger.MessageType.Alert);
                    return false;
                }
            }
        }
    }
}
