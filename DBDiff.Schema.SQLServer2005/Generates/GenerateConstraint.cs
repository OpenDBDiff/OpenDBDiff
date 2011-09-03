using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.SQLServer.Generates.Options;
using DBDiff.Schema.SQLServer.Generates.Generates.SQLCommands;
using DBDiff.Schema.Model;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Generates.Generates.Util;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateConstraint
    {
        private Generate root;

        public GenerateConstraint(Generate root)
        {
            this.root = root;
        }

        #region Check Functions...
        public void FillCheck(Database database, string connectionString)
        {
            int parentId = 0;
            ISchemaBase table = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(ConstraintSQLCommand.GetCheck(database.Info.Version), conn))
                {
                    root.RaiseOnReading(new ProgressEventArgs("Reading constraint...", Constants.READING_CONSTRAINTS));
                    conn.Open();
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Constraint item = null;
                        while (reader.Read())
                        {
                            root.RaiseOnReadingOne(reader["Name"]);
                            if (parentId != (int)reader["parent_object_id"])
                            {
                                parentId = (int)reader["parent_object_id"];
                                if (reader["ObjectType"].ToString().Trim().Equals("U"))
                                    table = database.Tables.Find(parentId);
                                else
                                    table = database.TablesTypes.Find(parentId);
                            } 
                            item = new Constraint(table);
                            item.Id = (int)reader["id"];
                            item.Name = reader["Name"].ToString();
                            item.Type = Constraint.ConstraintType.Check;
                            item.Definition = reader["Definition"].ToString();
                            item.WithNoCheck = (bool)reader["WithCheck"];
                            item.IsDisabled = (bool)reader["is_disabled"];
                            item.Owner = reader["Owner"].ToString();
                            if (database.Options.Ignore.FilterNotForReplication)
                                item.NotForReplication = (bool)reader["is_not_for_replication"];
                            if (reader["ObjectType"].ToString().Trim().Equals("U"))
                                ((Table)table).Constraints.Add(item);
                            else
                                ((TableType)table).Constraints.Add(item);
                        }
                    }
                }
            }
        }

        #endregion
        
        #region ForeignKey Functions...

        private static string GetSQLForeignKey()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT FK.object_id, C.user_type_id ,FK.parent_object_id,S.Name AS Owner, S2.Name AS ReferenceOwner, C2.Name AS ColumnName, C2.column_id AS ColumnId, C.name AS ColumnRelationalName, C.column_id AS ColumnRelationalId, T.object_id AS TableRelationalId, FK.Parent_object_id AS TableId, T.Name AS TableRelationalName, FK.Name, FK.is_disabled, FK.is_not_for_replication, FK.is_not_trusted, FK.delete_referential_action, FK.update_referential_action ");
            sql.Append("FROM sys.foreign_keys FK ");
            sql.Append("INNER JOIN sys.tables T ON T.object_id = FK.referenced_object_id ");
            sql.Append("INNER JOIN sys.schemas S2 ON S2.schema_id = T.schema_id ");
            sql.Append("INNER JOIN sys.foreign_key_columns FKC ON FKC.constraint_object_id = FK.object_id ");
            sql.Append("INNER JOIN sys.columns C ON C.object_id = FKC.referenced_object_id AND C.column_id = referenced_column_id ");
            sql.Append("INNER JOIN sys.columns C2 ON C2.object_id = FKC.parent_object_id AND C2.column_id = parent_column_id ");
            sql.Append("INNER JOIN sys.schemas S ON S.schema_id = FK.schema_id ");
            sql.Append("ORDER BY FK.parent_object_id, FK.Name, ColumnId");
            return sql.ToString();
        }

        private static void FillForeignKey(Database database, string connectionString)
        {
            int lastid = 0;
            int parentId = 0;
            Table table = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(GetSQLForeignKey(), conn))
                {
                    conn.Open();
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Constraint con = null;
                        while (reader.Read())
                        {
                            if (parentId != (int)reader["parent_object_id"])
                            {
                                parentId = (int)reader["parent_object_id"];
                                table = database.Tables.Find(parentId);
                            }
                            if (lastid != (int)reader["object_id"])
                            {
                                con = new Constraint(table);
                                con.Id = (int)reader["object_id"];
                                con.Name = reader["Name"].ToString();
                                con.Type = Constraint.ConstraintType.ForeignKey;
                                con.WithNoCheck = (bool)reader["is_not_trusted"];
                                con.RelationalTableFullName = "[" + reader["ReferenceOwner"].ToString() + "].[" + reader["TableRelationalName"].ToString() + "]";
                                con.RelationalTableId = (int)reader["TableRelationalId"];
                                con.Owner = reader["Owner"].ToString();
                                con.IsDisabled = (bool)reader["is_disabled"];
                                con.OnDeleteCascade = (byte)reader["delete_referential_action"];
                                con.OnUpdateCascade = (byte)reader["update_referential_action"];
                                if (database.Options.Ignore.FilterNotForReplication)
                                    con.NotForReplication = (bool)reader["is_not_for_replication"];
                                lastid = (int)reader["object_id"];
                                table.Constraints.Add(con);
                            }
                            ConstraintColumn ccon = new ConstraintColumn(con);
                            ccon.Name = reader["ColumnName"].ToString();
                            ccon.ColumnRelationalName = reader["ColumnRelationalName"].ToString();
                            ccon.ColumnRelationalId = (int)reader["ColumnRelationalId"];
                            ccon.Id = (int)reader["ColumnId"];
                            ccon.KeyOrder = con.Columns.Count;
                            ccon.ColumnRelationalDataTypeId = (int)reader["user_type_id"];
                            //table.DependenciesCount++;
                            con.Columns.Add(ccon);
                        }                        
                    }
                }
            }
        }
        #endregion

        #region UniqueKey Functions...
        private static void FillUniqueKey(Database database, string connectionString)
        {
            int lastId = 0;
            int parentId = 0;
            bool change = false;
            ISchemaBase table = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(ConstraintSQLCommand.GetUniqueKey(database.Info.Version), conn))
                {
                    conn.Open();
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Constraint con = null;
                        while (reader.Read())
                        {
                            if (parentId != (int)reader["ID"])
                            {
                                parentId = (int)reader["ID"];
                                if (reader["ObjectType"].ToString().Trim().Equals("U"))
                                    table = database.Tables.Find(parentId);
                                else
                                    table = database.TablesTypes.Find(parentId);
                                change = true;
                            }
                            else
                                change = false;
                            if ((lastId != (int)reader["Index_id"]) || (change))
                            {
                                con = new Constraint(table);
                                con.Name = reader["Name"].ToString();
                                con.Owner = (string)reader["Owner"];
                                con.Id = (int)reader["Index_id"];
                                con.Type = Constraint.ConstraintType.Unique;
                                con.Index.Id = (int)reader["Index_id"];
                                con.Index.AllowPageLocks = (bool)reader["allow_page_locks"];
                                con.Index.AllowRowLocks = (bool)reader["allow_row_locks"];
                                con.Index.FillFactor = (byte)reader["fill_factor"];
                                con.Index.IgnoreDupKey = (bool)reader["ignore_dup_key"];
                                con.Index.IsAutoStatistics = (bool)reader["ignore_dup_key"];
                                con.Index.IsDisabled = (bool)reader["is_disabled"];
                                con.Index.IsPadded = (bool)reader["is_padded"];
                                con.Index.IsPrimaryKey = false;
                                con.Index.IsUniqueKey = true;
                                con.Index.Type = (Index.IndexTypeEnum)(byte)reader["type"];
                                con.Index.Name = con.Name;
                                if (database.Options.Ignore.FilterTableFileGroup)
                                    con.Index.FileGroup = reader["FileGroup"].ToString();
                                lastId = (int)reader["Index_id"];
                                if (reader["ObjectType"].ToString().Trim().Equals("U"))
                                    ((Table)table).Constraints.Add(con);
                                else
                                    ((TableType)table).Constraints.Add(con);
                            }
                            ConstraintColumn ccon = new ConstraintColumn(con);
                            ccon.Name = reader["ColumnName"].ToString();
                            ccon.IsIncluded = (bool)reader["is_included_column"];
                            ccon.Order = (bool)reader["is_descending_key"];
                            ccon.Id = (int)reader["column_id"];
                            ccon.DataTypeId = (int)reader["user_type_id"];
                            con.Columns.Add(ccon);                            
                        }
                    }
                }
            }
        }
        #endregion

        #region PrimaryKey Functions...
        private static void FillPrimaryKey(Database database, string connectionString)
        {
            int lastId = 0;
            int parentId = 0;
            bool change = false;
            ISchemaBase table = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(ConstraintSQLCommand.GetPrimaryKey(database.Info.Version, null), conn))
                    {
                        conn.Open();
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Constraint con = null;
                            while (reader.Read())
                            {
                                if (parentId != (int)reader["ID"])
                                {
                                    parentId = (int)reader["ID"];
                                    if (reader["ObjectType"].ToString().Trim().Equals("U"))
                                        table = database.Tables.Find(parentId);
                                    else
                                        table = database.TablesTypes.Find(parentId);
                                    change = true;
                                }
                                else
                                    change = false;
                                if ((lastId != (int)reader["Index_id"]) || (change))
                                {
                                    con = new Constraint(table);
                                    con.Id = (int)reader["Index_id"];
                                    con.Name = (string)reader["Name"];
                                    con.Owner = (string)reader["Owner"];
                                    con.Type = Constraint.ConstraintType.PrimaryKey;
                                    con.Index.Id = (int)reader["Index_id"];
                                    con.Index.AllowPageLocks = (bool)reader["allow_page_locks"];
                                    con.Index.AllowRowLocks = (bool)reader["allow_row_locks"];
                                    con.Index.FillFactor = (byte)reader["fill_factor"];
                                    con.Index.IgnoreDupKey = (bool)reader["ignore_dup_key"];
                                    con.Index.IsAutoStatistics = (bool)reader["ignore_dup_key"];
                                    con.Index.IsDisabled = (bool)reader["is_disabled"];
                                    con.Index.IsPadded = (bool)reader["is_padded"];
                                    con.Index.IsPrimaryKey = true;
                                    con.Index.IsUniqueKey = false;
                                    con.Index.Type = (Index.IndexTypeEnum)(byte)reader["type"];
                                    con.Index.Name = con.Name;
                                    if (database.Options.Ignore.FilterTableFileGroup)
                                        con.Index.FileGroup = reader["FileGroup"].ToString();
                                    lastId = (int)reader["Index_id"];
                                    if (reader["ObjectType"].ToString().Trim().Equals("U"))
                                        ((Table)table).Constraints.Add(con);
                                    else
                                        ((TableType)table).Constraints.Add(con);
                                }
                                ConstraintColumn ccon = new ConstraintColumn(con);
                                ccon.Name = (string)reader["ColumnName"];
                                ccon.IsIncluded = (bool)reader["is_included_column"];
                                ccon.Order = (bool)reader["is_descending_key"];
                                ccon.KeyOrder = (byte)reader["key_ordinal"];
                                ccon.Id = (int)reader["column_id"];
                                ccon.DataTypeId = (int)reader["user_type_id"];
                                con.Columns.Add(ccon);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        public void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterConstraintPK)
                FillPrimaryKey(database, connectionString);
            if (database.Options.Ignore.FilterConstraintFK)
                FillForeignKey(database, connectionString);
            if (database.Options.Ignore.FilterConstraintUK)
                FillUniqueKey(database, connectionString);
            if (database.Options.Ignore.FilterConstraintCheck)
                FillCheck(database, connectionString);
        }
    }
}
