using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Options;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Generates
{
    public static class GenerateDDLTriggers
    {
        private static string GetSQL()
        {
            string sql = "";
            sql += "SELECT OBJECT_DEFINITION(T.object_id) AS Text,T.name,is_disabled,is_not_for_replication,is_instead_of_trigger ";
            sql += "FROM sys.triggers T ";
            sql += "WHERE T.parent_id = 0 AND T.parent_class = 0";
            return sql;
        }

        public static void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterDDLTriggers)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Trigger trigger = new Trigger(database);
                                trigger.Text = reader["Text"].ToString();
                                trigger.Name = reader["Name"].ToString();
                                trigger.InsteadOf = (bool)reader["is_instead_of_trigger"];
                                trigger.IsDisabled = (bool)reader["is_disabled"];
                                trigger.IsDDLTrigger = true;
                                trigger.NotForReplication = (bool)reader["is_not_for_replication"];
                                trigger.Owner = "";
                                database.DDLTriggers.Add(trigger);
                            }
                        }
                    }
                }
            }
        }
    }
}
