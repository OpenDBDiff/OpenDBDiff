using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Options;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Generates
{
    public static class GenerateSynonyms
    {
        private static string GetSQL()
        {
            string sql = "SELECT SCHEMA_NAME(schema_id) AS Owner,name,object_id,base_object_name from sys.synonyms ORDER BY Name";
            return sql;
        }

        public static void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterSynonyms)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        conn.Open();
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Synonym item = new Synonym(database);
                                item.Id = (int)reader["object_id"];
                                item.Name = reader["Name"].ToString();
                                item.Owner = reader["Owner"].ToString();
                                item.Value = reader["base_object_name"].ToString();
                                database.Synonyms.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
