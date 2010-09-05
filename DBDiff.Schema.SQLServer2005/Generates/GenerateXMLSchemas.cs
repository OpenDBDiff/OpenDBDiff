using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Options;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.SQLServer.Generates.Util;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates
{
    public static class GenerateXMLSchemas
    {
        private static string GetSQLColumnsDependencis()
        {
            string sql;
            sql = "SELECT O.Type, '[' + S1.Name + '].[' + XS.Name +']' AS XMLName, '[' + S.Name + '].[' + O.Name +']' AS TableName, '[' + S.Name + '].[' + O.Name + '].[' + C.Name + ']' AS ColumnName from sys.columns C ";
            sql += "INNER JOIN sys.xml_schema_collections XS ON XS.xml_collection_id = C.xml_collection_id ";
            sql += "INNER JOIN sys.objects O ON O.object_id = C.object_id ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = O.schema_id ";
            sql += "INNER JOIN sys.schemas S1 ON S1.schema_id = XS.schema_id ";
            sql += "ORDER BY XS.xml_collection_id";
            return sql;
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

        private static void FillColumnsDependencies(SchemaList<XMLSchema, Database> items, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(GetSQLColumnsDependencis(), conn))
                {
                    conn.Open();
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items[reader["XMLName"].ToString()].Dependencys.Add(new ObjectDependency(reader["TableName"].ToString(), reader["ColumnName"].ToString(), ConvertType.GetObjectType(reader["Type"].ToString())));
                        }
                    }
                }
            }
        }

        public static void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterXMLSchema)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQLXMLSchema(), conn))
                    {
                        conn.Open();
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                XMLSchema item = new XMLSchema(database);
                                item.Id = (int)reader["ID"];
                                item.Name = reader["name"].ToString();
                                item.Owner = reader["owner"].ToString();
                                item.Text = reader["Text"].ToString();
                                database.XmlSchemas.Add(item);
                            }
                        }
                    }
                }
                if (database.Options.Ignore.FilterTable)
                    FillColumnsDependencies(database.XmlSchemas, connectionString);
            }
        }
    }
}
