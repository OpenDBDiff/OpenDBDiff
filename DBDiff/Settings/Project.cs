using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DBDiff.Schema.SQLServer.Generates.Options;

namespace DBDiff.Settings
{
    using Newtonsoft.Json;

    public class Project
    {
        public enum ProjectType
        {
            SQLServer = 1
        }

        private const string connectionFormatSQLLite = "Data Source={0};Pooling=true;FailIfMissing=false";
        private const string connectionDBFileSQLLite = "Settings.conf";
        private static bool stillCaringAboutSqlLiteProjectErrors = true;

        public int Id { get; set; }
        public string ConnectionStringSource { get; set; }
        public string ConnectionStringDestination { get; set; }
        public SqlOption Options { get; set; }
        public ProjectType Type { get; set; }
        public string Name { get; set; }

        private static void ReallyDoSqlSomething(string sqlCommand, Action<SQLiteDataReader> OnRead, params string[] sqlNonQueryBeforeReadCommand)
        {
            string dbFile = connectionDBFileSQLLite;
            if (!File.Exists(dbFile))
            {
                dbFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dbFile);
            }

            using (var connection = new SQLiteConnection(String.Format(connectionFormatSQLLite, dbFile)))
            {
                // TODO: Don't know about Sqlite; hoping transaction is necessary?
                connection.Open();
                bool useTransaction = sqlNonQueryBeforeReadCommand != null && sqlNonQueryBeforeReadCommand.Length > 0 && !String.IsNullOrEmpty(sqlNonQueryBeforeReadCommand[0]);
                using (var transaction = useTransaction ? connection.BeginTransaction() : null)
                using (var command = new SQLiteCommand(sqlCommand, connection, null))
                {
                    if (transaction != null)
                    {
                        foreach (string sqlBeforeCommand in sqlNonQueryBeforeReadCommand)
                        {
                            if (!String.IsNullOrEmpty(sqlBeforeCommand))
                            {
                                using (var beforeCommand = new SQLiteCommand(sqlBeforeCommand, connection, null))
                                {
                                    beforeCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    if (OnRead != null)
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OnRead(reader);
                            }
                        }
                    }
                    else
                    {
                        command.ExecuteNonQuery();
                    }

                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
            }
        }

        private static void DoSqlSomething(string sqlCommand, Action<SQLiteDataReader> OnRead, params string[] sqlNonQueryBeforeReadCommand)
        {
            try
            {
                ReallyDoSqlSomething(sqlCommand, OnRead, sqlNonQueryBeforeReadCommand);
            }
            catch (Exception terribleCatchAllErrorsClause)
            {
                if (stillCaringAboutSqlLiteProjectErrors
                    && DialogResult.No == MessageBox.Show(terribleCatchAllErrorsClause.Message + "\n\nDo you want to see further errors?", "Project Error", MessageBoxButtons.YesNo))
                {
                    stillCaringAboutSqlLiteProjectErrors = false;
                }
            }
        }

        private static int Add(Project item)
        {
            int maxId = 0;
            DoSqlSomething(
                "SELECT MAX(ProjectId) AS NewId FROM Project WHERE Internal = 0",
                reader => maxId = int.Parse(reader["NewId"].ToString()),
                "INSERT INTO Project (Name, ConnectionStringSource, ConnectionStringDestination, Options, Type, Internal) VALUES ('" + item.Name.Replace("'", "''") + "','" + item.ConnectionStringSource + "','" + item.ConnectionStringDestination + "','" + SerializeOptions(item.Options) + "'," + ((int)item.Type).ToString() + ",0)");
            return maxId;
        }

        private static int Update(Project item)
        {
            DoSqlSomething(
                "UPDATE Project SET Name = '" + item.Name.Replace("'", "''") + "', ConnectionStringSource = '" + item.ConnectionStringSource + "', ConnectionStringDestination = '" + item.ConnectionStringDestination + "', Type = " + ((int)item.Type).ToString() + ", Options = '" + SerializeOptions(item.Options) + "'" + " WHERE ProjectId = " + item.Id.ToString(),
                null);
            return item.Id;
        }

        public static void Delete(int Id)
        {
            DoSqlSomething("DELETE FROM Project WHERE ProjectId = " + Id.ToString(), null);
        }

        public static int Save(Project item)
        {
            if (item.Id == 0)
                return Add(item);

            return Update(item);
        }

        public static void SaveLastConfiguration(String ConnectionStringSource, String ConnectionStringDestination)
        {
            if (GetLastConfiguration() != null)
            {
                DoSqlSomething("UPDATE Project SET ConnectionStringSource = '" + ConnectionStringSource + "', ConnectionStringDestination = '" + ConnectionStringDestination + "' WHERE Internal = 1", null);
            }
            else
            {
                DoSqlSomething("INSERT INTO Project (Name, ConnectionStringSource, ConnectionStringDestination, Options, Type, Internal) VALUES ('LastConfiguration','" + ConnectionStringSource + "','" + ConnectionStringDestination + "','',1,1)", null);
            }
        }

        public static Project GetLastConfiguration()
        {
            Project item = null;
            DoSqlSomething(
                //"SELECT * FROM Project WHERE Internal = 1 ORDER BY Name",
                "SELECT * FROM Project ORDER BY ProjectId DESC LIMIT 1",
                reader => item = new Project
                {
                    Id = int.Parse(reader["ProjectId"].ToString()),
                    ConnectionStringSource = reader["ConnectionStringSource"].ToString(),
                    ConnectionStringDestination = reader["ConnectionStringDestination"].ToString(),
                    Type = (ProjectType)(long)reader["Type"],
                    Options = DeserializeOptions((string)reader["Options"]),
                    Name = reader["Name"].ToString()
                });
            
            return item;
        }

        public static List<Project> GetAll()
        {
            List<Project> items = new List<Project>();
            DoSqlSomething(
                "SELECT * FROM Project WHERE Internal = 0 ORDER BY Name",
                reader => items.Add(new Project
                {
                    Id = int.Parse(reader["ProjectId"].ToString()),
                    ConnectionStringSource = reader["ConnectionStringSource"].ToString(),
                    ConnectionStringDestination =
                       reader["ConnectionStringDestination"].ToString(),
                    Type = (ProjectType)(long)reader["Type"],
                    Options = DeserializeOptions((string)reader["Options"]),
                    Name = reader["Name"].ToString()
                }));
            return items;
        }

        private static string SerializeOptions(SqlOption options)
        {
            if (options == null)
            {
                return string.Empty;
            }

            //Escape single quote in JSON due to SQLite standard
            return JsonConvert.SerializeObject(options).Replace("'", "''");
        }

        private static SqlOption DeserializeOptions(string options)
        {
            if (String.IsNullOrEmpty(options))
            {
                return new SqlOption();
            }

            return JsonConvert.DeserializeObject<SqlOption>(options);
        }
    }
}
