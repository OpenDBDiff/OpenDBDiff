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
        private const string sqliteSettingsFile = "settings.sqlite";
        private static bool showSqliteErrors = true;

        public int Id { get; set; }
        public string ConnectionStringSource { get; set; }
        public string ConnectionStringDestination { get; set; }
        public Schema.Model.IOption Options { get; set; }
        public ProjectType Type { get; set; }
        public string ProjectName { get; set; }
        public DateTime SavedDateTime { get; private set; }

        private static bool loading = false;

        static Project()
        {
            EnsureSqliteSettingsFileExists();
        }

        private static void EnsureSqliteSettingsFileExists()
        {
            loading = true;
            if (!File.Exists(SqliteSettingsFilePath) || !SchemaOk())
            {
                ExecuteSchema(SqliteSettingsFilePath);
            }
            loading = false;
        }

        private static bool SchemaOk()
        {
            try
            {
                bool tableExists = false;

                return TryExecuteSqliteCommand
                    (
                        commandText: "SELECT count(name) tableCount FROM sqlite_master WHERE type='table' and name = 'project' COLLATE NOCASE;",
                        onRead: r => tableExists = int.Parse(r["tableCount"].ToString()) == 1
                    ) && tableExists;
            }
            catch
            {
                return false;
            }
        }

        private static void ExecuteSchema(string sqliteSettingsFilePath)
        {
            TryExecuteSqliteCommand
            (
                commandText: @"
                    CREATE TABLE [project] (
                        ProjectId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                        ProjectName VARCHAR(80) NOT NULL,
                        ConnectionStringSource VARCHAR(500) NULL,
                        ConnectionStringDestination VARCHAR(500) NULL,
                        Options TEXT NULL,
                        Type INTEGER NOT NULL,
                        SavedDateTime TEXT NOT NULL,
                        IsLastConfiguration BOOLEAN NULL
                    )"
            );

            if (!SchemaOk())
                throw new InvalidOperationException("Fatal error: Unable to create project settings file.");
        }

        private static string SqliteSettingsFilePath
        {
            get
            {
                var userLocalAppDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), nameof(OpenDBDiff));
                if (!Directory.Exists(userLocalAppDataDirectory)) Directory.CreateDirectory(userLocalAppDataDirectory);

                return Path.Combine(userLocalAppDataDirectory, sqliteSettingsFile);
            }
        }


        private static void ExecuteSqliteCommand(string commandText, Action<SQLiteDataReader> onRead, params string[] sqlNonQueryBeforeReadCommand)
        {
            if (!loading)
                // This will execute a command to check if the project table exists
                // It's maybe not required for every SQLite command that is executed?
                // But if the file is deleted while OpenDBDiff is open, we don't want crashes, right?
                EnsureSqliteSettingsFileExists();

            using (var connection = new SQLiteConnection(String.Format(sqliteConnectionStringTemplate, SqliteSettingsFilePath)))
            {
                // TODO: Don't know about Sqlite; hoping transaction is necessary?
                connection.Open();
                bool useTransaction = sqlNonQueryBeforeReadCommand != null && sqlNonQueryBeforeReadCommand.Length > 0 && !String.IsNullOrEmpty(sqlNonQueryBeforeReadCommand[0]);
                using (var transaction = useTransaction ? connection.BeginTransaction() : null)
                using (var command = new SQLiteCommand(commandText, connection, null))
                {
                    try
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

                        if (onRead != null)
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    onRead(reader);
                                }
                            }
                        }
                        else
                        {
                            command.ExecuteNonQuery();
                        }

                        transaction?.Commit();
                    }
                    catch
                    {
                        transaction?.Rollback();
                        throw;
                    }
                }
            }
        }

        private static bool TryExecuteSqliteCommand(string commandText)
        {
            return TryExecuteSqliteCommand(commandText, null);
        }

        private static bool TryExecuteSqliteCommand(string commandText, Action<SQLiteDataReader> onRead)
        {
            return TryExecuteSqliteCommand(commandText, onRead, null);
        }

        private static bool TryExecuteSqliteCommand(string commandText, Action<SQLiteDataReader> onRead, params string[] sqlNonQueryBeforeReadCommand)
        {
            try
            {
                ExecuteSqliteCommand(commandText, onRead, sqlNonQueryBeforeReadCommand);
                return true;
            }
            catch (Exception ex)
            {
                if (showSqliteErrors)
                    showSqliteErrors = MessageBox.Show($"{ex.Message}\n\nDo you want to see further errors?\n\n{ex.ToString()}", "Project error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes;

                return false;
            }
        }

        private static int Add(Project item)
        {
            int maxId = 0;
            TryExecuteSqliteCommand(
                "SELECT MAX(ProjectId) AS NewId FROM project WHERE IsLastConfiguration = 0",
                r => maxId = int.Parse(r["NewId"].ToString()),
                $"INSERT INTO project (ProjectName, ConnectionStringSource, ConnectionStringDestination, Options, Type, SavedDateTime, IsLastConfiguration) VALUES ('{item.ProjectName.Replace("'", "''")}','{item.ConnectionStringSource}','{item.ConnectionStringDestination}','{item.GetSerializedOptions()}',{(int)item.Type},'{DateTime.Now.ToString("o")}',0)");
            return maxId;
        }

        protected virtual string GetSerializedOptions()
        {
            return Options.Serialize();
        }

        private static int Update(Project item)
        {
            TryExecuteSqliteCommand(
                $"UPDATE project SET ProjectName = '{item.ProjectName.Replace("'", "''")}', ConnectionStringSource = '{item.ConnectionStringSource}', ConnectionStringDestination = '{item.ConnectionStringDestination}', Type = {(int)item.Type}, SavedDateTime = '{DateTime.Now.ToString("o")}' WHERE ProjectId = {item.Id}");
            return item.Id;
        }

        public static void Delete(int Id)
        {
            TryExecuteSqliteCommand($"DELETE FROM project WHERE ProjectId = {Id}");
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
                TryExecuteSqliteCommand($"UPDATE project SET ConnectionStringSource = '{ConnectionStringSource}', ConnectionStringDestination = '{ConnectionStringDestination}', SavedDateTime = '{DateTime.Now.ToString("o")}' WHERE IsLastConfiguration = 1");
            }
            else
            {
                TryExecuteSqliteCommand($"INSERT INTO project (ProjectName, ConnectionStringSource, ConnectionStringDestination, Options, Type, SavedDateTime, IsLastConfiguration) VALUES ('LastConfiguration','{ConnectionStringSource}','{ConnectionStringDestination}','',1,'{DateTime.Now.ToString("o")}',1)");
            }
        }

        public static Project GetLastConfiguration()
        {
            Project item = null;
            TryExecuteSqliteCommand(
                "SELECT * FROM project WHERE IsLastConfiguration = 1 ORDER BY ProjectName LIMIT 1",
                r => item = new Project
                {
                    Id = int.Parse(r["ProjectId"].ToString()),
                    ConnectionStringSource = r["ConnectionStringSource"].ToString(),
                    ConnectionStringDestination = r["ConnectionStringDestination"].ToString(),
                    Type = (ProjectType)(long)r["Type"],
                    //Options = (SqlOption) reader["Options"],
                    ProjectName = r["ProjectName"].ToString(),
                    SavedDateTime = DateTime.Parse(r["SavedDateTime"].ToString())
                });
            return item;
        }

        public static IList<Project> GetAll()
        {
            var projects = new List<Project>();
            TryExecuteSqliteCommand(
                "SELECT * FROM project WHERE IsLastConfiguration = 0 ORDER BY ProjectName",
                r => projects.Add(new Project
                {
                    Id = int.Parse(r["ProjectId"].ToString()),
                    ConnectionStringSource = r["ConnectionStringSource"].ToString(),
                    ConnectionStringDestination = r["ConnectionStringDestination"].ToString(),
                    Type = (ProjectType)(long)r["Type"],
                    //Options = (SqlOption) reader["Options"],
                    ProjectName = r["ProjectName"].ToString(),
                    SavedDateTime = DateTime.Parse(r["SavedDateTime"].ToString())
                }));
            return projects;
        }
    }
}
