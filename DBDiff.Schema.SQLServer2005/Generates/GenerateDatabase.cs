using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Generates.Options;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.SQLServer.Generates.Generates.SQLCommands;

namespace DBDiff.Schema.SQLServer.Generates.Generates
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

        public DatabaseInfo Get(Database database)
        {
            DatabaseInfo item = new DatabaseInfo();
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand(DatabaseSQLCommand.GetVersion(database), conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //we must compare the decimal as well as Azure is 10.25
                            item.VersionNumber = float.Parse(reader["Version"].ToString());
                        }
                    }
                }
                using (SqlCommand command = new SqlCommand(DatabaseSQLCommand.Get(item.Version, database), conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            item.Collation = reader["Collation"].ToString();
                            item.HasFullTextEnabled = ((int)reader["IsFulltextEnabled"]) == 1;
                        }
                    }
                }

            }
            
            return item;
        }


    }
}
