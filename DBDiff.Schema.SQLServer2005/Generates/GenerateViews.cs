using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.SQLServer.Options;

namespace DBDiff.Schema.SQLServer.Generates
{
    public class GenerateViews
    {
        private string connectioString;
        private SqlOption objectFilter;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateViews(string connectioString, SqlOption filter)
        {
            this.connectioString = connectioString;
            this.objectFilter = filter;
        }

        private static string GetSQL()
        {
            string sql = "";
            sql += "SELECT distinct object_name(referenced_major_id) AS TableName, referenced_major_id, V.Name, OBJECTPROPERTY (V.object_id,'IsSchemaBound') AS IsSchemaBound, SCHEMA_NAME(V.schema_id) AS Owner, OBJECT_DEFINITION(V.object_id) AS Text, V.object_id ";
            sql += "FROM sys.views V ";
            sql += "LEFT JOIN sys.sql_dependencies D ON V.object_id = D.object_id ";
            sql += "ORDER BY V.object_id";
            return sql;
        }
        public Views Get(Database database)
        {
            Views views = null;
            Indexes indexes = null;

            views = GetView(database);
            
            if ((views.Count > 0) && (objectFilter.OptionFilter.FilterIndex))
                indexes = (new GenerateIndex(connectioString, objectFilter)).Get("V");


            return Merge(views, null, indexes);
        }

        public Views GetView(Database database)
        {
            Views stores = new Views(database);
            int lastViewId = 0;
            if (objectFilter.OptionFilter.FilterView)
            {
                using (SqlConnection conn = new SqlConnection(connectioString))
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
                                    stores.Add(item);
                                    lastViewId = item.Id;
                                }
                                if (item.IsSchemaBinding)
                                    database.Dependencies.Add((int)reader["referenced_major_id"], 0, (int)reader["referenced_major_id"], 0, item);
                            }
                        }
                    }                    
                }
            }
            return stores;
        }

        private Views Merge(Views views, Triggers triggers, Indexes indexes)
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
            return views;
        }
    }
}
