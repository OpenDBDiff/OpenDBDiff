using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Data.SqlClient;
using System.Threading;
using DBDiff.Schema.Model;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Options;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.SQLServer.Generates.SQLCommands;

namespace DBDiff.Schema.SQLServer.Generates
{
    public class GenerateTables
    {        
        private string connectioString;
        private SqlOption objectFilter;
        //public event Progress.ProgressHandler OnTableProgress;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateTables(string connectioString, SqlOption filter)
        {
            this.connectioString = connectioString;
            this.objectFilter = filter;
        }

        private int GetTablesCount(Database database)
        {
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(TableSQLCommand.GetTableCount(database.Info.Version), conn))
                {
                    return (int)command.ExecuteScalar();
                }
            }
        }
     
        private Column AddColumns(SqlDataReader reader, Table table)
        {
            Column col = new Column(table);
            col.Id = (int)reader["ID"];
            col.IsIdentity = (bool)reader["IsIdentity"];
            col.IsIdentityForReplication = ((int)reader["IsIdentityRepl"] == 1);
            if ((col.IsIdentity) || (col.IsIdentityForReplication))
            {
                if (!reader.IsDBNull(reader.GetOrdinal("IdentSeed")))
                    col.IdentitySeed = (int)(decimal)reader["IdentSeed"];
                else
                    col.IdentitySeed = 1;
                if (!reader.IsDBNull(reader.GetOrdinal("IdentIncrement")))
                    col.IdentityIncrement = (int)(decimal)reader["IdentIncrement"];
                else
                    col.IdentityIncrement = 1;
            }
            col.Name = reader["Name"].ToString();
            col.ComputedFormula = reader["formula"].ToString();
            col.IsPersisted = (bool)reader["FormulaPersisted"];
            col.IsComputed = (bool)reader["IsComputed"];
            col.Nullable = (bool)reader["IsNullable"];
            col.Collation = reader["Collation"].ToString();
            col.XmlSchema = reader["XMLSchema"].ToString();
            col.IsXmlDocument = (bool)reader["Is_xml_document"];
            col.Precision = (byte)reader["Precision"];
            col.Scale = (byte)reader["Scale"];
            col.DataUserTypeId = (int)reader["user_type_id"];
            col.IsUserDefinedType = (bool)reader["is_user_defined"];
            if (!String.IsNullOrEmpty(reader["Size"].ToString()))
                col.Size = (short)reader["Size"];
            col.HasIndexDependencies = ((int)reader["HasIndex"] == 1); ;
            col.HasComputedDependencies = ((int)reader["HasComputedFormula"] == 1);
            col.IsRowGuid = (bool)reader["IsRowGuid"];
            col.Type = reader["Type"].ToString();
            if (objectFilter.OptionFilter.FilterColumnPosition)
                col.Position = table.Columns.Count + 1;
            else
                col.Position = 0;

            ColumnConstraint cc = new ColumnConstraint(col);
            cc.Id = (int)reader["DefaultId"];
            if (cc.Id != 0)
            {
                cc.Name = reader["DefaultName"].ToString();
                cc.Type = Constraint.ConstraintType.Default;
                cc.Definition = reader["DefaultDefinition"].ToString();
                col.Constraints.Add(cc);
            }
            return col;
        }

        public Tables Get(Database database)
        {
            Tables tables = null;
            Triggers triggers = null;
            Indexes indexes = null;
            Constraints constraints = null;
            string error = "";
            try
            {
                Thread t1 = new Thread(delegate()
                {
                    try
                    {
                        tables = GetTables(database);
                    }
                    catch (Exception ex)
                    {
                        error = ex.StackTrace;
                    }
                });
                Thread t2 = new Thread(delegate()
                {
                    try
                    {
                        if (objectFilter.OptionFilter.FilterTrigger)
                            triggers = (new GenerateTriggers(connectioString, objectFilter).Get());
                    }
                    catch (Exception ex)
                    {
                        error = ex.StackTrace;
                    }
                });
                Thread t3 = new Thread(delegate()
                {
                    try
                    {
                        if (objectFilter.OptionFilter.FilterIndex)
                            indexes = (new GenerateIndex(connectioString, objectFilter)).Get("U");
                    }
                    catch (Exception ex)
                    {
                        error = ex.StackTrace;
                    }
                });
                Thread t4 = new Thread(delegate()
                {
                    try
                    {
                        if (objectFilter.OptionFilter.FilterConstraint)
                            constraints = (new GenerateConstraint(connectioString, objectFilter)).Get(database);
                    }
                    catch (Exception ex)
                    {
                        error = ex.StackTrace;
                    }
                });
                t1.Start();
                t2.Start();
                t3.Start();
                t4.Start();
                t1.Join();
                t2.Join();
                t3.Join();
                t4.Join();

                return Merge(tables, triggers, indexes, constraints);
            }
            catch
            {
                throw;
            }
        }

        private Tables GetTables(Database database)
        {
            try
            {
                Tables tables = new Tables(database);
                double tableCount = GetTablesCount(database);
                double tableIndex = 0;
                int textInRow;
                Boolean largeValues;
                Boolean varDecimal;
                int lastObjectId = 0;
                Table item = null;

                using (SqlConnection conn = new SqlConnection(connectioString))
                {
                    using (SqlCommand command = new SqlCommand(TableSQLCommand.GetTableDetail(database.Info.Version), conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (lastObjectId != (int)reader["TableId"])
                                {
                                    lastObjectId = (int)reader["TableId"];
                                    item = new Table(database);
                                    item.Id = (int)reader["TableId"];
                                    item.Name = reader["TableName"].ToString();
                                    item.Owner = reader["TableOwner"].ToString();
                                    item.HasClusteredIndex = (int)reader["HasClusteredIndex"] == 1;
                                    textInRow = (int)reader["Text_In_Row_limit"];
                                    largeValues = (Boolean)reader["large_value_types_out_of_row"];
                                    varDecimal = ((int)reader["HasVarDecimal"]) == 1;
                                    if (objectFilter.OptionFilter.FilterTableFileGroup)
                                    {
                                        item.FileGroup = reader["FileGroup"].ToString();
                                        item.FileGroupText = reader["FileGroupText"].ToString();
                                    }
                                    if (objectFilter.OptionFilter.FilterTableOption)
                                    {
                                        if (textInRow > 0) item.Options.Add("TextInRow", textInRow.ToString(CultureInfo.InvariantCulture));
                                        if (largeValues) item.Options.Add("LargeValues", "1");
                                        if (varDecimal) item.Options.Add("VarDecimal", "1");
                                    }
                                    tables.Add(item);
                                    tableIndex++;
                                }
                                if (objectFilter.OptionFilter.FilterTable)
                                    item.Columns.Add(AddColumns(reader, item));
                                //OnTableProgress(this,new ProgressEventArgs((tableIndex / tableCount) * 100));
                            }
                        }
                    }
                }

                //tables.Sort();
                tables.ToSQL();
                return tables;
            }
            catch
            {
                throw;
            }
        }

        private Tables Merge(Tables tables, Triggers triggers, Indexes indexes, Constraints constraints)
        {
            if (triggers != null)
            {
                foreach (Trigger trigger in triggers)
                {
                    Table table = tables.Find(trigger.Parent.Id);
                    trigger.Parent = table;
                    table.Triggers.Add(trigger);
                }
            }
            if (indexes != null)
            {
                foreach (Index index in indexes)
                {
                    Table parent = tables.Find(index.Parent.Id);
                    index.Parent = parent;
                    parent.Indexes.Add(index);
                    foreach (IndexColumn icolumn in index.Columns)
                    {
                        ((Database)parent.Parent).Dependencies.Add(parent.Id, icolumn.Id, parent.Id, icolumn.DataTypeId, index);
                    }
                }
            }
            if (constraints != null)
            {
                foreach (Constraint con in constraints)
                {
                    Table table = tables.Find(con.Parent.Id);
                    con.Parent = table;
                    table.Constraints.Add(con);
                    if (con.Type != Constraint.ConstraintType.Check)
                    {
                        foreach (ConstraintColumn ccolumn in con.Columns)
                        {
                            ((Database)table.Parent).Dependencies.Add(table.Id, ccolumn.Id, table.Id, ccolumn.DataTypeId, con);
                            if (con.Type == Constraint.ConstraintType.ForeignKey)
                            {
                                ((Database)table.Parent).Dependencies.Add(con.RelationalTableId, ccolumn.ColumnRelationalId, table.Id, ccolumn.ColumnRelationalDataTypeId , con);
                            }
                        }
                    }
                    else
                        ((Database)table.Parent).Dependencies.Add(table.Id, 0, table.Id, 0, con);
                }
            }
            return tables;
        }
    }
}
