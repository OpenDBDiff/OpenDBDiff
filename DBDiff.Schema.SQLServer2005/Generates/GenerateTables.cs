using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Data.SqlClient;
using System.Threading;
using DBDiff.Schema.Errors;
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
            if (((Database)table.Parent).Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2008)
            {
                col.IsSparse = (bool)reader["is_sparse"];
            }
            if ((int)reader["DefaultId"] != 0)
            {
                col.DefaultConstraint = new ColumnConstraint(col);
                col.DefaultConstraint.Id = (int)reader["DefaultId"];
                col.DefaultConstraint.Owner = table.Owner;
                col.DefaultConstraint.Name = reader["DefaultName"].ToString();
                col.DefaultConstraint.Type = Constraint.ConstraintType.Default;
                col.DefaultConstraint.Definition = reader["DefaultDefinition"].ToString();
            }
            if ((int)reader["rule_object_id"] != 0)
                col.Rule = ((Database)table.Parent).Rules.Find((int)reader["rule_object_id"]);
            table.Columns.Add(col);
        }

        public static void Fill(Database database, string connectionString, List<MessageLog> messages)
        {
            try
            {
                FillTables(database, connectionString);
                if (database.Tables.Count > 0)
                {
                    if (database.Options.Ignore.FilterTrigger)
                        GenerateTriggers.Fill(database, connectionString, messages);
                    if (database.Options.Ignore.FilterIndex)
                        GenerateIndex.Fill(database, connectionString, "U");
                    if (database.Options.Ignore.FilterConstraint)
                        GenerateConstraint.Fill(database, connectionString);
                }
            }
            catch (Exception ex)
            {
                messages.Add(new MessageLog(ex.Message,ex.StackTrace, MessageLog.LogType.Error));
            }
        }

        private static void FillTables(Database database, string connectionString)
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
                        command.CommandTimeout = 0;
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


    }
}
