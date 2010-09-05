using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Options;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Generates
{
    public class GenerateUserDataTypes
    {
        private string connectioString;
        private SqlOption objectFilter;
        //public event ProgressHandler OnTableProgress;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateUserDataTypes(string connectioString, SqlOption filter)
        {
            this.connectioString = connectioString;
            this.objectFilter = filter;
        }

        private static string GetSQLColumnsDependencis()
        {
            string sql = "SELECT 0 AS IsComputed, T.user_type_id,'[' + S.Name + '].[' + TT.Name + ']' AS TableName, '[' + S.Name + '].[' + TT.Name + '].[' + C.Name + ']' AS ColumnName,'[' + S2.Name + '].[' + T.Name + ']' AS TypeName FROM sys.types T ";
            sql += "INNER JOIN sys.columns C ON C.user_type_id = T.user_type_id ";
            sql += "INNER JOIN sys.tables TT ON TT.object_id = C.object_id ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = TT.schema_id ";
            sql += "INNER JOIN sys.schemas S2 ON S2.schema_id = T.schema_id ";
            sql += "WHERE is_user_defined = 1 ";
            sql += "UNION ";
            sql += "SELECT 1 AS IsComputed, T.user_type_id, '[' + S.Name + '].[' + TT.Name + ']' AS TableName, '[' + S.Name + '].[' + TT.Name + '].[' + C2.Name + ']' AS ColumnName, '[' + S2.Name + '].[' + T.Name + ']' AS TypeName FROM sys.types T ";
            sql += "INNER JOIN sys.columns C ON C.user_type_id = T.user_type_id ";
            sql += "INNER JOIN sys.sql_dependencies DEP ON DEP.referenced_major_id = C.object_id AND DEP.referenced_minor_id = C.column_Id AND DEP.object_id = C.object_id ";
            sql += "INNER JOIN sys.columns C2 ON C2.column_id = DEP.column_id ";
            sql += "INNER JOIN sys.tables TT ON TT.object_id = C2.object_id ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = TT.schema_id ";
            sql += "INNER JOIN sys.schemas S2 ON S2.schema_id = T.schema_id ";
            sql += "WHERE is_user_defined = 1 ";
            sql += "ORDER BY IsComputed DESC,T.user_type_id";
            return sql;
        }

        private UserDataTypes GetColumnsDependencies(UserDataTypes types)
        {
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand(GetSQLColumnsDependencis(), conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserDataType type = types[reader["TypeName"].ToString()];
                            Table table = ((Database)types.Parent).Tables[reader["TableName"].ToString()];
                            Column col = table.Columns[reader["ColumnName"].ToString()].Clone(table);
                            type.ColumnsDependencies.Add(col);
                        }
                    }
                }
            }
            return types;
        }

        public UserDataTypes Get(Database database)
        {
            UserDataTypes types = new UserDataTypes(database);
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand("sp_MShelptype null, 'uddt'", conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserDataType type = new UserDataType(database);
                            type.Id = (short)reader["tid"];
                            type.AllowNull = (bool)reader["nullable"];
                            type.Size = (int)reader["length"];
                            type.Name = reader["UserDatatypeName"].ToString();
                            type.Owner = reader["owner"].ToString();
                            if (!String.IsNullOrEmpty(reader["dt_prec"].ToString()))
                                type.Precision = int.Parse(reader["dt_prec"].ToString());
                            if (!String.IsNullOrEmpty(reader["dt_scale"].ToString()))
                                type.Scale = int.Parse(reader["dt_scale"].ToString());
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
                            types.Add(type);
                        }
                    }
                }
            }
            if (objectFilter.OptionFilter.FilterTable)
                return GetColumnsDependencies(types);
            else
                return types;
        }
    }
}
