using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Generates.Options;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Errors;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Generates.Util;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateTriggers
    {
        private Generate root;

        public GenerateTriggers(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL(DatabaseInfo.VersionTypeEnum version, SqlOption options)
        {
            
            string sql = "";
            sql += "SELECT T.object_id, O.type AS ObjectType, ISNULL(CONVERT(varchar,AM.execute_as_principal_id),'CALLER') as ExecuteAs, AF.name AS assembly_name, AM.assembly_class, AM.assembly_id, AM.assembly_method, T.type, CAST(ISNULL(tei.object_id,0) AS bit) AS IsInsert, CAST(ISNULL(teu.object_id,0) AS bit) AS IsUpdate, CAST(ISNULL(ted.object_id,0) AS bit) AS IsDelete, T.parent_id, S.name AS Owner,T.name,is_disabled,is_not_for_replication,is_instead_of_trigger ";
            sql += "FROM sys.triggers T ";
            sql += "INNER JOIN sys.objects O ON O.object_id = T.parent_id ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = O.schema_id ";
            sql += "LEFT JOIN sys.trigger_events AS tei ON tei.object_id = T.object_id and tei.type=1 ";
            sql += "LEFT JOIN sys.trigger_events AS teu ON teu.object_id = T.object_id and teu.type=2 ";
            sql += "LEFT JOIN sys.trigger_events AS ted ON ted.object_id = T.object_id and ted.type=3 ";
            if (version == DatabaseInfo.VersionTypeEnum.SQLServerDenali)
            {
                sql += ",(SELECT null as execute_as_principal_id, null as assembly_class, null as assembly_id, null as assembly_method) AS AM,";
                sql += "(SELECT null AS name) AS AF";
            }
            else
            {
                sql += "LEFT JOIN sys.assembly_modules AM ON AM.object_id = T.object_id ";
                sql += "LEFT JOIN sys.assemblies AF ON AF.assembly_id = AM.assembly_id";
            } 
            sql += " ORDER BY T.parent_id";

            return sql;
        }

        public void Fill(Database database, string connectionString, List<MessageLog> messages)
        {
            int parentId = 0;
            ISchemaBase parent = null;
            string type;
            try
            {
                if (database.Options.Ignore.FilterTrigger)
                {
                    root.RaiseOnReading(new ProgressEventArgs("Reading Triggers...", Constants.READING_TRIGGERS));
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(GetSQL(database.Info.Version, database.Options), conn))
                        {
                            conn.Open();
                            command.CommandTimeout = 0;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    type = reader["ObjectType"].ToString().Trim();
                                    if (parentId != (int)reader["parent_id"])
                                    {
                                        parentId = (int)reader["parent_id"];
                                        if (type.Equals("V"))
                                            parent = database.Views.Find(parentId);
                                        else
                                            parent = database.Tables.Find(parentId);
                                    }
                                    if (reader["type"].Equals("TR"))
                                    {
                                        Trigger item = new Trigger(parent);
                                        item.Id = (int)reader["object_id"];
                                        item.Name = reader["Name"].ToString();
                                        item.InsteadOf = (bool)reader["is_instead_of_trigger"];
                                        item.IsDisabled = (bool)reader["is_disabled"];
                                        item.IsDDLTrigger = false;
                                        item.Owner = reader["Owner"].ToString();
                                        if (database.Options.Ignore.FilterNotForReplication)
                                            item.NotForReplication = (bool)reader["is_not_for_replication"];
                                        if (type.Equals("V"))
                                            ((View)parent).Triggers.Add(item);
                                        else
                                            ((Table)parent).Triggers.Add(item);
                                    }
                                    else
                                    {
                                        CLRTrigger item = new CLRTrigger(parent);
                                        item.Id = (int)reader["object_id"];
                                        item.Name = reader["Name"].ToString();
                                        item.IsDelete = (bool)reader["IsDelete"];
                                        item.IsUpdate = (bool)reader["IsUpdate"];
                                        item.IsInsert = (bool)reader["IsInsert"];
                                        item.Owner = reader["Owner"].ToString();
                                        item.IsAssembly = true;
                                        item.AssemblyId = (int)reader["assembly_id"];
                                        item.AssemblyName = reader["assembly_name"].ToString();
                                        item.AssemblyClass = reader["assembly_class"].ToString();
                                        item.AssemblyExecuteAs = reader["ExecuteAs"].ToString();
                                        item.AssemblyMethod = reader["assembly_method"].ToString();
                                        if (type.Equals("V"))
                                            ((View)parent).CLRTriggers.Add(item);
                                        else
                                            ((Table)parent).CLRTriggers.Add(item);
                                        /*if (!database.Options.Ignore.FilterIgnoreNotForReplication)
                                            trigger.NotForReplication = (bool)reader["is_not_for_replication"];*/
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                messages.Add(new MessageLog(ex.Message, ex.StackTrace, MessageLog.LogType.Error));
            }
        }
    }
}
