using System;
using System.Collections.Generic;
using DBDiff.Schema.Attributes;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Options;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Database : SQLServerSchemaBase, IDatabase
    {
        private readonly List<DatabaseChangeStatus> _changesOptions;
        private DatabaseInfo _info;

        public Database() : base(null, Enums.ObjectType.Database)
        {
            AllObjects = new SearchSchemaBase();
            _changesOptions = new List<DatabaseChangeStatus>();
            Dependencies = new Dependencies();
            TablesTypes = new SchemaList<TableType, Database>(this, AllObjects);
            UserTypes = new SchemaList<UserDataType, Database>(this, AllObjects);
            XmlSchemas = new SchemaList<XMLSchema, Database>(this, AllObjects);
            Schemas = new SchemaList<Schema, Database>(this, AllObjects);
            Procedures = new SchemaList<StoreProcedure, Database>(this, AllObjects);
            CLRProcedures = new SchemaList<CLRStoreProcedure, Database>(this, AllObjects);
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

        [ShowItem("Store Procedures", "Procedure", true)]
        public SchemaList<StoreProcedure, Database> Procedures { get; private set; }

        [ShowItem("CLR Store Procedures", "CLRProcedure", true)]
        public SchemaList<CLRStoreProcedure, Database> CLRProcedures { get; private set; }

        [ShowItem("CLR Functions", "CLRFunction", true)]
        public SchemaList<CLRFunction, Database> CLRFunctions { get; private set; }

        [ShowItem("Schemas", "Schema")]
        public SchemaList<Schema, Database> Schemas { get; private set; }

        [ShowItem("XML Schemas", "XMLSchema")]
        public SchemaList<XMLSchema, Database> XmlSchemas { get; private set; }

        [ShowItem("Tables", "Table")]
        public SchemaList<Table, Database> Tables { get; private set; }

        [ShowItem("User Types", "UDT")]
        public SchemaList<UserDataType, Database> UserTypes { get; private set; }

        public SqlOption Options { get; set; }

        public DatabaseInfo Info
        {
            get { return _info; }
            set { _info = value; }
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
            var item = new Database
                           {
                               Assemblies = Assemblies.Clone(this),
                               Tables = Tables.Clone(this),
                               Procedures = Procedures.Clone(this),
                               Functions = Functions.Clone(this),
                               Views = Views.Clone(this),
                               AllObjects = AllObjects
                           };
            return item;
        }

        public SqlAction ActionMessage { get; private set; }

        public Boolean IsCaseSensity
        {
            get
            {
                bool isCS = false;
                if (!String.IsNullOrEmpty(_info.Collation))
                    isCS = _info.Collation.IndexOf("_CS_") != -1;

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
            var listDiff = new SQLScriptList();
            listDiff.Add("USE [" + Name + "]\r\nGO\r\n\r\n", 0, Enums.ScripActionType.UseDatabase);
            listDiff.AddRange(Assemblies.ToSqlDiff());
            listDiff.AddRange(UserTypes.ToSqlDiff());
            listDiff.AddRange(TablesTypes.ToSqlDiff());
            listDiff.AddRange(Tables.ToSqlDiff());
            listDiff.AddRange(Rules.ToSqlDiff());
            listDiff.AddRange(Schemas.ToSqlDiff());
            listDiff.AddRange(XmlSchemas.ToSqlDiff());
            listDiff.AddRange(Procedures.ToSqlDiff());
            listDiff.AddRange(CLRProcedures.ToSqlDiff());
            listDiff.AddRange(CLRFunctions.ToSqlDiff());
            listDiff.AddRange(FileGroups.ToSqlDiff());
            listDiff.AddRange(DDLTriggers.ToSqlDiff());
            listDiff.AddRange(Synonyms.ToSqlDiff());
            listDiff.AddRange(Views.ToSqlDiff());
            listDiff.AddRange(Users.ToSqlDiff());
            listDiff.AddRange(Functions.ToSqlDiff());
            listDiff.AddRange(Roles.ToSqlDiff());
            listDiff.AddRange(PartitionFunctions.ToSqlDiff());
            listDiff.AddRange(PartitionSchemes.ToSqlDiff());
            listDiff.AddRange(FullText.ToSqlDiff());
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
                    case Enums.ObjectType.StoreProcedure:
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
                    case Enums.ObjectType.CLRStoreProcedure:
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
                                ((Table) schema).FullTextIndex.Exists(
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