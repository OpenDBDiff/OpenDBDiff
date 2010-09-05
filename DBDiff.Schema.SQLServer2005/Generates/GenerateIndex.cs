using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Globalization;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.SQLServer.Options;

namespace DBDiff.Schema.SQLServer.Generates
{
    public class GenerateIndex
    {
        private string connectioString;
        private SqlOption indexFilter;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateIndex(string connectioString, SqlOption filter)
        {
            this.connectioString = connectioString;
            this.indexFilter = filter;
        }

        private static string GetSQLIndex(string type)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT OO.type AS ObjectType, IC.key_ordinal, C.user_type_id, I.object_id, dsidx.Name as FileGroup, C.column_id,C.Name AS ColumnName, I.Name, I.index_id, I.type, is_unique, ignore_dup_key, is_primary_key, is_unique_constraint, fill_factor, is_padded, is_disabled, allow_row_locks, allow_page_locks, IC.is_descending_key, IC.is_included_column, ISNULL(ST.no_recompute,0) AS NoAutomaticRecomputation ");
            sql.Append("FROM sys.indexes I ");
            sql.Append("INNER JOIN sys.objects OO ON OO.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.index_columns IC ON IC.index_id = I.index_id AND IC.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.data_spaces AS dsidx ON dsidx.data_space_id = I.data_space_id ");
            sql.Append("INNER JOIN sys.columns C ON C.column_id = IC.column_id AND IC.object_id = C.object_id ");
            sql.Append("LEFT JOIN sys.stats AS ST ON ST.stats_id = I.index_id AND ST.object_id = I.object_id ");
            sql.Append("WHERE I.type IN (1,2,3) ");
            sql.Append("AND is_unique_constraint = 0 AND is_primary_key = 0 "); //AND I.object_id = " + table.Id.ToString(CultureInfo.InvariantCulture) + " ");
            sql.Append("AND objectproperty(I.object_id, 'IsMSShipped') <> 1 ");
            sql.Append("AND OO.Type = '" + type + "' ");
            sql.Append("ORDER BY I.object_id, I.Name");
            return sql.ToString();
        }

        public Indexes Get(string type)
        {
            Indexes cons = new Indexes(null);
            string last = "";
            int parentId = 0;
            ISchemaBase parent = null;

            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand(GetSQLIndex(type), conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Index con = null;
                        while (reader.Read())
                        {
                            if (parentId != (int)reader["object_id"])
                            {
                                parentId = (int)reader["object_id"];
                                if (reader["object_id"].ToString().Equals("V"))
                                    parent = new View(null);
                                else
                                    parent = new Table(null);
                                parent.Id = parentId;
                            }                            
                            if (!last.Equals(reader["Name"].ToString()))
                            {
                                con = new Index(parent);
                                con.Name = reader["Name"].ToString();
                                con.Type = (Index.IndexTypeEnum)(byte)reader["type"];
                                con.Id = (int)reader["Index_id"];
                                con.AllowPageLocks = (bool)reader["allow_page_locks"];
                                con.AllowRowLocks = (bool)reader["allow_row_locks"];
                                con.FillFactor = (byte)reader["fill_factor"];
                                con.IgnoreDupKey = (bool)reader["ignore_dup_key"];
                                con.IsAutoStatistics = (bool)reader["NoAutomaticRecomputation"];
                                con.IsDisabled = (bool)reader["is_disabled"];
                                con.IsPadded = (bool)reader["is_padded"];
                                con.IsPrimaryKey = (bool)reader["is_primary_key"];
                                con.IsUniqueKey = (bool)reader["is_unique"];
                                if ((indexFilter.OptionFilter.FilterTableFileGroup) && (con.Type != Index.IndexTypeEnum.XML))
                                    con.FileGroup = reader["FileGroup"].ToString();
                                last = reader["Name"].ToString();
                                cons.Add(con);
                            }
                            IndexColumn ccon = new IndexColumn((Table)con.Parent);
                            ccon.Name = reader["ColumnName"].ToString();
                            ccon.IsIncluded = (bool)reader["is_included_column"];
                            ccon.Order = (bool)reader["is_descending_key"];
                            ccon.Id = (int)reader["column_id"];
                            ccon.KeyOrder = (byte)reader["key_ordinal"];
                            ccon.DataTypeId = (int)reader["user_type_id"];
                            con.Columns.Add(ccon);
                        }
                    }
                }
            }
            return cons;
        }
    }
}
