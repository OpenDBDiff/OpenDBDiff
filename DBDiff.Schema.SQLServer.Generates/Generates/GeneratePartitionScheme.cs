using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GeneratePartitionScheme
    {
        private Generate root;

        public GeneratePartitionScheme(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            return SQLQueries.SQLQueryFactory.Get("DBDiff.Schema.SQLServer.Generates.SQLQueries.GetPartitionSchemes");
        }

        public void Fill(Database database, string connectioString)
        {
            int lastObjectId = 0;
            PartitionScheme item = null;
            if (database.Options.Ignore.FilterPartitionScheme)
            {
                using (SqlConnection conn = new SqlConnection(connectioString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        conn.Open();
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (lastObjectId != (int)reader["ID"])
                                {
                                    lastObjectId = (int)reader["ID"];
                                    item = new PartitionScheme(database);
                                    item.Id = (int)reader["ID"];
                                    item.Name = reader["name"].ToString();
                                    item.PartitionFunction = reader["FunctionName"].ToString();
                                    database.PartitionSchemes.Add(item);
                                }
                                item.FileGroups.Add(reader["FileGroupName"].ToString());
                            }
                        }
                    }
                }
            }
        }
    }
}
