using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Generates.Options;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates
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
            string sql = "SELECT obj.object_id, Name, SCHEMA_NAME(obj.schema_id) AS Owner, ISNULL(smobj.definition, ssmobj.definition) AS [Definition] from sys.objects obj  ";
            sql += "LEFT OUTER JOIN sys.sql_modules AS smobj ON smobj.object_id = obj.object_id ";
            sql += "LEFT OUTER JOIN sys.system_sql_modules AS ssmobj ON ssmobj.object_id = obj.object_id ";
            sql += "where obj.type='D'";
            return sql;
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
