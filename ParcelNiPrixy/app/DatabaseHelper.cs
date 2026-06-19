using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace ParcelTrackingSystem
{
    public static class DatabaseHelper
    {
        private const string Server   = "localhost";
        private const string Database = "parcel_tracking_db";
        private const string UserId   = "root";
        private const string Password = ""; // ← update if you have a password

        public static string ConnectionString =>
            $"Server={Server};Port=3306;Database={Database};Uid={UserId};Pwd={Password};CharSet=utf8;";

        public static MySqlConnection GetConnection()
        {
            var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public static DataTable ExecuteProcedureTable(string proc, params MySqlParameter[] parameters)
        {
            using var conn    = GetConnection();
            using var cmd     = new MySqlCommand(proc, conn);
            cmd.CommandType   = CommandType.StoredProcedure;
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            using var adapter = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public static int ExecuteProcedureNonQuery(string proc, params MySqlParameter[] parameters)
        {
            using var conn  = GetConnection();
            using var cmd   = new MySqlCommand(proc, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteNonQuery();
        }

        public static MySqlCommand ExecuteProcedureWithOutput(MySqlConnection conn, string proc, params MySqlParameter[] parameters)
        {
            var cmd = new MySqlCommand(proc, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            cmd.ExecuteNonQuery();
            return cmd;
        }
    }
}
