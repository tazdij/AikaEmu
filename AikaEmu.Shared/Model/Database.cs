using System;
using System.Collections.Generic;
using System.Text;
using AikaEmu.Shared.Model.Configuration;
using AikaEmu.Shared.Utils;
using MySql.Data.MySqlClient;
using NLog;

namespace AikaEmu.Shared.Model
{
    public class Database<T> : Singleton<T> where T : class
    {
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();
        private string _connectionString;

        public virtual void Init(SqlConnection config)
        {
            _connectionString =
                $"server={config.Host}; port={config.Port}; database={config.Database}; uid={config.User}; password={config.Pass}; " +
                "charset=utf8; pooling=true; min pool size=0; max pool size=100;";

            MySqlConnection connection = null;
            try
            {
                connection = GetConnection();
            }
            finally
            {
                connection?.Close();
            }
        }

        public virtual MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public uint MySqlCommand(SqlCommandType sqlCommandType, string tableName, Dictionary<string, object> args, MySqlConnection connection,
            MySqlTransaction transaction = null)
        {
            var columns = new StringBuilder();
            var values = new StringBuilder();
            foreach (var key in args.Keys)
            {
                columns.AppendFormat("`{0}`, ", key);
                values.AppendFormat("@{0}, ", key);
            }

            string sqlQuery;
            switch (sqlCommandType)
            {
                case SqlCommandType.Insert:
                    sqlQuery = "INSERT";
                    break;
                case SqlCommandType.Replace:
                    sqlQuery = "REPLACE";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sqlCommandType), sqlCommandType, null);
            }

            var commandText = $"{sqlQuery} INTO `{tableName}`({columns.ToString().Trim(' ', ',')}) VALUES ({values.ToString().Trim(' ', ',')});";
            var command = new MySqlCommand(commandText, connection, transaction);
            foreach (var (key, value) in args)
                command.Parameters.AddWithValue("@" + key, value);

            command.ExecuteNonQuery();
            return (uint) command.LastInsertedId;
        }
    }
}