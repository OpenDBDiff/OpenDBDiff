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

        private static string GetSQL()
        {
            string sql;
            sql = "SELECT SUBSTRING(CONVERT(varchar,SERVERPROPERTY('productversion')),1,PATINDEX('.',CONVERT(varchar,SERVERPROPERTY('productversion')))+1) AS Version";
            return sql;
        }

        public DatabaseInfo Get()
        {
            DatabaseInfo item = new DatabaseInfo();
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            item.VersionNumber = float.Parse(reader["Version"].ToString());
                        }
                    }
                }
            }
            return item;
        }
    }
}
