using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace OpenDBDiff.Settings
{
    public class Project
    {
        public enum ProjectType
        {
            SQLServer = 1
        }

        private const string sqliteConnectionStringTemplate = "Data Source={0};Pooling=true;FailIfMissing=false";
        private const string sqliteSettingsFile = "Settings.conf";
        private static bool showSqliteErrors = true;

        public int Id { get; set; }
        public string ConnectionStringSource { get; set; }
        public string ConnectionStringDestination { get; set; }
        public Schema.Model.IOption Options { get; set; }
        public ProjectType Type { get; set; }
        public string Name { get; set; }

        private static void ExecuteSqliteCommand(string sqlCommand, Action<SQLiteDataReader> OnRead, params string[] sqlNonQueryBeforeReadCommand)
        {
            string dbFile = sqliteSettingsFile;
            if (!File.Exists(dbFile))
            {
                dbFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dbFile);
            }

            using (var connection = new SQLiteConnection(String.Format(sqliteConnectionStringTemplate, dbFile)))
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

        private static bool TryExecuteSqliteCommand(string sqlCommand)
        {
            return TryExecuteSqliteCommand(sqlCommand, null);
        }

        private static bool TryExecuteSqliteCommand(string sqlCommand, Action<SQLiteDataReader> OnRead)
        {
            return TryExecuteSqliteCommand(sqlCommand, OnRead, null);
        }

        private static bool TryExecuteSqliteCommand(string sqlCommand, Action<SQLiteDataReader> OnRead, params string[] sqlNonQueryBeforeReadCommand)
        {
            try
            {
                ExecuteSqliteCommand(sqlCommand, OnRead, sqlNonQueryBeforeReadCommand);
                return true;
            }
            catch (Exception ex)
            {
                if (showSqliteErrors
                    && DialogResult.No == MessageBox.Show(ex.Message + "\n\nDo you want to see further errors?", "Project error", MessageBoxButtons.YesNo, MessageBoxIcon.Error))
                {
                    showSqliteErrors = false;
                }
                return false;
            }
        }

        private static int Add(Project item)
        {
            int maxId = 0;
            TryExecuteSqliteCommand(
                "SELECT MAX(ProjectId) AS NewId FROM Project WHERE Internal = 0",
                reader => maxId = int.Parse(reader["NewId"].ToString()),
                "INSERT INTO Project (Name, ConnectionStringSource, ConnectionStringDestination, Options, Type, Internal) VALUES ('" + item.Name.Replace("'", "''") + "','" + item.ConnectionStringSource + "','" + item.ConnectionStringDestination + "','" + item.GetSerializedOptions() + "'," + ((int)item.Type).ToString() + ",0)");
            return maxId;
        }

        protected virtual string GetSerializedOptions()
        {
            return Options.Serialize();
        }

        private static int Update(Project item)
        {
            TryExecuteSqliteCommand(
                "UPDATE Project SET Name = '" + item.Name.Replace("'", "''") + "', ConnectionStringSource = '" + item.ConnectionStringSource + "', ConnectionStringDestination = '" + item.ConnectionStringDestination + "', Type = " + ((int)item.Type).ToString() + " WHERE ProjectId = " + item.Id.ToString());
            return item.Id;
        }

        public static void Delete(int Id)
        {
            TryExecuteSqliteCommand("DELETE FROM Project WHERE ProjectId = " + Id.ToString());
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
                TryExecuteSqliteCommand("UPDATE Project SET ConnectionStringSource = '" + ConnectionStringSource + "', ConnectionStringDestination = '" + ConnectionStringDestination + "' WHERE Internal = 1");
            }
            else
            {
                TryExecuteSqliteCommand("INSERT INTO Project (Name, ConnectionStringSource, ConnectionStringDestination, Options, Type, Internal) VALUES ('LastConfiguration','" + ConnectionStringSource + "','" + ConnectionStringDestination + "','',1,1)");
            }
        }

        public static Project GetLastConfiguration()
        {
            Project item = null;
            TryExecuteSqliteCommand(
                "SELECT * FROM Project WHERE Internal = 1 ORDER BY Name",
                reader => item = new Project
                {
                    Id = int.Parse(reader["ProjectId"].ToString()),
                    ConnectionStringSource = reader["ConnectionStringSource"].ToString(),
                    ConnectionStringDestination = reader["ConnectionStringDestination"].ToString(),
                    Type = (ProjectType)(long)reader["Type"],
                    //Options = (SqlOption) reader["Options"],
                    Name = reader["Name"].ToString()
                });
            return item;
        }

        public static List<Project> GetAll()
        {
            List<Project> items = new List<Project>();
            TryExecuteSqliteCommand(
                "SELECT * FROM Project WHERE Internal = 0 ORDER BY Name",
                reader => items.Add(new Project
                {
                    Id = int.Parse(reader["ProjectId"].ToString()),
                    ConnectionStringSource = reader["ConnectionStringSource"].ToString(),
                    ConnectionStringDestination =
                       reader["ConnectionStringDestination"].ToString(),
                    Type = (ProjectType)(long)reader["Type"],
                    //Options = (SqlOption) reader["Options"],
                    Name = reader["Name"].ToString()
                }));
            return items;
        }
    }
}
