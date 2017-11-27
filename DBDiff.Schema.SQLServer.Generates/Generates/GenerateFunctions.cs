using System;
using System.Data.SqlClient;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Generates.Generates.SQLCommands;
using DBDiff.Schema.SQLServer.Generates.Generates.Util;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateFunctions
    {
        private Generate root;

        public GenerateFunctions(Generate root)
        {
            this.root = root;
        }

        private static string GetSQLParameters()
        {
            return SQLQueries.SQLQueryFactory.Get("DBDiff.Schema.SQLServer.Generates.SQLQueries.GetParameters");
        }

        private static void FillParameters(Database database, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(GetSQLParameters(), conn))
                {
                    conn.Open();
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Parameter param = new Parameter();
                            param.Name = reader["Name"].ToString();
                            param.Type = reader["TypeName"].ToString();
                            param.Size = (short)reader["max_length"];
                            param.Scale = (byte)reader["scale"];
                            param.Precision = (byte)reader["precision"];
                            param.Output = (bool)reader["is_output"];
                            if (param.Type.Equals("nchar") || param.Type.Equals("nvarchar"))
                            {
                                if (param.Size != -1)
                                    param.Size = param.Size / 2;
                            }
                            database.CLRFunctions[reader["ObjectName"].ToString()].Parameters.Add(param);
                        }
                    }
                }
            }
        }

        public void Fill(Database database, string connectionString)
        {
            int lastViewId = 0;
            if ((database.Options.Ignore.FilterFunction) || (database.Options.Ignore.FilterCLRFunction))
            {
                root.RaiseOnReading(new ProgressEventArgs("Reading functions...", Constants.READING_FUNCTIONS));
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(FunctionSQLCommand.Get(database.Info.Version), conn))
                    {
                        conn.Open();
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Function itemF = null;
                            CLRFunction itemC = null;
                            while (reader.Read())
                            {
                                root.RaiseOnReadingOne(reader["name"]);
                                if ((!reader["type"].ToString().Trim().Equals("FS")) && (database.Options.Ignore.FilterFunction))
                                {
                                    if (lastViewId != (int)reader["object_id"])
                                    {
                                        itemF = new Function(database);
                                        itemF.Id = (int)reader["object_id"];
                                        itemF.Name = reader["name"].ToString();
                                        itemF.Owner = reader["owner"].ToString();
                                        itemF.IsSchemaBinding = reader["IsSchemaBound"].ToString().Equals("1");
                                        database.Functions.Add(itemF);
                                        lastViewId = itemF.Id;
                                    }
                                    if (itemF.IsSchemaBinding)
                                    {
                                        if (!reader.IsDBNull(reader.GetOrdinal("referenced_major_id")))
                                            database.Dependencies.Add(database, (int)reader["referenced_major_id"], itemF);
                                        if (!String.IsNullOrEmpty(reader["TableName"].ToString()))
                                            itemF.DependenciesIn.Add(reader["TableName"].ToString());
                                        if (!String.IsNullOrEmpty(reader["DependOut"].ToString()))
                                            itemF.DependenciesOut.Add(reader["DependOut"].ToString());
                                    }
                                }
                                if ((reader["type"].ToString().Trim().Equals("FS")) && (database.Options.Ignore.FilterCLRFunction))
                                {
                                    itemC = new CLRFunction(database);
                                    if (lastViewId != (int)reader["object_id"])
                                    {
                                        itemC.Id = (int)reader["object_id"];
                                        itemC.Name = reader["name"].ToString();
                                        itemC.Owner = reader["owner"].ToString();
                                        itemC.IsAssembly = true;
                                        itemC.AssemblyId = (int)reader["assembly_id"];
                                        itemC.AssemblyName = reader["assembly_name"].ToString();
                                        itemC.AssemblyClass = reader["assembly_class"].ToString();
                                        itemC.AssemblyExecuteAs = reader["ExecuteAs"].ToString();
                                        itemC.AssemblyMethod = reader["assembly_method"].ToString();
                                        itemC.ReturnType.Type = reader["ReturnType"].ToString();
                                        itemC.ReturnType.Size = (short)reader["max_length"];
                                        itemC.ReturnType.Scale = (byte)reader["Scale"];
                                        itemC.ReturnType.Precision = (byte)reader["precision"];
                                        if (itemC.ReturnType.Type.Equals("nchar") || itemC.ReturnType.Type.Equals("nvarchar"))
                                        {
                                            if (itemC.ReturnType.Size != -1)
                                                itemC.ReturnType.Size = itemC.ReturnType.Size / 2;
                                        }
                                        database.CLRFunctions.Add(itemC);
                                        lastViewId = itemC.Id;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (database.CLRFunctions.Count > 0)
                FillParameters(database, connectionString);
        }
    }
}
