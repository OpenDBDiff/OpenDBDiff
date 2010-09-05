using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.SQLServer2000.Model;

namespace DBDiff.Schema.SQLServer2000
{
    public class GenerateUserDataTypes
    {
        private string connectioString;
        //public event ProgressHandler OnTableProgress;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateUserDataTypes(string connectioString)
        {
            this.connectioString = connectioString;
        }

        public UserDataTypes Get(Database database)
        {
            UserDataTypes types = new UserDataTypes(database);
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand("sp_MShelptype null, 'uddt'", conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserDataType type = new UserDataType(database);
                            type.AllowNull = reader["nullable"].ToString().Equals("1");
                            type.Size = int.Parse(reader["length"].ToString());
                            type.Name = reader["UserDatatypeName"].ToString();
                            type.Owner = reader["owner"].ToString();
                            if (!reader["dt_prec"].ToString().Equals(""))
                                type.Precision = int.Parse(reader["dt_prec"].ToString());
                            if (!reader["dt_scale"].ToString().Equals(""))
                                type.Scale = int.Parse(reader["dt_scale"].ToString());
                            type.Type = reader["basetypename"].ToString();
                            types.Add(type);
                        }
                    }
                }
            }
            return types;
        }
    }
}
