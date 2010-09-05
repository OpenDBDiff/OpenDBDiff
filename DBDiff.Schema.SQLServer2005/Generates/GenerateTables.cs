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
using DBDiff.Schema.Misc;

namespace DBDiff.Schema.SQLServer.Generates
{
    public static class GenerateTables
    {             
        private static void FillColumn(Table table, SqlDataReader reader)
        {
            Column col = new Column(table);
            col.Id = (int)reader["ID"];
            if (!((Database)table.Parent).Options.Ignore.FilterIgnoreColumnOrder)
                col.Position = table.Columns.Count + 1;
            if (!((Database)table.Parent).Options.Ignore.FilterIgnoreNotForReplication)
                col.IsIdentityForReplication = ((int)reader["IsIdentityRepl"] == 1);
            if (!((Database)table.Parent).Options.Ignore.FilterIgnoreColumnCollation)
                col.Collation = reader["Collation"].ToString();

            col.IsIdentity = (bool)reader["IsIdentity"];
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
            col.Owner = table.Owner;
            col.ComputedFormula = reader["formula"].ToString();
            col.IsPersisted = (bool)reader["FormulaPersisted"];
            col.IsComputed = (bool)reader["IsComputed"];
            col.Nullable = (bool)reader["IsNullable"];
            
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

            ColumnConstraint cc = new ColumnConstraint(col);
            cc.Id = (int)reader["DefaultId"];
            if (cc.Id != 0)
            {
                cc.Owner = table.Owner;
                cc.Name = reader["DefaultName"].ToString();
                cc.Type = Constraint.ConstraintType.Default;
                cc.Definition = reader["DefaultDefinition"].ToString();
                col.Constraints.Add(cc);
            }
            if ((int)reader["rule_object_id"] != 0)
                col.Rule = ((Database)table.Parent).Rules.Find((int)reader["rule_object_id"]);
            table.Columns.Add(col);
        }

        public static void Fill(Database database, string connectionString)
        {
            string error = "";
            Triggers triggers = null;
            Indexes indexes = null;
            Constraints constraints = null;
            try
            {
                FillTables(database, connectionString);
                if (database.Options.Ignore.FilterTrigger)
                    triggers = (new GenerateTriggers(connectionString, database.Options).Get());
                if (database.Options.Ignore.FilterIndex)
                    indexes = (new GenerateIndex(connectionString, database.Options)).Get("U");
                if (database.Options.Ignore.FilterConstraint)
                    constraints = (new GenerateConstraint(connectionString, database.Options)).Get(database);
                if (String.IsNullOrEmpty(error))
                    Merge(database.Tables, triggers, indexes, constraints);
                else
                    throw new SchemaException(error);                
            }
            catch
            {
                throw;
            }
        }

        public static void FillTables(Database database, string connectionString)
        {
            try
            {
                int textInRow;
                Boolean largeValues;
                Boolean varDecimal;
                int lastObjectId = 0;
                Table item = null;

                using (SqlConnection conn = new SqlConnection(connectionString))
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
                                    if (database.Options.Ignore.FilterTableFileGroup)
                                    {
                                        item.FileGroup = reader["FileGroup"].ToString();
                                        item.FileGroupText = reader["FileGroupText"].ToString();
                                    }
                                    if (database.Options.Ignore.FilterTableOption)
                                    {
                                        if (textInRow > 0) item.Options.Add("TextInRow", textInRow.ToString(CultureInfo.InvariantCulture));
                                        if (largeValues) item.Options.Add("LargeValues", "1");
                                        if (varDecimal) item.Options.Add("VarDecimal", "1");
                                    }
                                    database.Tables.Add(item);
                                }
                                if (database.Options.Ignore.FilterTable)
                                    FillColumn(item, reader);
                            }
                        }
                    }
                }
                //tables.ToSQL();
            }
            catch
            {
                throw;
            }
        }

        private static void Merge(Tables tables, Triggers triggers, Indexes indexes, Constraints constraints)
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
        }
    }
}
