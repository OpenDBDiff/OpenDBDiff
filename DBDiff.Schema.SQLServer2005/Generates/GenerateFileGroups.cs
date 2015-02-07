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
            string sql;
            sql = "select file_id,";
            sql += "type,";
            sql += "name,";
            sql += "physical_name,";
            sql += "size,";
            sql += "max_size,";
            sql += "growth,";
            sql += "is_sparse,";
            sql += "is_percent_growth ";
            sql += "from sys.database_files WHERE data_space_id = " + filegroup.Id.ToString();
            return sql;
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

        private static string GetSQL()
        {
            string sql;
            sql = "SELECT  ";
            sql += "name, ";
            sql += "data_space_id AS [ID], ";
            sql += "is_default, ";
            sql += "is_read_only, ";
            sql += "type ";
            sql += "FROM sys.filegroups ORDER BY name";
            return sql;
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
                                    FillFiles(item,connectionString);
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
