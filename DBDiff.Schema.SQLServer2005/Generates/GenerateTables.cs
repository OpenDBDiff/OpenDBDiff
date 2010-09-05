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
using DBDiff.Schema.SQLServer.Generates.Options;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.SQLServer.Generates.Generates.SQLCommands;
using DBDiff.Schema.SQLServer.Generates.Generates.Util;
using DBDiff.Schema.Misc;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateTables
    {
        private static int colIDIndex = -1;
        private static int colNameIndex = -1;
        private static int colFormulaIndex = -1;
        private static int colIsPersistedIndex = -1;
        private static int colIsComputedIndex = -1;
        private static int colNullableIndex = -1;
        private static int colXmlSchemaIndex = -1;
        private static int colIs_xml_documentIndex = -1;
        private static int colPrecisionIndex = -1;
        private static int colScaleIndex = -1;
        private static int colDataUserTypeIdIndex = -1;
        private static int colIsUserDefinedTypeIndex = -1;
        private static int colSizeIndex = -1;
        private static int colHasIndexIndex = -1;
        private static int colHasComputedFormulaIndex = -1;
        private static int colIsRowGuidIndex = -1;
        private static int colTypeIndex = -1;
        private static int colOwnerType = -1;
        private static int colis_sparseIndex = -1;
        private static int colIs_FileStream = -1;
        private static int colDefaultIdIndex = -1;
        private static int colDefaultNameIndex = -1;
        private static int colDefaultDefinitionIndex = -1;
        private static int colrule_object_idIndex = -1;
        private static int colIsIdentityReplIndex = -1;
        private static int colCollationIndex = -1;
        private static int colIsIdentityIndex = -1;
        private static int colIdentSeedIndex = -1;
        private static int colIdentIncrementIndex = -1;
        private static int TableIdIndex = -1;
        private static int TableNameIndex = -1;
        private static int TableOwnerIndex = -1;
        private static int Text_In_Row_limitIndex = -1;
        private static int HasClusteredIndexIndex = -1;
        private static int large_value_types_out_of_rowIndex = -1;
        private static int HasVarDecimalIndex = -1;
        private static int FileGroupIndex = -1;
        private static int FileGroupTextIndex = -1;
        private static int FileGroupStreamIndex = -1;

        private Generate root;

        public GenerateTables(Generate root)
        {
            this.root = root;
        }

        private static void InitTableIndex(Database database, SqlDataReader reader)
        {
            if (TableIdIndex == -1)
            {
                TableIdIndex = reader.GetOrdinal("TableId");
                TableNameIndex = reader.GetOrdinal("TableName");
                TableOwnerIndex = reader.GetOrdinal("TableOwner");
                Text_In_Row_limitIndex = reader.GetOrdinal("Text_In_Row_limit");
                HasClusteredIndexIndex = reader.GetOrdinal("HasClusteredIndex");
                large_value_types_out_of_rowIndex = reader.GetOrdinal("large_value_types_out_of_row");
                HasVarDecimalIndex = reader.GetOrdinal("HasVarDecimal");
                FileGroupIndex = reader.GetOrdinal("FileGroup");
                FileGroupTextIndex = reader.GetOrdinal("FileGroupText");
                if (database.Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2008)
                {
                    FileGroupStreamIndex = reader.GetOrdinal("FileGroupStream");
                }
            }
        }

        private static void InitColIndex(Database database, SqlDataReader reader)
        {
            if (colNameIndex == -1)
            {
                colIDIndex = reader.GetOrdinal("ID");
                colNameIndex = reader.GetOrdinal("Name");
                colFormulaIndex = reader.GetOrdinal("Formula");
                colIsPersistedIndex = reader.GetOrdinal("FormulaPersisted");
                colIsComputedIndex = reader.GetOrdinal("IsComputed");
                colNullableIndex = reader.GetOrdinal("IsNullable");
                colOwnerType = reader.GetOrdinal("OwnerType");
                colXmlSchemaIndex = reader.GetOrdinal("XmlSchema");
                colIs_xml_documentIndex = reader.GetOrdinal("Is_xml_document");
                colPrecisionIndex = reader.GetOrdinal("Precision");
                colScaleIndex = reader.GetOrdinal("Scale");
                colDataUserTypeIdIndex = reader.GetOrdinal("user_type_id");
                colIsUserDefinedTypeIndex = reader.GetOrdinal("is_user_defined");                
                colSizeIndex = reader.GetOrdinal("Size");
                colHasIndexIndex = reader.GetOrdinal("HasIndex");
                colHasComputedFormulaIndex = reader.GetOrdinal("HasComputedFormula");
                colIsRowGuidIndex = reader.GetOrdinal("IsRowGuid");
                colTypeIndex = reader.GetOrdinal("Type");
                colDefaultIdIndex = reader.GetOrdinal("DefaultId");
                colDefaultNameIndex = reader.GetOrdinal("DefaultName");
                colDefaultDefinitionIndex = reader.GetOrdinal("DefaultDefinition");
                colrule_object_idIndex = reader.GetOrdinal("rule_object_id");
                colIsIdentityReplIndex = reader.GetOrdinal("IsIdentityRepl");
                colCollationIndex = reader.GetOrdinal("Collation");
                colIsIdentityIndex = reader.GetOrdinal("IsIdentity");
                colIdentSeedIndex = reader.GetOrdinal("IdentSeed");
                colIdentIncrementIndex = reader.GetOrdinal("IdentIncrement");
                if (database.Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2008)
                {
                    colis_sparseIndex = reader.GetOrdinal("is_sparse");
                    colIs_FileStream = reader.GetOrdinal("is_filestream");
                }
            }
        }

        private static void FillColumn<T>(ITable<T> table, SqlDataReader reader) where T:ISchemaBase
        {
            Database database = (Database)table.Parent;

            InitColIndex(database, reader);
            Column col = new Column((ISchemaBase)table);
            col.Id = (int)reader[colIDIndex];
            if (database.Options.Ignore.FilterColumnOrder)
                col.Position = table.Columns.Count + 1;
            
            if (database.Options.Ignore.FilterColumnCollation)
                col.Collation = (string)reader[colCollationIndex];

            if (database.Options.Ignore.FilterColumnIdentity)
            {
                col.IsIdentity = (bool)reader[colIsIdentityIndex];
                if ((col.IsIdentity) || (col.IsIdentityForReplication))
                {
                    if (!reader.IsDBNull(colIdentSeedIndex))
                        col.IdentitySeed = (int)(decimal)reader[colIdentSeedIndex];
                    else
                        col.IdentitySeed = 1;
                    if (!reader.IsDBNull(colIdentIncrementIndex))
                        col.IdentityIncrement = (int)(decimal)reader[colIdentIncrementIndex];
                    else
                        col.IdentityIncrement = 1;
                }
                if (database.Options.Ignore.FilterNotForReplication)
                    col.IsIdentityForReplication = ((int)reader[colIsIdentityReplIndex] == 1);
            }
            col.Name = (string)reader[colNameIndex];
            col.Owner = table.Owner;
            col.ComputedFormula = (string)reader[colFormulaIndex];
            col.IsPersisted = (bool)reader[colIsPersistedIndex];
            col.IsComputed = (bool)reader[colIsComputedIndex];
            col.IsNullable = (bool)reader[colNullableIndex];            
            col.XmlSchema = reader[colXmlSchemaIndex].ToString();
            col.IsXmlDocument = (bool)reader[colIs_xml_documentIndex];
            col.Precision = (byte)reader[colPrecisionIndex];
            col.Scale = (byte)reader[colScaleIndex];
            col.DataUserTypeId = (int)reader[colDataUserTypeIdIndex];
            col.IsUserDefinedType = (bool)reader[colIsUserDefinedTypeIndex];
            if (!String.IsNullOrEmpty(reader[colSizeIndex].ToString()))
                col.Size = (short)reader[colSizeIndex];
            col.HasIndexDependencies = ((int)reader[colHasIndexIndex] == 1); ;
            col.HasComputedDependencies = ((int)reader[colHasComputedFormulaIndex] == 1);
            col.IsRowGuid = (bool)reader[colIsRowGuidIndex];
            if (col.IsUserDefinedType)
                col.Type = "[" + (string)reader[colOwnerType] + "].[" + (string)reader[colTypeIndex] + "]";
            else
                col.Type = (string)reader[colTypeIndex];
            if (((Database)table.Parent).Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2008)
            {
                col.IsSparse = (bool)reader[colis_sparseIndex];
                col.IsFileStream = (bool)reader[colIs_FileStream];
            }
            if ((int)reader[colDefaultIdIndex] != 0)
            {
                col.DefaultConstraint = new ColumnConstraint(col);
                col.DefaultConstraint.Id = (int)reader[colDefaultIdIndex];
                col.DefaultConstraint.Owner = table.Owner;
                col.DefaultConstraint.Name = (string)reader[colDefaultNameIndex];
                col.DefaultConstraint.Type = Constraint.ConstraintType.Default;
                col.DefaultConstraint.Definition = (string)reader[colDefaultDefinitionIndex];
            }
            if ((int)reader[colrule_object_idIndex] != 0)
                col.Rule = ((Database)table.Parent).Rules.Find((int)reader[colrule_object_idIndex]);
            table.Columns.Add(col);
        }

        public void Fill(Database database, string connectionString, List<MessageLog> messages)
        {
            try
            {
                root.RaiseOnReading(new ProgressEventArgs("Reading tables...",Constants.READING_TABLES));
                FillTables(database, connectionString);
                if ((database.Tables.Count > 0) || (database.TablesTypes.Count > 0))
                {
                    if (database.Options.Ignore.FilterConstraint)
                        (new GenerateConstraint(root)).Fill(database, connectionString);
                }
            }
            catch (Exception ex)
            {
                messages.Add(new MessageLog(ex.Message,ex.StackTrace, MessageLog.LogType.Error));
            }
        }

        private void FillTables(Database database, string connectionString)
        {
            try
            {
                int textInRow;
                Boolean largeValues;
                Boolean varDecimal;
                int lastObjectId = 0;
                bool isTable = true;
                ISchemaBase item = null;

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
                                InitTableIndex(database, reader);
                                if (lastObjectId != (int)reader[TableIdIndex])
                                {
                                    lastObjectId = (int)reader[TableIdIndex];
                                    isTable = reader["ObjectType"].ToString().Trim().Equals("U");
                                    if (isTable)
                                    {
                                        item = new Table(database);
                                        item.Id = (int)reader[TableIdIndex];
                                        item.Name = (string)reader[TableNameIndex];
                                        item.Owner = (string)reader[TableOwnerIndex];
                                        ((Table)item).HasClusteredIndex = (int)reader[HasClusteredIndexIndex] == 1;
                                        textInRow = (int)reader[Text_In_Row_limitIndex];
                                        largeValues = (Boolean)reader[large_value_types_out_of_rowIndex];
                                        varDecimal = ((int)reader[HasVarDecimalIndex]) == 1;
                                        if (database.Options.Ignore.FilterTableFileGroup)
                                        {
                                            ((Table)item).FileGroup = (string)reader[FileGroupIndex];
                                            ((Table)item).FileGroupText = (string)reader[FileGroupTextIndex];
                                            if (database.Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2008)
                                            {
                                                ((Table)item).FileGroupStream = (string)reader[FileGroupStreamIndex];
                                            }
                                        }
                                        if (database.Options.Ignore.FilterTableOption)
                                        {
                                            if (textInRow > 0) ((Table)item).Options.Add(new TableOption("TextInRow", textInRow.ToString(CultureInfo.InvariantCulture), item));
                                            if (largeValues) ((Table)item).Options.Add(new TableOption("LargeValues", "1", item));
                                            if (varDecimal) ((Table)item).Options.Add(new TableOption("VarDecimal", "1", item));
                                        }
                                        database.Tables.Add((Table)item);
                                    }
                                    else
                                    {
                                        item = new TableType(database);
                                        item.Id = (int)reader[TableIdIndex];
                                        item.Name = (string)reader[TableNameIndex];
                                        item.Owner = (string)reader[TableOwnerIndex];
                                        database.TablesTypes.Add((TableType)item);
                                    }
                                }
                                if (isTable)
                                {
                                    if (database.Options.Ignore.FilterTable)
                                        FillColumn<Table>((ITable<Table>)item, reader);
                                }
                                else
                                {
                                    if (database.Options.Ignore.FilterUserDataType)
                                        FillColumn<TableType>((ITable<TableType>)item, reader);
                                }                                
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
