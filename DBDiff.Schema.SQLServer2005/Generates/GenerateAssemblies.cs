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
    public class GenerateAssemblies
    {        
        private string connectioString;
        private SqlOption objectFilter;

                /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateAssemblies(string connectioString, SqlOption filter)
        {
            this.connectioString = connectioString;
            this.objectFilter = filter;
        }

        private static string GetSQL()
        {
            string sql = "SELECT clr_name, AF.assembly_id, A.name, S1.name AS Owner, permission_set_desc, is_visible,content ";
            sql += "FROM sys.assemblies A ";
            sql += "INNER JOIN sys.assembly_files AF ON AF.assembly_id = A.assembly_id ";
            sql += "INNER JOIN sys.schemas S1 ON S1.schema_id = A.principal_id";
            return sql;
        }

        public Assemblys Get(Database database)
        {
            Assemblys types = new Assemblys(database);
            if (objectFilter.OptionFilter.FilterAssemblies)
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
                                Assembly item = new Assembly(database);
                                item.Id = (int)reader["assembly_id"];
                                item.Name = reader["Name"].ToString();
                                item.Owner = reader["Owner"].ToString();
                                item.CLRName = reader["clr_name"].ToString();
                                item.PermissionSet = reader["permission_set_desc"].ToString();
                                byte[] buff = (byte[])reader["content"];
                                
                                StringBuilder sHex = new StringBuilder(2 * buff.Length);
                                for (int i = 0; i < buff.Length; i++)
                                    sHex.AppendFormat("{0:X2} ", buff[i]);

                                item.Content = "0x" + sHex.ToString().Replace(" ", String.Empty);

                                item.Visible = (bool)reader["is_visible"];
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
