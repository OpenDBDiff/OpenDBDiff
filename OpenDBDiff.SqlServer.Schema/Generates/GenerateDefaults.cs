﻿using Microsoft.Data.SqlClient;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Generates
{
    public class GenerateDefaults
    {
        private Generate root;

        public GenerateDefaults(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            return SQLQueries.SQLQueryFactory.Get("GetDefaults");
        }

        public void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterRules)
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
                                Default item = new Default(database);
                                item.Id = (int)reader["object_id"];
                                item.Name = reader["Name"].ToString();
                                item.Owner = reader["Owner"].ToString();
                                item.Value = reader["Definition"].ToString();
                                database.Defaults.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
