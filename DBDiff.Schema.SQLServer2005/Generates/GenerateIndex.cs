using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Globalization;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.SQLServer.Options;
using DBDiff.Schema.SQLServer.Generates.SQLCommands;

namespace DBDiff.Schema.SQLServer.Generates
{
    public class GenerateIndex
    {
        public static void Fill(Database database, string connectionString, string type)
        {
            string last = "";
            int parentId = 0;
            ISchemaBase parent = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(IndexSQLCommand.Get(database.Info.Version,type), conn))
                {
                    conn.Open();
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Index con = null;
                        while (reader.Read())
                        {
                            if (parentId != (int)reader["object_id"])
                            {
                                parentId = (int)reader["object_id"];
                                if (type.Equals("V"))
                                    parent = database.Views.Find(parentId);
                                else
                                    parent = database.Tables.Find(parentId);
                            }                            
                            if (!last.Equals(reader["Name"].ToString()))
                            {
                                con = new Index(parent);
                                con.Name = reader["Name"].ToString();
                                con.Type = (Index.IndexTypeEnum)(byte)reader["type"];
                                con.Id = (int)reader["Index_id"];
                                con.AllowPageLocks = (bool)reader["allow_page_locks"];
                                con.AllowRowLocks = (bool)reader["allow_row_locks"];
                                con.IgnoreDupKey = (bool)reader["ignore_dup_key"];
                                con.IsAutoStatistics = (bool)reader["NoAutomaticRecomputation"];
                                con.IsDisabled = (bool)reader["is_disabled"];
                                con.IsPadded = (bool)reader["is_padded"];
                                con.IsPrimaryKey = (bool)reader["is_primary_key"];
                                con.IsUniqueKey = (bool)reader["is_unique"];
                                if (database.Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2008)
                                {
                                    con.FilterDefintion = reader["FilterDefinition"].ToString();
                                }
                                if (!database.Options.Ignore.FilterIndexFillFactor)
                                    con.FillFactor = (byte)reader["fill_factor"];
                                if ((database.Options.Ignore.FilterTableFileGroup) && (con.Type != Index.IndexTypeEnum.XML))
                                    con.FileGroup = reader["FileGroup"].ToString();
                                last = reader["Name"].ToString();
                                if (type.Equals("V"))
                                    ((View)parent).Indexes.Add(con);
                                else
                                    ((Table)parent).Indexes.Add(con);
                            }
                            IndexColumn ccon = new IndexColumn((Table)con.Parent);
                            ccon.Name = reader["ColumnName"].ToString();
                            ccon.IsIncluded = (bool)reader["is_included_column"];
                            ccon.Order = (bool)reader["is_descending_key"];
                            ccon.Id = (int)reader["column_id"];
                            ccon.KeyOrder = (byte)reader["key_ordinal"];
                            ccon.DataTypeId = (int)reader["user_type_id"];
                            if ((!ccon.IsIncluded) || (ccon.IsIncluded && !database.Options.Ignore.FilterIndexIncludeColumns))
                                con.Columns.Add(ccon);
                        }
                    }
                }
            }
        }
    }
}
