using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Data.SqlClient;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.SQLServer.Options;
using DBDiff.Schema.SQLServer.Generates.SQLCommands;

namespace DBDiff.Schema.SQLServer.Generates
{
    public class GenerateConstraint
    {
        private string connectionString;
        private SqlOption constraintFilter;
        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateConstraint(string connectioString, SqlOption filter)
        {
            this.connectionString = connectioString;
            constraintFilter = filter;
        }
        #region Check Functions...

        private static string GetSQLCheck()
        {
            string sql;
            sql = "SELECT  ";
            sql += "parent_object_id, ";
            sql += "object_id AS ID, ";
            sql += "parent_column_id, ";
            sql += "name, ";
            sql += "type, ";
            sql += "definition, ";
            sql += "is_disabled, ";
            sql += "is_not_trusted AS WithCheck, ";
            sql += "is_not_for_replication, ";
            sql += "0 ";
            sql += "FROM sys.check_constraints ORDER BY parent_object_id,name";
            return sql;
        }

        public Constraints GetCheck()
        {
            Constraints cons = null;
            int parentId = 0;
            Table table = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(GetSQLCheck(), conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Constraint con = null;
                        while (reader.Read())
                        {
                            if (parentId != (int)reader["parent_object_id"])
                            {
                                parentId = (int)reader["parent_object_id"];
                                table = new Table(null);
                                table.Id = parentId;
                            } 
                            if (cons == null) cons = new Constraints(table);
                            con = new Constraint(table);
                            con.Id = (int)reader["id"];
                            con.NotForReplication = (bool)reader["is_not_for_replication"];
                            con.Name = reader["Name"].ToString();
                            con.Type = Constraint.ConstraintType.Check;
                            con.Definition = reader["Definition"].ToString();
                            con.WithNoCheck = (bool)reader["WithCheck"];
                            con.IsDisabled = (bool)reader["is_disabled"];
                            cons.Add(con);
                            //((Database)table.Parent).ConstraintDependencies.Add(table.Id, 0, table.Id, con);
                        }
                    }
                }
            }
            return cons;
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

        private Constraints GetForeignKey()
        {
            Constraints cons = new Constraints(null);
            string last = "";
            int parentId = 0;
            Table table = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(GetSQLForeignKey(), conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Constraint con = null;
                        while (reader.Read())
                        {
                            if (parentId != (int)reader["parent_object_id"])
                            {
                                parentId = (int)reader["parent_object_id"];
                                table = new Table(null);
                                table.Id = parentId;
                            }                            
                            if (!last.Equals(reader["Name"].ToString()))
                            {
                                //if (!String.IsNullOrEmpty(last)) cons.Add(con);
                                con = new Constraint(table);
                                con.Id = (int)reader["object_id"];
                                con.NotForReplication = (bool)reader["is_not_for_replication"];
                                con.Name = reader["Name"].ToString();
                                con.Type = Constraint.ConstraintType.ForeignKey;
                                con.WithNoCheck = (bool)reader["is_not_trusted"];
                                con.RelationalTableFullName = "[" + reader["ReferenceOwner"].ToString() + "].[" + reader["TableRelationalName"].ToString() + "]";
                                con.RelationalTableId = (int)reader["TableRelationalId"];
                                con.Owner = reader["Owner"].ToString();
                                con.IsDisabled = (bool)reader["is_disabled"];
                                con.OnDeleteCascade = (byte)reader["delete_referential_action"];
                                con.OnUpdateCascade = (byte)reader["update_referential_action"];
                                last = reader["Name"].ToString();
                                cons.Add(con);
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
            return cons;
        }
        #endregion

        #region UniqueKey Functions...

        private static string GetSQLUniqueKey()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT I.object_Id AS id,dsidx.Name as FileGroup, C.user_type_id, C.column_id, I.Index_id, C.Name AS ColumnName, I.Name, I.type, I.fill_factor, I.is_padded, I.allow_row_locks, I.allow_page_locks, I.ignore_dup_key, I.is_disabled, IC.is_descending_key, IC.is_included_column ");
            sql.Append("FROM sys.indexes I ");
            sql.Append("INNER JOIN sys.index_columns IC ON IC.index_id = I.index_id AND IC.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.columns C ON C.column_id = IC.column_id AND IC.object_id = C.object_id ");
            sql.Append("INNER JOIN sys.data_spaces AS dsidx ON dsidx.data_space_id = I.data_space_id ");
            sql.Append("WHERE is_unique_constraint = 1 ORDER BY I.object_id,I.Name");
            return sql.ToString();
        }

        private Constraints GetUniqueKey()
        {
            Constraints cons = new Constraints(null);
            string last = "";
            int parentId = 0;
            Table table = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(GetSQLUniqueKey(), conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Constraint con = null;
                        while (reader.Read())
                        {
                            if (parentId != (int)reader["id"])
                            {
                                parentId = (int)reader["id"];
                                table = new Table(null);
                                table.Id = parentId;
                            } 
                            if (!last.Equals(reader["Name"].ToString()))
                            {
                                con = new Constraint(table);
                                con.Name = reader["Name"].ToString();
                                con.Id = (int)reader["id"];
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
                                if (constraintFilter.OptionFilter.FilterTableFileGroup)
                                    con.Index.FileGroup = reader["FileGroup"].ToString();
                                last = con.Name;
                                cons.Add(con);
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
            return cons;
        }
        #endregion

        #region PrimaryKey Functions...
        private Constraints GetPrimaryKey(Database database)
        {
            Constraints cons = new Constraints(null);            
            string last = "";
            int parentId = 0;
            Table table = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(ConstraintSQLCommand.GetPrimaryKey(database.Info.Version, null), conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Constraint con = null;
                        while (reader.Read())
                        {
                            if (parentId != (int)reader["ID"])
                            {
                                parentId = (int)reader["ID"];
                                table = new Table(null);
                                table.Id = parentId;
                            }
                            if (!last.Equals(reader["Name"].ToString()))
                            {
                                con = new Constraint(table);
                                con.Id = (int)reader["Index_id"];
                                con.Name = (string)reader["Name"];
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
                                if (constraintFilter.OptionFilter.FilterTableFileGroup)
                                    con.Index.FileGroup = reader["FileGroup"].ToString();
                                last = con.Name;
                                cons.Add(con);
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
            return cons;
        }
        #endregion

        public Constraints Get(Database database)
        {
            Constraints constraints = new Constraints(null);
            constraints.AddRange(GetPrimaryKey(database));
            constraints.AddRange(GetForeignKey());
            constraints.AddRange(GetUniqueKey());
            constraints.AddRange(GetCheck());
            return constraints;
        }
    }
}
