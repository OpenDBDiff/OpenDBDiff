using System.Data.SqlClient;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Generates.Generates.SQLCommands;
using DBDiff.Schema.SQLServer.Generates.Generates.Util;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateFullTextIndex
    {
        private Generate root;

        public GenerateFullTextIndex(Generate root)
        {
            this.root = root;
        }

        public void Fill(Database database, string connectionString)
        {
            //not supported in azure yet
            if (database.Info.Version == DatabaseInfo.VersionTypeEnum.SQLServerAzure10) return;

            int parentId = 0;
            bool change = false;
            Table parent = null;
            root.RaiseOnReading(new ProgressEventArgs("Reading FullText Index...", Constants.READING_INDEXES));
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(FullTextIndexSQLCommand.Get(database.Info.Version), conn))
                {
                    conn.Open();
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        FullTextIndex item = null;
                        while (reader.Read())
                        {
                            root.RaiseOnReadingOne(reader["Name"]);
                            if (parentId != (int)reader["object_id"])
                            {
                                parentId = (int)reader["object_id"];
                                parent = database.Tables.Find(parentId);
                                change = true;
                            }
                            else
                                change = false;
                            if (change)
                            {
                                item = new FullTextIndex(parent);
                                item.Name = reader["Name"].ToString();
                                item.Owner = parent.Owner;
                                item.FullText = reader["FullTextCatalogName"].ToString();
                                item.Index = reader["IndexName"].ToString();
                                item.IsDisabled = !(bool)reader["is_enabled"];
                                item.ChangeTrackingState = reader["ChangeTracking"].ToString();
                                if (database.Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2008)
                                    item.FileGroup = reader["FileGroupName"].ToString();
                                ((Table)parent).FullTextIndex.Add(item);
                            }
                            FullTextIndexColumn ccon = new FullTextIndexColumn();
                            ccon.ColumnName = reader["ColumnName"].ToString();
                            ccon.Language = reader["LanguageName"].ToString();
                            item.Columns.Add(ccon);
                        }
                    }
                }
            }
        }
    }
}
