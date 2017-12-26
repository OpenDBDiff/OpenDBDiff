using System;
using System.Data.SqlClient;
using System.Text;
using DBDiff.Schema.Events;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Generates.SQLCommands;
using DBDiff.Schema.SQLServer.Generates.Generates.Util;
using DBDiff.Schema.SQLServer.Generates.Model;

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
            return SQLQueries.SQLQueryFactory.Get("DBDiff.Schema.SQLServer.Generates.SQLQueries.GetForeignKeys");
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
