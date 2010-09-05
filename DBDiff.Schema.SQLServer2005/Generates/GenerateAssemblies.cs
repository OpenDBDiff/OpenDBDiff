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
    public static class GenerateAssemblies
    {        
        private static string GetSQL()
        {
            string sql = "SELECT AF.content AS FileContent, AF.File_Id AS FileId, AF.Name AS FileName, '[' + S2.Name + '].[' + AT.Name + ']' as UDTName, clr_name, AF.assembly_id, A.name, S1.name AS Owner, permission_set_desc, is_visible,content ";
            sql += "FROM sys.assemblies A ";
            sql += "INNER JOIN sys.assembly_files AF ON AF.assembly_id = A.assembly_id ";
            sql += "INNER JOIN sys.schemas S1 ON S1.schema_id = A.principal_id ";
            sql += "LEFT JOIN sys.assembly_types AT ON AT.assembly_id = A.assembly_id ";
            sql += "LEFT JOIN sys.schemas S2 ON S2.schema_id = AT.schema_id ";
            sql += "ORDER BY A.Name";
            return sql;
        }

        private static string ToHex(byte[] stream)
        {
            StringBuilder sHex = new StringBuilder(2 * stream.Length);
            for (int i = 0; i < stream.Length; i++)
                sHex.AppendFormat("{0:X2} ", stream[i]);
            return "0x" + sHex.ToString().Replace(" ", String.Empty);
        }

        public static void Fill(Database database, string connectionString)
        {
            int lastViewId = 0;
            if (database.Options.Ignore.FilterAssemblies)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Assembly item = null;
                            while (reader.Read())
                            {                                
                                if (lastViewId != (int)reader["assembly_id"])
                                {
                                    item = new Assembly(database);
                                    item.Id = (int)reader["assembly_id"];
                                    item.Name = reader["Name"].ToString();
                                    item.Owner = reader["Owner"].ToString();
                                    item.CLRName = reader["clr_name"].ToString();
                                    item.PermissionSet = reader["permission_set_desc"].ToString();
                                    item.Content = ToHex((byte[])reader["content"]);
                                    item.Visible = (bool)reader["is_visible"];
                                    lastViewId = item.Id;
                                    database.Assemblies.Add(item);
                                }
                                if (((int)reader["FileId"]) != 1)
                                    item.Files.Add(new AssemblyFile(reader["FileName"].ToString(), ToHex((byte[])reader["content"])));
                                item.Dependencys.Add(new ObjectDependency(reader["UDTName"].ToString(), ""));
                            }
                        }
                    }
                }
            }
        }
    }
}
