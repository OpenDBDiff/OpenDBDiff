using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.SQLServer.Options;

namespace DBDiff.Schema.SQLServer.Generates
{
    public static class GenerateViews
    {
        private static string GetSQL()
        {
            string sql = "";
            sql += "select distinct ISNULL('[' + S3.Name + '].[' + object_name(D2.object_id) + ']','') AS DependOut, '[' + S2.Name + '].[' + object_name(D.referenced_major_id) + ']' AS TableName, D.referenced_major_id, OBJECT_DEFINITION(P.object_id) AS Text, OBJECTPROPERTY (P.object_id,'IsSchemaBound') AS IsSchemaBound, P.object_id, S.name as owner, P.name as name from sys.views P ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = P.schema_id ";
            sql += "LEFT JOIN sys.sql_dependencies D ON P.object_id = D.object_id ";
            sql += "LEFT JOIN sys.objects O ON O.object_id = D.referenced_major_id ";
            sql += "LEFT JOIN sys.schemas S2 ON S2.schema_id = O.schema_id ";
            sql += "LEFT JOIN sys.sql_dependencies D2 ON P.object_id = D2.referenced_major_id ";
            sql += "LEFT JOIN sys.objects O2 ON O2.object_id = D2.object_id ";
            sql += "LEFT JOIN sys.schemas S3 ON S3.schema_id = O2.schema_id ";
            sql += "ORDER BY P.object_id";
            return sql;
        }

        public static void Fill(Database database, string connectionString)
        {
            Indexes indexes = null;
            if (database.Options.Ignore.FilterView)
            {
                FillView(database,connectionString);                
                if ((database.Views.Count > 0) && (database.Options.Ignore.FilterIndex))
                    indexes = (new GenerateIndex(connectionString, database.Options)).Get("V");

                Merge(database.Views, null, indexes);
            }
        }

        private static void FillView(Database database, string connectionString)
        {
            int lastViewId = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        View item = null;
                        while (reader.Read())
                        {
                            if (lastViewId != (int)reader["object_id"])
                            {
                                item = new View(database);
                                item.Id = (int)reader["object_id"];
                                item.Name = reader["name"].ToString();
                                item.Owner = reader["owner"].ToString();
                                item.Text = reader["text"].ToString();
                                item.IsSchemaBinding = reader["IsSchemaBound"].ToString().Equals("1");
                                database.Views.Add(item);
                                lastViewId = item.Id;
                            }
                            if (item.IsSchemaBinding)
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("referenced_major_id")))
                                    database.Dependencies.Add((int)reader["referenced_major_id"], item);
                                if (!String.IsNullOrEmpty(reader["TableName"].ToString()))
                                    item.DependenciesIn.Add(reader["TableName"].ToString());
                                if (!String.IsNullOrEmpty(reader["DependOut"].ToString()))
                                    item.DependenciesOut.Add(reader["DependOut"].ToString());
                            }
                        }
                    }
                }                    
            }
        }

        private static void Merge(Views views, Triggers triggers, Indexes indexes)
        {
            /*if (triggers != null)
            {
                foreach (Trigger trigger in triggers)
                {
                    Table table = tables.Find(trigger.Parent.Id);
                    trigger.Parent = table;
                    table.Triggers.Add(trigger);
                }
            }*/
            if (indexes != null)
            {
                foreach (Index index in indexes)
                {
                    View parent = views.Find(index.Parent.Id);
                    index.Parent = parent;
                    parent.Indexes.Add(index);
                    foreach (IndexColumn icolumn in index.Columns)
                    {
                        ((Database)parent.Parent).Dependencies.Add(parent.Id, icolumn.Id, parent.Id, icolumn.DataTypeId, index);
                    }
                }
            }
        }
    }
}
