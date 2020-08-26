using System;
using System.Data.SqlClient;
using System.Text;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Generates
{
    public class GeneratePartitionFunctions
    {
        private Generate root;

        public GeneratePartitionFunctions(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            return SQLQueries.SQLQueryFactory.Get("GetPartitionFunctions");
        }

        private static string ToHex(byte[] stream)
        {
            StringBuilder sHex = new StringBuilder(2 * stream.Length);
            for (int i = 0; i < stream.Length; i++)
                sHex.AppendFormat("{0:X2} ", stream[i]);
            return "0x" + sHex.ToString().Replace(" ", String.Empty);
        }

        public void Fill(Database database, string connectioString)
        {
            int lastObjectId = 0;
            PartitionFunction item = null;
            if (database.Options.Ignore.FilterPartitionFunction)
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
                                if (lastObjectId != (int)reader["function_id"])
                                {
                                    lastObjectId = (int)reader["function_id"];
                                    item = new PartitionFunction(database);
                                    item.Id = (int)reader["function_id"];
                                    item.Name = reader["name"].ToString();
                                    item.IsBoundaryRight = (bool)reader["IsRight"];
                                    item.Precision = (byte)reader["precision"];
                                    item.Scale = (byte)reader["scale"];
                                    item.Size = (short)reader["max_length"];
                                    item.Type = reader["TypeName"].ToString();
                                    database.PartitionFunctions.Add(item);
                                }

                                switch (item.Type) {
                                    case "binary":
                                    case "varbinary":
                                        item.Values.Add(ToHex((byte[])reader["value"]));
                                        break;
                                    case "date":
                                        item.Values.Add(String.Format("'{0:yyyy/MM/dd}'", (DateTime)reader["value"]));
                                        break;
                                    case "smalldatetime":
                                    case "datetime":
                                        item.Values.Add(String.Format("'{0:yyyy/MM/dd HH:mm:ss.fff}'", (DateTime)reader["value"]));
                                        break;
                                    default:
                                        item.Values.Add(reader["value"].ToString());
                                        break;
                                }
                                    
                            }
                        }
                    }
                }
            }
        }
    }
}
