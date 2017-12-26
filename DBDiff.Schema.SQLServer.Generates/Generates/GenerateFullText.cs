using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateFullText
    {
        private Generate root;

        public GenerateFullText(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            return SQLQueries.SQLQueryFactory.Get("DBDiff.Schema.SQLServer.Generates.SQLQueries.GetFullTextCatalogs");
        }

        public void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterFullText)
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
                                FullText item = new FullText(database);
                                item.Id = (int)reader["fulltext_catalog_id"];
                                item.Name = reader["Name"].ToString();
                                item.Owner = reader["Owner"].ToString();
                                item.IsAccentSensity = (bool)reader["is_accent_sensitivity_on"];
                                item.IsDefault = (bool)reader["is_default"];
                                if (!reader.IsDBNull(reader.GetOrdinal("path")))
                                    item.Path = reader["path"].ToString().Substring(0, reader["path"].ToString().Length - item.Name.Length);
                                if (!reader.IsDBNull(reader.GetOrdinal("FileGroupName")))
                                    item.FileGroupName = reader["FileGroupName"].ToString();
                                database.FullText.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
