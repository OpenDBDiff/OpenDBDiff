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
    public class GenerateDDLTriggers
    {
        private string connectioString;
        private SqlOption objectFilter;
        //public event Progress.ProgressHandler OnTableProgress;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateDDLTriggers(string connectioString, SqlOption filter)
        {
            this.connectioString = connectioString;
            this.objectFilter = filter;
        }

        private static string GetSQLDDLTrigger()
        {
            string sql = "";
            sql += "SELECT OBJECT_DEFINITION(t.object_id) AS Text,T.name,is_disabled,is_not_for_replication,is_instead_of_trigger ";
            sql += "FROM sys.triggers T ";
            sql += "WHERE T.parent_id = 0 AND T.parent_class = 0";
            return sql;
        }

        public Triggers Get(Database database)
        {
            if (database != null)
            {
                Triggers triggers = new Triggers(database);
                if (objectFilter.OptionFilter.FilterDDLTriggers)
                {
                    using (SqlConnection conn = new SqlConnection(connectioString))
                    {
                        conn.Open();
                        using (SqlCommand command = new SqlCommand(GetSQLDDLTrigger(), conn))
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
                                    triggers.Add(trigger);
                                }
                            }
                        }
                    }
                }
                return triggers;
            }
            else
                throw new ArgumentNullException("database");
        }
    }
}
