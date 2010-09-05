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
    public class GenerateRules
    {
        private string connectioString;
        private SqlOption objectFilter;
        //public event ProgressHandler OnTableProgress;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateRules(string connectioString, SqlOption filter)
        {
            this.connectioString = connectioString;
            this.objectFilter = filter;
        }

        private static string GetSQL()
        {
            string sql = "select obj.object_id, Name, SCHEMA_NAME(obj.schema_id) AS Owner, ISNULL(smobj.definition, ssmobj.definition) AS [Definition] from sys.objects obj  ";
            sql += "LEFT OUTER JOIN sys.sql_modules AS smobj ON smobj.object_id = obj.object_id ";
            sql += "LEFT OUTER JOIN sys.system_sql_modules AS ssmobj ON ssmobj.object_id = obj.object_id ";
            sql += "where obj.type='R'";
            return sql;
        }

        public Rules Get(Database database)
        {
            Rules types = new Rules(database);
            if (objectFilter.OptionFilter.FilterRules)
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
                                Rule item = new Rule(database);
                                item.Id = (int)reader["object_id"];
                                item.Name = reader["Name"].ToString();
                                item.Owner = reader["Owner"].ToString();
                                item.Value = reader["Definition"].ToString();
                                types.Add(item);
                            }
                        }
                    }
                }
            }
            return types;
        }
    }
}
