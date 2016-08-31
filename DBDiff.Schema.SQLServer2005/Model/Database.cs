using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DBDiff.Schema.Attributes;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Options;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Database : SQLServerSchemaBase, IDatabase
    {
        private readonly List<DatabaseChangeStatus> _changesOptions;

        public Database() : base(null, Enums.ObjectType.Database)
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

        [ShowItem("Full Text Catalog", "FullText")]
        public SchemaList<FullText, Database> FullText { get; private set; }

        [ShowItem("Table Type", "Table")]
        public SchemaList<TableType, Database> TablesTypes { get; private set; }

        [ShowItem("Partition Scheme", "PartitionScheme")]
        public SchemaList<PartitionScheme, Database> PartitionSchemes { get; private set; }

        [ShowItem("Partition Functions", "PartitionFunction")]
        public SchemaList<PartitionFunction, Database> PartitionFunctions { get; private set; }

        [ShowItem("Defaults")]
        public SchemaList<Default, Database> Defaults { get; private set; }

        [ShowItem("Roles", "Rol")]
        public SchemaList<Role, Database> Roles { get; private set; }

        [ShowItem("Functions", "Function", true)]
        public SchemaList<Function, Database> Functions { get; private set; }

        [ShowItem("Users", "User")]
        public SchemaList<User, Database> Users { get; private set; }

        [ShowItem("Views", "View", true)]
        public SchemaList<View, Database> Views { get; private set; }

        [ShowItem("Assemblies", "Assembly")]
        public SchemaList<Assembly, Database> Assemblies { get; private set; }

        [ShowItem("Synonyms")]
        public SchemaList<Synonym, Database> Synonyms { get; private set; }

        [ShowItem("DLL Triggers")]
        public SchemaList<Trigger, Database> DDLTriggers { get; private set; }

        [ShowItem("File Groups")]
        public SchemaList<FileGroup, Database> FileGroups { get; private set; }

        [ShowItem("Rules")]
        public SchemaList<Rule, Database> Rules { get; private set; }

        [ShowItem("Stored Procedures", "Procedure", true)]
        public SchemaList<StoredProcedure, Database> Procedures { get; private set; }

        [ShowItem("CLR Stored Procedures", "CLRProcedure", true)]
        public SchemaList<CLRStoredProcedure, Database> CLRProcedures { get; private set; }

        [ShowItem("CLR Functions", "CLRFunction", true)]
        public SchemaList<CLRFunction, Database> CLRFunctions { get; private set; }

        [ShowItem("Schemas", "Schema")]
        public SchemaList<Schema, Database> Schemas { get; private set; }

        [ShowItem("XML Schemas", "XMLSchema")]
        public SchemaList<XMLSchema, Database> XmlSchemas { get; private set; }

        [ShowItem("Tables", "Table", true)]
        public SchemaList<Table, Database> Tables { get; private set; }

        [ShowItem("User Types", "UDT")]
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

        public Boolean IsCaseSensity
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

        /*public List<ISchemaBase> FindAllByColumn(String ColumnName)
        {
            this.t
        }*/

        public override SQLScriptList ToSqlDiff()
        {
            return ToSqlDiff(new List<ISchemaBase>());
        }
        public override SQLScriptList ToSqlDiff(List<ISchemaBase> schemas)
        {
            var isAzure10 = this.Info.Version == DatabaseInfo.VersionTypeEnum.SQLServerAzure10;

            var listDiff = new SQLScriptList();
            listDiff.Add(new SQLScript(String.Format(@"/*

    Open DBDiff {0}
    http://opendbiff.codeplex.com/

    Script created by {1}\{2} on {3} at {4}.

    Created on:  {5}
    Source:      {6} on {7}
    Destination: {8} on {9}

*/

",
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                Environment.UserDomainName,
                Environment.UserName,
                DateTime.Now.ToShortDateString(),
                DateTime.Now.ToLongTimeString(),
                Environment.MachineName,
                SourceInfo != null ? SourceInfo.Database : "Uknown",
                SourceInfo != null ? SourceInfo.Server : "Uknown",
                Info != null ? Info.Database : "Uknown",
                Info != null ? Info.Server : "Uknown",
                0), 0, Enums.ScripActionType.None));
            if (!isAzure10)
            {
                listDiff.Add("USE [" + Name + "]\r\nGO\r\n\r\n", 0, Enums.ScripActionType.UseDatabase);
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
                Enums.ObjectType type = AllObjects.GetType(_FullName);
                string parentName = "";

                switch (type)
                {
                    case Enums.ObjectType.Table:
                        return Tables[_FullName];
                    case Enums.ObjectType.StoredProcedure:
                        return Procedures[_FullName];
                    case Enums.ObjectType.Function:
                        return Functions[_FullName];
                    case Enums.ObjectType.View:
                        return Views[_FullName];
                    case Enums.ObjectType.Assembly:
                        return Assemblies[_FullName];
                    case Enums.ObjectType.UserDataType:
                        return UserTypes[_FullName];
                    case Enums.ObjectType.TableType:
                        return TablesTypes[_FullName];
                    case Enums.ObjectType.XMLSchema:
                        return XmlSchemas[_FullName];
                    case Enums.ObjectType.CLRStoredProcedure:
                        return CLRProcedures[_FullName];
                    case Enums.ObjectType.CLRFunction:
                        return CLRFunctions[_FullName];
                    case Enums.ObjectType.Synonym:
                        return Synonyms[_FullName];
                    case Enums.ObjectType.FullText:
                        return FullText[_FullName];
                    case Enums.ObjectType.Rule:
                        return Rules[_FullName];
                    case Enums.ObjectType.PartitionFunction:
                        return PartitionFunctions[_FullName];
                    case Enums.ObjectType.PartitionScheme:
                        return PartitionSchemes[_FullName];
                    case Enums.ObjectType.Role:
                        return Roles[_FullName];
                    case Enums.ObjectType.Schema:
                        return Schemas[_FullName];
                    case Enums.ObjectType.Constraint:
                        parentName = AllObjects.GetParentName(_FullName);
                        return Tables[parentName].Constraints[_FullName];
                    case Enums.ObjectType.Index:
                        parentName = AllObjects.GetParentName(_FullName);
                        type = AllObjects.GetType(parentName);
                        if (type == Enums.ObjectType.Table)
                            return Tables[parentName].Indexes[_FullName];
                        return Views[parentName].Indexes[_FullName];
                    case Enums.ObjectType.Trigger:
                        parentName = AllObjects.GetParentName(_FullName);
                        type = AllObjects.GetType(parentName);
                        if (type == Enums.ObjectType.Table)
                            return Tables[parentName].Triggers[_FullName];
                        return Views[parentName].Triggers[_FullName];
                    case Enums.ObjectType.CLRTrigger:
                        parentName = AllObjects.GetParentName(_FullName);
                        type = AllObjects.GetType(parentName);
                        if (type == Enums.ObjectType.Table)
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
