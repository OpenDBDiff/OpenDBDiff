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
            return SQLQueries.SQLQueryFactory.Get("DBDiff.Schema.SQLServer.Generates.SQLQueries.GetAssemblyFiles");
        }

        private static string GetSQL()
        {
            return SQLQueries.SQLQueryFactory.Get("DBDiff.Schema.SQLServer.Generates.SQLQueries.GetAssemblies");
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
                                    AssemblyFile file = new AssemblyFile(assem, reader["FileName"].ToString(), ToHex((byte[])reader["FileContent"]));
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
                                        Id = (int)reader["assembly_id"],
                                        Name = reader["Name"].ToString(),
                                        Owner = reader["Owner"].ToString(),
                                        CLRName = reader["clr_name"].ToString(),
                                        PermissionSet = reader["permission_set_desc"].ToString(),
                                        Text = ToHex((byte[])reader["content"]),
                                        Visible = (bool)reader["is_visible"]
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
