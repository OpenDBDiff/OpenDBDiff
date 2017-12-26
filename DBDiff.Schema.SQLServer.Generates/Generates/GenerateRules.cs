using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateRules
    {
        private Generate root;

        public GenerateRules(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            return SQLQueries.SQLQueryFactory.Get("DBDiff.Schema.SQLServer.Generates.SQLQueries.GetRules");
        }

        public void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterRules)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Rule item = new Rule(database);
                                item.Id = (int)reader["object_id"];
                                item.Name = reader["Name"].ToString();
                                item.Owner = reader["Owner"].ToString();
                                item.Text = reader["Definition"].ToString();
                                database.Rules.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
