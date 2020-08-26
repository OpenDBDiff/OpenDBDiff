using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Attributes;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class Database : SQLServerSchemaBase, IDatabase
    {
        private readonly List<DatabaseChangeStatus> _changesOptions;

        public Database() : base(null, ObjectType.Database)
        {
            AllObjects = new SearchSchemaBase();
            _changesOptions = new List<DatabaseChangeStatus>();
            Dependencies = new Dependencies();
            TablesTypes = new SchemaList<TableType, Database>(this, AllObjects);
            UserTypes = new SchemaList<UserDataType, Database>(this, AllObjects);
            XmlSchemas = new SchemaList<XMLSchema, Database>(this, AllObjects);
            Schemas = new SchemaList<Schema, Database>(this, AllObjects);
            Procedures = new SchemaList<StoredProcedure, Database>(this, AllObjects);
            CLRProcedures = new SchemaList<CLRStoredProcedure, Database>(this, AllObjects);
            CLRFunctions = new SchemaList<CLRFunction, Database>(this, AllObjects);
            FileGroups = new SchemaList<FileGroup, Database>(this);
            Rules = new SchemaList<Rule, Database>(this, AllObjects);
            DDLTriggers = new SchemaList<Trigger, Database>(this, AllObjects);
            Synonyms = new SchemaList<Synonym, Database>(this, AllObjects);
            Assemblies = new SchemaList<Assembly, Database>(this, AllObjects);
            Views = new SchemaList<View, Database>(this, AllObjects);
            Users = new SchemaList<User, Database>(this, AllObjects);
            FullText = new SchemaList<FullText, Database>(this, AllObjects);
            Functions = new SchemaList<Function, Database>(this, AllObjects);
            PartitionFunctions = new SchemaList<PartitionFunction, Database>(this, AllObjects);
            PartitionSchemes = new SchemaList<PartitionScheme, Database>(this, AllObjects);
            Roles = new SchemaList<Role, Database>(this);
            Tables = new SchemaList<Table, Database>(this, AllObjects);
            Defaults = new SchemaList<Default, Database>(this, AllObjects);
            ActionMessage = new SqlAction(this);
        }

        internal SearchSchemaBase AllObjects { get; private set; }

        [SchemaNode("Full Text Catalog", "FullText")]
        public SchemaList<FullText, Database> FullText { get; private set; }

        [SchemaNode("Table Type", "Table")]
        public SchemaList<TableType, Database> TablesTypes { get; private set; }

        [SchemaNode("Partition Scheme", "PartitionScheme")]
        public SchemaList<PartitionScheme, Database> PartitionSchemes { get; private set; }

        [SchemaNode("Partition Functions", "PartitionFunction")]
        public SchemaList<PartitionFunction, Database> PartitionFunctions { get; private set; }

        [SchemaNode("Defaults")]
        public SchemaList<Default, Database> Defaults { get; private set; }

        [SchemaNode("Roles", "Rol")]
        public SchemaList<Role, Database> Roles { get; private set; }

        [SchemaNode("Functions", "Function", true)]
        public SchemaList<Function, Database> Functions { get; private set; }

        [SchemaNode("Users", "User")]
        public SchemaList<User, Database> Users { get; private set; }

        [SchemaNode("Views", "View", true)]
        public SchemaList<View, Database> Views { get; private set; }

        [SchemaNode("Assemblies", "Assembly")]
        public SchemaList<Assembly, Database> Assemblies { get; private set; }

        [SchemaNode("Synonyms", "Assembly")] // We don't have an icon for synonyms at the moment.
        public SchemaList<Synonym, Database> Synonyms { get; private set; }

        [SchemaNode("DLL Triggers")]
        public SchemaList<Trigger, Database> DDLTriggers { get; private set; }

        [SchemaNode("File Groups")]
        public SchemaList<FileGroup, Database> FileGroups { get; private set; }

        [SchemaNode("Rules")]
        public SchemaList<Rule, Database> Rules { get; private set; }

        [SchemaNode("Stored Procedures", "Procedure", true)]
        public SchemaList<StoredProcedure, Database> Procedures { get; private set; }

        [SchemaNode("CLR Stored Procedures", "CLRProcedure", true)]
        public SchemaList<CLRStoredProcedure, Database> CLRProcedures { get; private set; }

        [SchemaNode("CLR Functions", "CLRFunction", true)]
        public SchemaList<CLRFunction, Database> CLRFunctions { get; private set; }

        [SchemaNode("Schemas", "Schema")]
        public SchemaList<Schema, Database> Schemas { get; private set; }

        [SchemaNode("XML Schemas", "XMLSchema")]
        public SchemaList<XMLSchema, Database> XmlSchemas { get; private set; }

        [SchemaNode("Tables", "Table", true)]
        public SchemaList<Table, Database> Tables { get; private set; }

        [SchemaNode("User Types", "UDT")]
        public SchemaList<UserDataType, Database> UserTypes { get; private set; }

        public SqlOption Options { get; set; }
        IOption IDatabase.Options { get { return Options; } }

        public DatabaseInfo Info { get; set; }

        public DatabaseInfo SourceInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Coleccion de dependencias de constraints.
        /// </summary>
        internal Dependencies Dependencies { get; set; }

        private List<DatabaseChangeStatus> ChangesOptions
        {
            get { return _changesOptions; }
        }

        #region IDatabase Members

        public override ISchemaBase Clone(ISchemaBase parent)
        {
            //Get a list of all of the objects that are SchemaLists, so that we can clone them all.
            var item = new Database() { AllObjects = this.AllObjects };

            var explicitProperties = (from properties in this.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                                      where properties.PropertyType.GetInterface(typeof(ISchemaList<Code, Database>).Name) != null
                                      select properties).ToList();

            foreach (var property in explicitProperties)
            {
                object value = property.GetValue(this, null);

                //Clone the value
                value = value.GetType().GetMethod("Clone").Invoke(value, new object[] { this });

                //Set the value to the cloned object
                property.SetValue(item, value, null);
            }

            return item;
        }

        public SqlAction ActionMessage { get; private set; }

        public Boolean IsCaseSensitive
        {
            get
            {
                bool isCS = false;
                if (!String.IsNullOrEmpty(Info.Collation))
                    isCS = Info.Collation.IndexOf("_CS_") != -1;

                if (Options.Comparison.CaseSensityType == SqlOptionComparison.CaseSensityOptions.Automatic)
                    return isCS;
                if (Options.Comparison.CaseSensityType == SqlOptionComparison.CaseSensityOptions.CaseSensity)
                    return true;
                if (Options.Comparison.CaseSensityType == SqlOptionComparison.CaseSensityOptions.CaseInsensity)
                    return false;

                return false;
            }
        }

        public override string ToSql()
        {
            string sql = "";
            sql += FileGroups.ToSql();
            sql += Schemas.ToSql();
            sql += XmlSchemas.ToSql();
            sql += Rules.ToSql();
            sql += UserTypes.ToSql();
            sql += Assemblies.ToSql();
            sql += Tables.ToSql();
            sql += Functions.ToSql();
            sql += Procedures.ToSql();
            sql += CLRProcedures.ToSql();
            sql += CLRFunctions.ToSql();
            sql += DDLTriggers.ToSql();
            sql += Synonyms.ToSql();
            sql += Views.ToSql();
            sql += Users.ToSql();
            sql += PartitionFunctions.ToSql();
            sql += FullText.ToSql();
            return sql;
        }

        public override SQLScriptList ToSqlDiff(ICollection<ISchemaBase> schemas)
        {
            var isAzure10 = this.Info.Version == DatabaseInfo.SQLServerVersion.SQLServerAzure10;

            var listDiff = new SQLScriptList();

            var header = $@"/*

    OpenDBDiff {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()}
    https://github.com/OpenDBDiff/OpenDBDiff

    Script created by {Environment.UserDomainName}\{Environment.UserName} on {DateTime.Now.ToShortDateString()} at {DateTime.Now.ToLongTimeString()}.

    Created on:  {Environment.MachineName}
    Source:      {SourceInfo?.Database ?? "Unknown"} on {SourceInfo?.Server ?? "Unknown"}
    Destination: {Info?.Database ?? "Unknown"} on {Info?.Server ?? "Unknown"}

    ### This script performs actions to change the Destination schema to the Source schema. ###

*/

";

            listDiff.Add(new SQLScript(header, 0, ScriptAction.None));

            if (!isAzure10)
            {
                listDiff.Add("USE [" + Name + "]\r\nGO\r\n\r\n", 0, ScriptAction.UseDatabase);
                listDiff.AddRange(Assemblies.ToSqlDiff(schemas));
                listDiff.AddRange(UserTypes.ToSqlDiff(schemas));
            }
            listDiff.AddRange(TablesTypes.ToSqlDiff(schemas));
            listDiff.AddRange(Tables.ToSqlDiff(schemas));
            listDiff.AddRange(Rules.ToSqlDiff(schemas));
            listDiff.AddRange(Schemas.ToSqlDiff(schemas));
            listDiff.AddRange(XmlSchemas.ToSqlDiff(schemas));
            listDiff.AddRange(Procedures.ToSqlDiff(schemas));
            if (!isAzure10)
            {
                listDiff.AddRange(CLRProcedures.ToSqlDiff(schemas));
                listDiff.AddRange(CLRFunctions.ToSqlDiff(schemas));
                listDiff.AddRange(FileGroups.ToSqlDiff(schemas));
            }
            listDiff.AddRange(DDLTriggers.ToSqlDiff(schemas));
            listDiff.AddRange(Synonyms.ToSqlDiff(schemas));
            listDiff.AddRange(Views.ToSqlDiff(schemas));
            listDiff.AddRange(Users.ToSqlDiff(schemas));
            listDiff.AddRange(Functions.ToSqlDiff(schemas));
            listDiff.AddRange(Roles.ToSqlDiff(schemas));
            listDiff.AddRange(PartitionFunctions.ToSqlDiff(schemas));
            listDiff.AddRange(PartitionSchemes.ToSqlDiff(schemas));
            if (!isAzure10)
            {
                listDiff.AddRange(FullText.ToSqlDiff(schemas));
            }
            return listDiff;
        }

        public override string ToSqlDrop()
        {
            return "";
        }

        public override string ToSqlAdd()
        {
            return "";
        }

        #endregion

        public ISchemaBase Find(int id)
        {
            try
            {
                string full = AllObjects.GetFullName(id);
                return Find(full);
            }
            catch
            {
                return null;
            }
        }

        public ISchemaBase Find(String _FullName)
        {
            try
            {
                var typeVal = AllObjects.GetType(_FullName);
                if (!typeVal.HasValue)
                {
                    return null;
                }
                ObjectType type = typeVal.Value;


                string parentName = "";

                switch (type)
                {
                    case ObjectType.Table:
                        return Tables[_FullName];
                    case ObjectType.StoredProcedure:
                        return Procedures[_FullName];
                    case ObjectType.Function:
                        return Functions[_FullName];
                    case ObjectType.View:
                        return Views[_FullName];
                    case ObjectType.Assembly:
                        return Assemblies[_FullName];
                    case ObjectType.UserDataType:
                        return UserTypes[_FullName];
                    case ObjectType.TableType:
                        return TablesTypes[_FullName];
                    case ObjectType.XMLSchema:
                        return XmlSchemas[_FullName];
                    case ObjectType.CLRStoredProcedure:
                        return CLRProcedures[_FullName];
                    case ObjectType.CLRFunction:
                        return CLRFunctions[_FullName];
                    case ObjectType.Synonym:
                        return Synonyms[_FullName];
                    case ObjectType.FullText:
                        return FullText[_FullName];
                    case ObjectType.Rule:
                        return Rules[_FullName];
                    case ObjectType.PartitionFunction:
                        return PartitionFunctions[_FullName];
                    case ObjectType.PartitionScheme:
                        return PartitionSchemes[_FullName];
                    case ObjectType.Role:
                        return Roles[_FullName];
                    case ObjectType.Schema:
                        return Schemas[_FullName];
                    case ObjectType.Constraint:
                        parentName = AllObjects.GetParentName(_FullName);
                        return Tables[parentName].Constraints[_FullName];
                    case ObjectType.Index:
                        parentName = AllObjects.GetParentName(_FullName);

                        var typeName = AllObjects.GetType(parentName);
                        if (!typeName.HasValue)
                        {
                            return null;
                        }
                        type = typeName.Value;
                        if (type == ObjectType.Table)
                            return Tables[parentName].Indexes[_FullName];
                        return Views[parentName].Indexes[_FullName];
                    case ObjectType.Trigger:
                        parentName = AllObjects.GetParentName(_FullName);
                        var typeNameB = AllObjects.GetType(parentName);
                        if (!typeNameB.HasValue)
                        {
                            return null;
                        }
                        type = typeNameB.Value;
                        if (type == ObjectType.Table)
                            return Tables[parentName].Triggers[_FullName];
                        return Views[parentName].Triggers[_FullName];
                    case ObjectType.CLRTrigger:
                        parentName = AllObjects.GetParentName(_FullName);
                        var typeNameC = AllObjects.GetType(parentName);
                        if (!typeNameC.HasValue)
                        {
                            return null;
                        }
                        type = typeNameC.Value;
                        if (type == ObjectType.Table)
                            return Tables[parentName].CLRTriggers[_FullName];
                        return Views[parentName].CLRTriggers[_FullName];
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /*private SQLScriptList CleanScripts(SQLScriptList listDiff)
        {
            SQLScriptList alters = listDiff.FindAlter();
            for (int j = 0; j < alters.Count; j++)
            {
                //alters[j].
            }
            return null;
        }*/

        public void BuildDependency()
        {
            ISchemaBase schema;
            var indexes = new List<Index>();
            var constraints = new List<Constraint>();

            Tables.ForEach(item => indexes.AddRange(item.Indexes));
            Views.ForEach(item => indexes.AddRange(item.Indexes));
            Tables.ForEach(item => constraints.AddRange(item.Constraints));

            foreach (Index index in indexes)
            {
                schema = index.Parent;
                foreach (IndexColumn icolumn in index.Columns)
                {
                    Dependencies.Add(this, schema.Id, icolumn.Id, schema.Id, icolumn.DataTypeId, index);
                }
            }

            foreach (Constraint con in constraints)
            {
                schema = con.Parent;
                if (con.Type != Constraint.ConstraintType.Check)
                {
                    foreach (ConstraintColumn ccolumn in con.Columns)
                    {
                        Dependencies.Add(this, schema.Id, ccolumn.Id, schema.Id, ccolumn.DataTypeId, con);
                        if (con.Type == Constraint.ConstraintType.ForeignKey)
                        {
                            Dependencies.Add(this, con.RelationalTableId, ccolumn.ColumnRelationalId, schema.Id,
                                             ccolumn.ColumnRelationalDataTypeId, con);
                        }
                        else
                        {
                            if (
                                ((Table)schema).FullTextIndex.Exists(
                                    item => { return item.Index.Equals(con.Name); }))
                            {
                                Dependencies.Add(this, schema.Id, 0, schema.Id, 0, con);
                            }
                        }
                    }
                }
                else
                    Dependencies.Add(this, schema.Id, 0, schema.Id, 0, con);
            }
        }

        #region Nested type: DatabaseChangeStatus

        private enum DatabaseChangeStatus
        {
            AlterChangeTracking = 1,
            AlterCollation = 2
        }

        #endregion
    }
}
