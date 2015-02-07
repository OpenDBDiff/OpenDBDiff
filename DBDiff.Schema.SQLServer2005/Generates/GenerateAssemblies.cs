using System;
using System.Data.SqlClient;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateAssemblies
    {
        private Generate root;

        public GenerateAssemblies(Generate root)
        {
            this.root = root;
        }

        private static string GetSQLFiles()
        {
            string sql = "select '[' + A.Name + ']' AS Name, AF.content AS FileContent, AF.File_Id AS FileId, AF.Name AS FileName ";
            sql += "FROM sys.assemblies A ";
            sql += "INNER JOIN sys.assembly_files AF ON AF.assembly_id = A.assembly_id ";
            sql += "ORDER BY A.Name ";
            return sql;
        }

        private static string GetSQL()
        {
            string sql = "select DISTINCT '[' + S2.Name + '].[' + AT.Name + ']' as UDTName, ";
            sql += "ISNULL('[' + A2.name + ']','') AS Dependency, ";
            sql += "ISNULL('[' + S3.Name + '].[' + A3.name + ']','') AS ObjectDependency, ";
            sql += "AF.assembly_id, A.clr_name,A.name,S.name AS Owner, A.permission_set_desc, A.is_visible, content ";
            sql += "FROM sys.assemblies A ";
            sql += "INNER JOIN sys.assembly_files AF ON AF.assembly_id = A.assembly_id ";
            sql += "LEFT JOIN sys.assembly_references AR ON A.assembly_id = AR.referenced_assembly_id ";
            sql += "LEFT JOIN sys.assemblies A2 ON A2.assembly_id = AR.assembly_id ";
            sql += "LEFT JOIN sys.schemas S1 ON S1.schema_id = A2.principal_id ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = A.principal_id ";
            sql += "LEFT JOIN sys.assembly_types AT ON AT.assembly_id = A.assembly_id ";
            sql += "LEFT JOIN sys.schemas S2 ON S2.schema_id = AT.schema_id ";
            sql += "LEFT JOIN sys.assembly_modules AM ON AM.assembly_id = A.assembly_id ";
            sql += "LEFT JOIN sys.objects A3 ON A3.object_id = AM.object_id ";
            sql += "LEFT JOIN sys.schemas S3 ON S3.schema_id = A3.schema_id ";
            sql += "ORDER BY A.name";
            return sql;
        }

        private static string ToHex(byte[] stream)
        {
            StringBuilder sHex = new StringBuilder(2 * stream.Length);
            for (int i = 0; i < stream.Length; i++)
                sHex.AppendFormat("{0:X2} ", stream[i]);
            return "0x" + sHex.ToString().Replace(" ", String.Empty);
        }

        private static void FillFiles(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterAssemblies)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQLFiles(), conn))
                    {
                        conn.Open();
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (((int)reader["FileId"]) != 1)
                                {
                                    Assembly assem = database.Assemblies[reader["Name"].ToString()];
                                    AssemblyFile file = new AssemblyFile(assem,reader["FileName"].ToString(), ToHex((byte[])reader["FileContent"]));
                                    assem.Files.Add(file);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void Fill(Database database, string connectionString)
        {
            int lastViewId = 0;
            if (database.Options.Ignore.FilterAssemblies)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        conn.Open();
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Assembly item = null;
                            while (reader.Read())
                            {                                
                                if (lastViewId != (int)reader["assembly_id"])
                                {
                                    item = new Assembly(database)
                                               {
                                                   Id = (int) reader["assembly_id"],
                                                   Name = reader["Name"].ToString(),
                                                   Owner = reader["Owner"].ToString(),
                                                   CLRName = reader["clr_name"].ToString(),
                                                   PermissionSet = reader["permission_set_desc"].ToString(),
                                                   Text = ToHex((byte[]) reader["content"]),
                                                   Visible = (bool) reader["is_visible"]
                                               };
                                    lastViewId = item.Id;
                                    database.Assemblies.Add(item);
                                }
                                if (!String.IsNullOrEmpty(reader["Dependency"].ToString()))
                                    item.DependenciesOut.Add(reader["Dependency"].ToString());
                                if (!String.IsNullOrEmpty(reader["ObjectDependency"].ToString()))
                                    item.DependenciesOut.Add(reader["ObjectDependency"].ToString());
                                if (!String.IsNullOrEmpty(reader["UDTName"].ToString()))
                                    item.DependenciesOut.Add(reader["UDTName"].ToString());
                            }
                        }
                    }
                    FillFiles(database, connectionString);
                }
            }
        }
    }
}
