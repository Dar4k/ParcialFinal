using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace Infrastructure.Data;

public static class BadDb
{
    public static string ConnectionString { get; private set; }

    public static void Initialize(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Sql");
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

        if (string.IsNullOrEmpty(dbPassword))
        {
            Console.WriteLine("ERROR: The database password is not set in the environment variable 'DB_PASSWORD'.");
        }

        ConnectionString = connectionString.Replace("{DB_PASSWORD}", dbPassword);
    }

    public static int ExecuteNonQueryUnsafe(string sql)
    {
        var conn = new SqlConnection(ConnectionString);
        var cmd = new SqlCommand(sql, conn);
        conn.Open();
        return cmd.ExecuteNonQuery();
    }

    public static IDataReader ExecuteReaderUnsafe(string sql)
    {
        var conn = new SqlConnection(ConnectionString);
        var cmd = new SqlCommand(sql, conn);
        conn.Open();
        return cmd.ExecuteReader();
    }
}