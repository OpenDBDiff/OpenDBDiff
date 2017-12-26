using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DBDiff.Schema.Errors;
using DBDiff.Schema.Events;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Generates.SQLCommands;
using DBDiff.Schema.SQLServer.Generates.Generates.Util;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateUserDataTypes
    {
        private readonly Generate root;

        public GenerateUserDataTypes(Generate root)
        {
            this.root = root;
        }

        private static string GetSQLColumnsDependencies()
        {
            return SQLQueries.SQLQueryFactory.Get("DBDiff.Schema.SQLServer.Generates.SQLQueries.GetSQLColumnsDependencies");
        }

        private static void FillColumnsDependencies(SchemaList<UserDataType, Database> types, string connectionString)
        {
            if (types == null) throw new ArgumentNullException("types");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(GetSQLColumnsDependencies(), conn))
                {
                    conn.Open();
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            types[reader["TypeName"].ToString()].Dependencys.Add(new ObjectDependency(reader["TableName"].ToString(), reader["ColumnName"].ToString(), ConvertType.GetObjectType(reader["Type"].ToString())));
                        }
                    }
                }
            }
        }

        public void Fill(Database database, string connectionString, List<MessageLog> messages)
        {
            //not supported in azure yet http://msdn.microsoft.com/en-us/library/ee336233.aspx
            if (database.Info.Version == DatabaseInfo.VersionTypeEnum.SQLServerAzure10) return;

            try
            {
                if (database.Options.Ignore.FilterUserDataType)
                {
                    root.RaiseOnReading(new ProgressEventArgs("Reading UDT...", Constants.READING_UDT));
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(UserDataTypeCommand.Get(database.Info.Version), conn))
                        {
                            conn.Open();
                            command.CommandTimeout = 0;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    root.RaiseOnReadingOne(reader["Name"]);
                                    UserDataType item = new UserDataType(database);
                                    item.Id = (int)reader["tid"];
                                    item.AllowNull = (bool)reader["is_nullable"];
                                    item.Size = (short)reader["max_length"];
                                    item.Name = reader["Name"].ToString();
                                    item.Owner = reader["owner"].ToString();
                                    item.Precision = int.Parse(reader["precision"].ToString());
                                    item.Scale = int.Parse(reader["scale"].ToString());
                                    if (!String.IsNullOrEmpty(reader["defaultname"].ToString()))
                                    {
                                        item.Default.Name = reader["defaultname"].ToString();
                                        item.Default.Owner = reader["defaultowner"].ToString();
                                    }
                                    if (!String.IsNullOrEmpty(reader["rulename"].ToString()))
                                    {
                                        item.Rule.Name = reader["rulename"].ToString();
                                        item.Rule.Owner = reader["ruleowner"].ToString();
                                    }
                                    item.Type = reader["basetypename"].ToString();
                                    item.IsAssembly = (bool)reader["is_assembly_type"];
                                    item.AssemblyId = (int)reader["assembly_id"];
                                    item.AssemblyName = reader["assembly_name"].ToString();
                                    item.AssemblyClass = reader["assembly_class"].ToString();
                                    database.UserTypes.Add(item);
                                }
                            }
                        }
                    }
                    if (database.Options.Ignore.FilterTable)
                        FillColumnsDependencies(database.UserTypes, connectionString);
                }
            }
            catch (Exception ex)
            {
                messages.Add(new MessageLog(ex.Message, ex.StackTrace, MessageLog.LogType.Error));
            }
        }
    }
}
