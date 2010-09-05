using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Options;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.SQLServer.Generates.Util;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates
{
    public static class GenerateUserDataTypes
    {
        private static string GetSQLColumnsDependencis()
        {
            string sql = "SELECT TT.type, 0 AS IsComputed, T.user_type_id,'[' + S.Name + '].[' + TT.Name + ']' AS TableName, '[' + S.Name + '].[' + TT.Name + '].[' + C.Name + ']' AS ColumnName,'[' + S2.Name + '].[' + T.Name + ']' AS TypeName FROM sys.types T ";
            sql += "INNER JOIN sys.columns C ON C.user_type_id = T.user_type_id ";
            sql += "INNER JOIN sys.objects TT ON TT.object_id = C.object_id ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = TT.schema_id ";
            sql += "INNER JOIN sys.schemas S2 ON S2.schema_id = T.schema_id ";
            sql += "WHERE is_user_defined = 1 ";
            sql += "UNION ";
            sql += "SELECT TT.type, 1 AS IsComputed, T.user_type_id, '[' + S.Name + '].[' + TT.Name + ']' AS TableName, '[' + S.Name + '].[' + TT.Name + '].[' + C2.Name + ']' AS ColumnName, '[' + S2.Name + '].[' + T.Name + ']' AS TypeName FROM sys.types T ";
            sql += "INNER JOIN sys.columns C ON C.user_type_id = T.user_type_id ";
            sql += "INNER JOIN sys.sql_dependencies DEP ON DEP.referenced_major_id = C.object_id AND DEP.referenced_minor_id = C.column_Id AND DEP.object_id = C.object_id ";
            sql += "INNER JOIN sys.columns C2 ON C2.column_id = DEP.column_id AND C2.object_id = DEP.object_id ";
            sql += "INNER JOIN sys.objects TT ON TT.object_id = C2.object_id ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = TT.schema_id ";
            sql += "INNER JOIN sys.schemas S2 ON S2.schema_id = T.schema_id ";
            sql += "WHERE is_user_defined = 1 ";

            sql += "UNION ";
            sql += "SELECT TT.type, 0 AS IsComputed, T.user_type_id,'[' + S.Name + '].[' + TT.Name + ']' AS TableName, '[' + S.Name + '].[' + TT.Name + '].[' + C.Name + ']' AS ColumnName,'[' + S2.Name + '].[' + T.Name + ']' AS TypeName from sys.sql_dependencies DEP ";            
            sql += "INNER JOIN sys.objects TT ON DEP.object_id = TT.object_id ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = TT.schema_id ";
            sql += "INNER JOIN sys.parameters C ON C.object_id = TT.object_id AND C.parameter_id = DEP.referenced_minor_id ";
            sql += "INNER JOIN sys.types T ON C.user_type_id = T.user_type_id ";
            sql += "INNER JOIN sys.schemas S2 ON S2.schema_id = T.schema_id ";
            sql += "WHERE is_user_defined = 1 ";

            sql += "ORDER BY IsComputed DESC,T.user_type_id";
            return sql;
        }

        private static string GetSQL()
        {
            string sql = "select ISNULL(AF.name,'') AS assembly_name, ISNULL(AT.assembly_id,0) AS assembly_id, ISNULL(assembly_class,'') AS assembly_class, T.max_length, S2.name as defaultowner, O2.name as defaultname, S1.name as ruleowner, O.name as rulename, ISNULL(T2.Name,'') AS basetypename, S.Name AS Owner, T.Name, T.is_assembly_type, T.user_type_id AS tid, T.is_nullable, T.precision, T.scale from sys.types T ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = T.schema_id ";
            sql += "LEFT JOIN sys.types T2 ON t2.user_type_id = T.system_type_id ";
            sql += "LEFT JOIN sys.objects O ON O.type = 'R' and O.object_id = T.rule_object_id ";
            sql += "LEFT JOIN sys.schemas S1 ON S1.schema_id = O.schema_id ";
            sql += "LEFT JOIN sys.objects O2 ON O2.type = 'D' and O2.object_id = T.default_object_id ";
            sql += "LEFT JOIN sys.schemas S2 ON S2.schema_id = O2.schema_id ";
            sql += "LEFT JOIN sys.assembly_types AT ON AT.user_type_id = T.user_type_id AND T.is_assembly_type = 1 ";
            sql += "LEFT JOIN sys.assemblies AF ON AF.assembly_id = AT.assembly_id ";
            sql += "WHERE T.is_user_defined = 1 ORDER BY T.Name";
            return sql;
        }

        private static void FillColumnsDependencies(SchemaList<UserDataType, Database> types, string connectionString)
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
                            types[reader["TypeName"].ToString()].Dependencys.Add(new ObjectDependency(reader["TableName"].ToString(), reader["ColumnName"].ToString(), ConvertType.GetObjectType(reader["Type"].ToString())));
                        }
                    }
                }
            }
        }

        public static void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterUserDataType)
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
                                UserDataType type = new UserDataType(database);
                                type.Id = (int)reader["tid"];
                                type.AllowNull = (bool)reader["is_nullable"];
                                type.Size = (short)reader["max_length"];
                                type.Name = reader["Name"].ToString();
                                type.Owner = reader["owner"].ToString();
                                type.Precision = int.Parse(reader["precision"].ToString());
                                type.Scale = int.Parse(reader["scale"].ToString());
                                if (!String.IsNullOrEmpty(reader["defaultname"].ToString()))
                                {
                                    type.Default.Name = reader["defaultname"].ToString();
                                    type.Default.Owner = reader["defaultowner"].ToString();
                                }
                                if (!String.IsNullOrEmpty(reader["rulename"].ToString()))
                                {
                                    type.Rule.Name = reader["rulename"].ToString();
                                    type.Rule.Owner = reader["ruleowner"].ToString();
                                }
                                type.Type = reader["basetypename"].ToString();
                                type.IsAssembly = (bool)reader["is_assembly_type"];
                                type.AssemblyId = (int)reader["assembly_id"];
                                type.AssemblyName = reader["assembly_name"].ToString();
                                type.AssemblyClass = reader["assembly_class"].ToString();

                                database.UserTypes.Add(type);
                            }
                        }
                    }
                }
                if (database.Options.Ignore.FilterTable)
                    FillColumnsDependencies(database.UserTypes, connectionString);
            }
        }
    }
}
