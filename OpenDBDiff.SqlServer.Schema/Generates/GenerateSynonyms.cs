using Microsoft.Data.SqlClient;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Generates
{
    public class GenerateSynonyms
    {
        private Generate root;

        public GenerateSynonyms(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            return SQLQueries.SQLQueryFactory.Get("GetSynonyms");
        }

        public void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterSynonyms)
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
                                Synonym item = new Synonym(database);
                                item.Id = (int)reader["object_id"];
                                item.Name = reader["Name"].ToString();
                                item.Owner = reader["Owner"].ToString();
                                item.Value = reader["base_object_name"].ToString();
                                database.Synonyms.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
