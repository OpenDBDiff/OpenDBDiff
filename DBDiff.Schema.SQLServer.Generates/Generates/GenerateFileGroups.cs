using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateFileGroups
    {
        private Generate root;

        public GenerateFileGroups(Generate root)
        {
            this.root = root;
        }

        private static string GetSQLFile(FileGroup filegroup)
        {
            string query = SQLQueries.SQLQueryFactory.Get("DBDiff.Schema.SQLServer.Generates.SQLQueries.GetDatabaseFile");

            return query.Replace("{ID}", filegroup.Id.ToString());
        }

        private static string GetSQL()
        {
            return SQLQueries.SQLQueryFactory.Get("DBDiff.Schema.SQLServer.Generates.SQLQueries.GetFileGroups");
        }

        private static void FillFiles(FileGroup filegroup, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(GetSQLFile(filegroup), conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FileGroupFile item = new FileGroupFile(filegroup);
                            item.Id = (int)reader["file_id"];
                            item.Name = reader["name"].ToString();
                            item.Owner = "";
                            item.Growth = (int)reader["growth"];
                            item.IsPercentGrowth = (bool)reader["is_percent_growth"];
                            item.IsSparse = (bool)reader["is_sparse"];
                            item.MaxSize = (int)reader["max_size"];
                            item.PhysicalName = reader["physical_name"].ToString();
                            item.Size = (int)reader["size"];
                            item.Type = (byte)reader["type"];
                            filegroup.Files.Add(item);
                        }
                    }
                }
            }
        }

        public void Fill(Database database, string connectionString)
        {
            try
            {
                if (database.Options.Ignore.FilterTableFileGroup)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                        {
                            conn.Open();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    FileGroup item = new FileGroup(database);
                                    item.Id = (int)reader["ID"];
                                    item.Name = reader["name"].ToString();
                                    item.Owner = "";
                                    item.IsDefaultFileGroup = (bool)reader["is_default"];
                                    item.IsReadOnly = (bool)reader["is_read_only"];
                                    item.IsFileStream = reader["type"].Equals("FD");
                                    FillFiles(item, connectionString);
                                    database.FileGroups.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
