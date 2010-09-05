using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.SQLServer.Options;

namespace DBDiff.Schema.SQLServer.Generates
{
    public static class GenerateSchemas
    {
        private static string GetSQL()
        {
            string sql;
            sql = "select S1.name,S1.schema_id, S2.name AS Owner from sys.schemas S1 ";
            sql += "INNER JOIN sys.schemas S2 ON S2.schema_id = S1.principal_id ";
            //sql += "WHERE S1.schema_id <> S1.principal_id";
            return sql;
        }

        public static void Fill(Database database, string connectioString)
        {
            if (database.Options.Ignore.FilterSchema)
            {
                using (SqlConnection conn = new SqlConnection(connectioString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Model.Schema item = new Model.Schema(database);
                                item.Id = (int)reader["schema_id"];
                                item.Name = reader["name"].ToString();
                                item.Owner = reader["owner"].ToString();
                                database.Schemas.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
