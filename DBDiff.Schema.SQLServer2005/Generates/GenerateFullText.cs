using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;
using System.Data.SqlClient;

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
            string sql = "SELECT S.Name as Owner,F.name AS FileGroupName, fulltext_catalog_id, FC.Name, path, FC.is_default, is_accent_sensitivity_on ";
            sql += "FROM sys.fulltext_catalogs FC ";
            sql += "LEFT JOIN sys.filegroups F ON F.data_space_id = FC.data_space_id ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = FC.principal_id";
            return sql;
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
