using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateDDLTriggers
    {
        private Generate root;

        public GenerateDDLTriggers(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            return SQLQueries.SQLQueryFactory.Get("DBDiff.Schema.SQLServer.Generates.SQLQueries.GetDDLTriggers");
        }

        public void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterDDLTriggers)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        command.CommandTimeout = 0;
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
