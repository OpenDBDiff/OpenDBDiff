using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using DBDiff.Schema.SQLServer.Generates.Options;

namespace DBDiff.Settings
{
    public class Project
    {
        public enum ProjectType
        {
            SQLServer = 1
        }

        private const string connectionSQLLite = "Data Source=Settings.conf;Pooling=true;FailIfMissing=false";

        public int Id { get; set; }
        public string ConnectionStringSource { get; set; }
        public string ConnectionStringDestination { get; set; }
        public SqlOption Options { get; set; }
        public ProjectType Type { get; set; }
        public string Name { get; set; }
 
        private static int Add(Project item)
        {
            int maxId = 0;
            using (SQLiteConnection connection = new SQLiteConnection(connectionSQLLite))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (
                        SQLiteCommand command = new SQLiteCommand("INSERT INTO Project (Name, ConnectionStringSource, ConnectionStringDestination, Options, Type, Internal) VALUES ('" + item.Name.Replace("'","''") + "','" + item.ConnectionStringSource + "','" + item.ConnectionStringDestination + "','" + item.Options + "'," + ((int) item.Type).ToString() + ",0)", connection, transaction))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SQLiteCommand command = new SQLiteCommand("SELECT MAX(ProjectId) AS NewId FROM Project WHERE Internal = 0",connection, transaction))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                maxId = int.Parse(reader["NewId"].ToString());
                            }
                        }
                    }
                    transaction.Commit();
                }
            }
            return maxId;
        }

        private static int Update(Project item)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionSQLLite))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("UPDATE Project SET Name = '" + item.Name.Replace("'","''") + "', ConnectionStringSource = '" + item.ConnectionStringSource + "', ConnectionStringDestination = '" + item.ConnectionStringDestination + "', Type = " + ((int)item.Type).ToString() + " WHERE ProjectId = " + item.Id.ToString(),connection))
                {
                    command.ExecuteNonQuery(); 
                }
            }
            return item.Id;
        }

        public static void Delete(int Id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionSQLLite))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("DELETE FROM Project WHERE ProjectId = " + Id.ToString(), connection))
                {
                    command.ExecuteNonQuery();
                }
            }
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
                using (SQLiteConnection connection = new SQLiteConnection(connectionSQLLite))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand("UPDATE Project SET ConnectionStringSource = '" + ConnectionStringSource + "', ConnectionStringDestination = '" + ConnectionStringDestination + "' WHERE Internal = 1", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }                
            }
            else
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionSQLLite))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand("INSERT INTO Project (Name, ConnectionStringSource, ConnectionStringDestination, Options, Type, Internal) VALUES ('LastConfiguration','" + ConnectionStringSource + "','" + ConnectionStringDestination + "','',1,1)", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static Project GetLastConfiguration()
        {
            Project item = null;
            using (SQLiteConnection connection = new SQLiteConnection(connectionSQLLite))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Project WHERE Internal = 1 ORDER BY Name", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            item = new Project
                            {
                                Id = int.Parse(reader["ProjectId"].ToString()),
                                ConnectionStringSource = reader["ConnectionStringSource"].ToString(),
                                ConnectionStringDestination = reader["ConnectionStringDestination"].ToString(),
                                Type = (ProjectType)(long)reader["Type"],
                                //Options = (SqlOption) reader["Options"],
                                Name = reader["Name"].ToString()
                            };                            
                        }
                    }
                }
            }
            return item;
            
        }

        public static List<Project> GetAll()
        {
            List<Project> items = new List<Project>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionSQLLite))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Project WHERE Internal = 0 ORDER BY Name",connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Project item = new Project
                                               {
                                                   Id = int.Parse(reader["ProjectId"].ToString()),
                                                   ConnectionStringSource = reader["ConnectionStringSource"].ToString(),
                                                   ConnectionStringDestination =
                                                       reader["ConnectionStringDestination"].ToString(),
                                                   Type = (ProjectType) (long) reader["Type"],
                                                   //Options = (SqlOption) reader["Options"],
                                                   Name = reader["Name"].ToString()
                                               };
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
        }
    }
}
