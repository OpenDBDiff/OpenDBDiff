using OpenDBDiff.Abstractions.Schema.Errors;
using OpenDBDiff.Abstractions.Schema.Events;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Generates.Util;
using OpenDBDiff.SqlServer.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace OpenDBDiff.SqlServer.Schema.Generates
{
    public class GenerateTriggers
    {
        private Generate root;

        public GenerateTriggers(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL(DatabaseInfo.SQLServerVersion version, SqlOption options)
        {
            if (version == DatabaseInfo.SQLServerVersion.SQLServerAzure10)
            {
                return SQLQueries.SQLQueryFactory.Get("GetTriggers", version);
            }
            else
            {
                return SQLQueries.SQLQueryFactory.Get("GetTriggers");
            }
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
                                    root.RaiseOnReadingOne(reader["Name"]);
                                    type = reader["ObjectType"].ToString().Trim();
                                    if (parentId != (int)reader["parent_id"])
                                    {
                                        parentId = (int)reader["parent_id"];
                                        if (type.Equals("V"))
                                            parent = database.Views.Find(parentId);
                                        else
                                            parent = database.Tables.Find(parentId);
                                    }
                                    if (parent == null) { continue; }
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
