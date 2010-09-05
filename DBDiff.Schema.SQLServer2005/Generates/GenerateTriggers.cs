using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Options;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.Errors;

namespace DBDiff.Schema.SQLServer.Generates
{
    public static class GenerateTriggers
    {
        private static string GetSQL()
        {
            string sql = "";
            sql += "SELECT ISNULL(CONVERT(varchar,AM.execute_as_principal_id),'CALLER') as ExecuteAs, AF.name AS assembly_name, AM.assembly_class, AM.assembly_id, AM.assembly_method, T.type, CAST(ISNULL(tei.object_id,0) AS bit) AS IsInsert, CAST(ISNULL(teu.object_id,0) AS bit) AS IsUpdate, CAST(ISNULL(ted.object_id,0) AS bit) AS IsDelete, T.parent_id, OBJECT_DEFINITION(t.object_id) AS Text, S.name AS Owner,T.name,is_disabled,is_not_for_replication,is_instead_of_trigger ";
            sql += "FROM sys.triggers T ";
            sql += "INNER JOIN sys.objects O ON O.object_id = T.parent_id ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = O.schema_id ";
            sql += "LEFT JOIN sys.trigger_events AS tei ON tei.object_id = t.object_id and tei.type=1 ";
            sql += "LEFT JOIN sys.trigger_events AS teu ON teu.object_id = t.object_id and teu.type=2 ";
            sql += "LEFT JOIN sys.trigger_events AS ted ON ted.object_id = t.object_id and ted.type=3 ";
            sql += "LEFT JOIN sys.assembly_modules AM ON AM.object_id = T.object_id ";
            sql += "LEFT JOIN sys.assemblies AF ON AF.assembly_id = AM.assembly_id ";
            sql += "ORDER BY T.parent_id ";

            return sql;
        }

        public static void Fill(Database database, string connectionString, List<MessageLog> messages)
        {
            int parentId = 0;
            Table table = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        conn.Open();
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (parentId != (int)reader["parent_id"])
                                {
                                    parentId = (int)reader["parent_id"];
                                    table = database.Tables.Find(parentId);
                                }
                                if (reader["type"].Equals("TR"))
                                {
                                    Trigger trigger = new Trigger(table);
                                    trigger.Name = reader["Name"].ToString();
                                    trigger.InsteadOf = (bool)reader["is_instead_of_trigger"];
                                    trigger.IsDisabled = (bool)reader["is_disabled"];
                                    trigger.IsDDLTrigger = false;
                                    trigger.Owner = reader["Owner"].ToString();
                                    trigger.Text = reader["Text"].ToString();
                                    if (!database.Options.Ignore.FilterIgnoreNotForReplication)
                                        trigger.NotForReplication = (bool)reader["is_not_for_replication"];
                                    table.Triggers.Add(trigger);
                                }
                                else
                                {
                                    CLRTrigger trigger = new CLRTrigger(table);
                                    trigger.Name = reader["Name"].ToString();
                                    trigger.IsDelete = (bool)reader["IsDelete"];
                                    trigger.IsUpdate = (bool)reader["IsUpdate"];
                                    trigger.IsInsert = (bool)reader["IsInsert"];
                                    trigger.Owner = reader["Owner"].ToString();
                                    trigger.IsAssembly = true;
                                    trigger.AssemblyId = (int)reader["assembly_id"];
                                    trigger.AssemblyName = reader["assembly_name"].ToString();
                                    trigger.AssemblyClass = reader["assembly_class"].ToString();
                                    trigger.AssemblyExecuteAs = reader["ExecuteAs"].ToString();
                                    trigger.AssemblyMethod = reader["assembly_method"].ToString();
                                    table.CLRTriggers.Add(trigger);
                                    /*if (!database.Options.Ignore.FilterIgnoreNotForReplication)
                                        trigger.NotForReplication = (bool)reader["is_not_for_replication"];*/

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
