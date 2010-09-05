using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Options;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Generates
{
    public class GenerateDatabase
    {
        private string connectioString;
        private SqlOption objectFilter;

                /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateDatabase(string connectioString, SqlOption filter)
        {
            this.connectioString = connectioString;
            this.objectFilter = filter;
        }

        private static string GetSQL(Database databaseSchema)
        {
            string sql;
            sql = "SELECT DATABASEPROPERTYEX('" + databaseSchema.Name + "','Collation') AS Collation, SUBSTRING(CONVERT(varchar,SERVERPROPERTY('productversion')),1,PATINDEX('.',CONVERT(varchar,SERVERPROPERTY('productversion')))+2) AS Version";
            return sql;
        }

        public DatabaseInfo Get(Database databaseSchema)
        {
            DatabaseInfo item = new DatabaseInfo();
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand(GetSQL(databaseSchema), conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            item.Collation = reader["Collation"].ToString();
                            item.VersionNumber = float.Parse(reader["Version"].ToString().Replace(".",""));
                        }
                    }
                }
            }
            return item;
        }


    }
}
