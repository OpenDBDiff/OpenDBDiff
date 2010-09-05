using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.SQLServer.Options;

namespace DBDiff.Schema.SQLServer.Generates
{
    public class GenerateSchemas
    {
        private string connectioString;
        private SqlOption objectFilter;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateSchemas(string connectioString, SqlOption filter)
        {
            this.connectioString = connectioString;
            this.objectFilter = filter;
        }

        private static string GetSQL()
        {
            string sql;
            sql = "select S1.name,S1.schema_id, S2.name AS Owner from sys.schemas S1 ";
            sql += "INNER JOIN sys.schemas S2 ON S2.schema_id = S1.principal_id ";
            sql += "WHERE S1.schema_id <> S1.principal_id";
            return sql;
        }

        public Schemas Get(Database database)
        {
            Schemas types = new Schemas(database);
            if (objectFilter.OptionFilter.FilterSchema)
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
