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
            string sql;
            sql = "select P.data_space_id AS ID, DS.Name AS FileGroupName,P.Name, F.name AS FunctionName ";
            sql += "from sys.partition_schemes P ";
            sql += "INNER JOIN sys.partition_functions F ON F.function_id = P.function_id ";
            sql += "INNER JOIN sys.destination_data_spaces DF ON DF.partition_scheme_id = P.data_space_id ";
            sql += "INNER JOIN sys.data_spaces DS ON DS.data_space_id = DF.data_space_id ";
            sql += "ORDER BY P.data_space_id, DF.destination_id ";

            return sql;
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
