using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Options;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Generates
{
    public class GenerateXMLSchemas
    {
        private string connectioString;
        private SqlOption objectFilter;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateXMLSchemas(string connectioString, SqlOption filter)
        {
            this.connectioString = connectioString;
            this.objectFilter = filter;
        }

        private static string GetSQLXMLSchema()
        {
            string sql;
            sql = "SELECT  ";
            sql += "xsc.name, ";
            sql += "xsc.xml_collection_id AS [ID], ";
            sql += "sch.name AS Owner, ";
            sql += "XML_SCHEMA_NAMESPACE(sch.Name, xsc.name) AS Text ";
            sql += "FROM sys.xml_schema_collections AS xsc ";
            sql += "INNER JOIN sys.schemas AS sch ON xsc.schema_id = sch.schema_id ";
            sql += "WHERE xsc.schema_id <> 4";
            return sql;
        }

        public XMLSchemas Get(Database database)
        {
            XMLSchemas types = new XMLSchemas(database);
            if (objectFilter.OptionFilter.FilterXMLSchema)
            {
                using (SqlConnection conn = new SqlConnection(connectioString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQLXMLSchema(), conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                XMLSchema schema = new XMLSchema(database);
                                schema.Id = (int)reader["ID"];
                                schema.Name = reader["name"].ToString();
                                schema.Owner = reader["owner"].ToString();
                                schema.Text = reader["Text"].ToString();
                                types.Add(schema);
                            }
                        }
                    }
                }
            }
            return types;
        }
    }
}
