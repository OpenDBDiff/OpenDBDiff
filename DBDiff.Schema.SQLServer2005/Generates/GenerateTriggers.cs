using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Options;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Generates
{
    internal class GenerateTriggers
    {
        private string connectioString;
        private SqlOption objectFilter;

                /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateTriggers(string connectioString, SqlOption filter)
        {
            this.connectioString = connectioString;
            this.objectFilter = filter;
        }

        private static string GetSQLTrigger()
        {
            string sql = "";
            sql += "SELECT T.parent_id, OBJECT_DEFINITION(t.object_id) AS Text, S.name AS Owner,T.name,is_disabled,is_not_for_replication,is_instead_of_trigger ";
            sql += "FROM sys.triggers T ";
            sql += "INNER JOIN sys.objects O ON O.object_id = T.parent_id ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = O.schema_id ORDER BY T.parent_id";
            return sql;
        }

        public Triggers Get()
        {
            Triggers triggers = new Triggers(null);
            int parentId = 0;
            Table table = null;

            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(GetSQLTrigger(), conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (parentId != (int)reader["parent_id"])
                            {
                                parentId = (int)reader["parent_id"];
                                table = new Table(null);
                                table.Id = parentId;                                
                            }
                            Trigger trigger = new Trigger(table);
                            trigger.Name = reader["Name"].ToString();
                            trigger.InsteadOf = (bool)reader["is_instead_of_trigger"];
                            trigger.IsDisabled = (bool)reader["is_disabled"];
                            trigger.IsDDLTrigger = false;
                            trigger.NotForReplication = (bool)reader["is_not_for_replication"];
                            trigger.Owner = reader["Owner"].ToString();
                            trigger.Text = reader["Text"].ToString();
                            triggers.Add(trigger);
                        }
                    }
                }
            }
            return triggers;
        }
    }
}
