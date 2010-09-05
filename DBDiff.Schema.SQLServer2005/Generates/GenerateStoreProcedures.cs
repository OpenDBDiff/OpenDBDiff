using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.SQLServer.Options;

namespace DBDiff.Schema.SQLServer.Generates
{
    public static class GenerateStoreProcedures
    {
        private static string GetSQL()
        {
            string sql = "";
            sql += "select OBJECT_DEFINITION(P.object_id) AS Text, P.object_id, S.name as owner, P.name as name from sys.procedures P ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = P.schema_id ";
            return sql;
        }

        public static void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterStoreProcedure)
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
                                StoreProcedure item = new StoreProcedure(database);
                                item.Id = (int)reader["object_id"];
                                item.Name = reader["name"].ToString();
                                item.Owner = reader["owner"].ToString();
                                item.Text = reader["text"].ToString();
                                database.Procedures.Add(item);
                            }
                        }
                    }                    
                }
            }
        }
    }
}
