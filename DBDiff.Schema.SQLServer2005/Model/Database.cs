using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DBDiff.Schema.Model;
using DBDiff.Schema.Attributes;
using DBDiff.Schema.SQLServer.Options;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Database : SQLServerSchemaBase
    {
        private SqlOption options;
        
        private Dependencies constraintDependencies;        
        private Defaults defaults;        
        private DatabaseInfo info;
        private SchemaList<FileGroup, Database> fileGroups;
        private SchemaList<UserDataType, Database> userTypes;                
        private SchemaList<Schema, Database> schemas;        
        private SchemaList<Trigger, Database> ddlTriggers;
        private SchemaList<XMLSchema, Database> xmlSchemas;
        private SchemaList<Assembly, Database> assemblies;                
        private SchemaList<Function, Database> functions;
        private SchemaList<StoreProcedure, Database> procedures;
        private SchemaList<CLRStoreProcedure, Database> CLRprocedures;
        private SchemaList<Rule, Database> rules;
        private SchemaList<Synonym, Database> synonyms;
        private SchemaList<Table, Database> tables;
        private SchemaList<User, Database> users;
        private SchemaList<View, Database> views;
        private Roles roles;
        private Dictionary<string, ISchemaBase> allObjects;

        public Database():base(Enums.ObjectType.Database)
        {
            allObjects = new Dictionary<string, ISchemaBase>();
            constraintDependencies = new Dependencies();
            userTypes = new SchemaList<UserDataType, Database>(this, this.allObjects);
            xmlSchemas = new SchemaList<XMLSchema, Database>(this, this.allObjects);
            schemas = new SchemaList<Schema, Database>(this, this.allObjects);
            procedures = new SchemaList<StoreProcedure, Database>(this, this.allObjects);
            CLRprocedures = new SchemaList<CLRStoreProcedure, Database>(this, this.allObjects);
            fileGroups = new SchemaList<FileGroup, Database>(this);
            rules = new SchemaList<Rule, Database>(this, this.allObjects);
            ddlTriggers = new SchemaList<Trigger, Database>(this, this.allObjects);
            synonyms = new SchemaList<Synonym, Database>(this, this.allObjects);
            assemblies = new SchemaList<Assembly, Database>(this,this.allObjects);
            views = new SchemaList<View, Database>(this, this.allObjects);
            users = new SchemaList<User, Database>(this, this.allObjects);
            functions = new SchemaList<Function, Database>(this,this.allObjects);
            roles = new Roles(this);
            tables = new SchemaList<Table, Database>(this, this.allObjects);
            defaults = new Defaults(this);            
        }

        internal Dictionary<string, ISchemaBase> AllObjects
        {
            get { return allObjects; }            
        }

        [ShowItemAttribute("Defaults")]
        public Defaults Defaults
        {
            get { return defaults; }
            set { defaults = value; }
        }

        [ShowItemAttribute("Roles","Rol")]
        public Roles Roles
        {
            get { return roles; }
        }

        [ShowItemAttribute("Functions","Function",true)]
        public SchemaList<Function, Database> Functions
        {
            get { return functions; }
        }

        [ShowItemAttribute("Users","User")]
        public SchemaList<User, Database> Users
        {
            get { return users; }
        }

        [ShowItemAttribute("Views","View",true)]
        public SchemaList<View, Database> Views
        {
            get { return views; }
            set { views = value; }
        }

        [ShowItemAttribute("Assemblies","Assembly")]
        public SchemaList<Assembly, Database> Assemblies
        {
            get { return assemblies; }
        }

        [ShowItemAttribute("Synonyms")]
        public SchemaList<Synonym, Database> Synonyms
        {
            get { return synonyms; }
        }

        public SqlOption Options
        {
            get { return options; }
            set { options = value; }
        }

        public DatabaseInfo Info
        {
            get { return info; }
            set { info = value; }
        }

        /// <summary>
        /// Coleccion de dependencias de constraints.
        /// </summary>
        internal Dependencies Dependencies
        {
            get { return constraintDependencies; }
            set { constraintDependencies = value; }
        }

        [ShowItemAttribute("DLL Triggers")]
        public SchemaList<Trigger, Database> DDLTriggers
        {
            get { return ddlTriggers; }
        }

        [ShowItemAttribute("File Groups")]
        public SchemaList<FileGroup, Database> FileGroups
        {
            get { return fileGroups; }
        }

        [ShowItemAttribute("Rules")]
        public SchemaList<Rule, Database> Rules
        {
            get { return rules; }
        }

        /// <summary>
        /// Coleccion de Store Procedures de la base.
        /// </summary>
        [ShowItemAttribute("Store Procedures","Procedure", true)]
        public SchemaList<StoreProcedure, Database> Procedures
        {
            get { return procedures; }
        }

        [ShowItemAttribute("CLR Store Procedures", "CLRProcedure", true)]
        public SchemaList<CLRStoreProcedure, Database> CLRProcedures
        {
            get { return CLRprocedures; }
        }

        /// <summary>
        /// Coleccion de schemas de la base.
        /// </summary>
        [ShowItemAttribute("Schemas","Schema")]
        public SchemaList<Schema, Database> Schemas
        {
            get { return schemas; }
        }

        /// <summary>
        /// Coleccion de XML schemas de la base
        /// </summary>
        [ShowItemAttribute("XML Schemas","XMLSchema")]
        public SchemaList<XMLSchema, Database> XmlSchemas
        {
            get { return xmlSchemas; }
        }

        /// <summary>
        /// Coleccion de tablas de la base.
        /// </summary>
        [ShowItemAttribute("Tables","Table")]
        public SchemaList<Table, Database> Tables
        {
            get { return tables; }
        }

        /// <summary>
        /// Coleccion de userTypes de la base.
        /// </summary>
        [ShowItemAttribute("User Types","UDT")]
        public SchemaList<UserDataType, Database> UserTypes
        {
            get { return userTypes; }
            set { userTypes = value; }
        }

        
        /// <summary>
        /// Nombre del objecto
        /// </summary>
        public new string Name
        {
            get
            {
                if (base.Name.IndexOf(' ', 0) == -1)
                    return base.Name;
                else
                    return "[" + base.Name + "]";
            }
            set { base.Name = value; }
        }

        public override string ToSql()
        {
            string sql = "";
            sql += fileGroups.ToSQL();
            sql += schemas.ToSQL();
            sql += xmlSchemas.ToSQL();
            sql += rules.ToSQL();
            sql += userTypes.ToSQL();
            sql += assemblies.ToSQL();
            sql += tables.ToSQL();
            sql += functions.ToSQL();
            sql += procedures.ToSQL();
            sql += CLRprocedures.ToSQL();
            sql += ddlTriggers.ToSQL();
            sql += synonyms.ToSQL();
            sql += views.ToSQL();
            sql += users.ToSQL();
            return sql;
        }

        /*public List<ISchemaBase> FindAllByColumn(String ColumnName)
        {
            this.t
        }*/

        public ISchemaBase Find(String FullName)
        {
            if (allObjects.ContainsKey(FullName))
                return allObjects[FullName];
            else
                return null;
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();
            listDiff.Add("USE " + Name + "\r\nGO\r\n\r\n", 0, Enums.ScripActionType.UseDatabase);
            listDiff.AddRange(assemblies.ToSqlDiff());
            listDiff.AddRange(userTypes.ToSqlDiff());
            listDiff.AddRange(tables.ToSqlDiff());            
            listDiff.AddRange(rules.ToSqlDiff());
            listDiff.AddRange(schemas.ToSqlDiff());
            listDiff.AddRange(xmlSchemas.ToSqlDiff());
            listDiff.AddRange(procedures.ToSqlDiff());
            listDiff.AddRange(CLRprocedures.ToSqlDiff());
            listDiff.AddRange(fileGroups.ToSqlDiff());
            listDiff.AddRange(ddlTriggers.ToSqlDiff());
            listDiff.AddRange(synonyms.ToSqlDiff());            
            listDiff.AddRange(views.ToSqlDiff());
            listDiff.AddRange(users.ToSqlDiff());
            listDiff.AddRange(functions.ToSqlDiff());
            listDiff.AddRange(roles.ToSqlDiff());
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

        public void BuildDependency()
        {
            ISchemaBase schema;
            List<Index> indexes = new List<Index>();
            Constraints constraints = new Constraints(null);

            this.Tables.ForEach(item => { indexes.AddRange(item.Indexes); });
            this.Views.ForEach(item => { indexes.AddRange(item.Indexes); });
            this.Tables.ForEach(item => { constraints.AddRange(item.Constraints); });

            foreach (Index index in indexes)
            {
                schema = index.Parent;
                foreach (IndexColumn icolumn in index.Columns)
                {
                    this.Dependencies.Add(schema.Id, icolumn.Id, schema.Id, icolumn.DataTypeId, index);
                }
            }

            if (constraints != null)
            {
                foreach (Constraint con in constraints)
                {
                    schema = con.Parent;
                    if (con.Type != Constraint.ConstraintType.Check)
                    {
                        foreach (ConstraintColumn ccolumn in con.Columns)
                        {
                            this.Dependencies.Add(schema.Id, ccolumn.Id, schema.Id, ccolumn.DataTypeId, con);
                            if (con.Type == Constraint.ConstraintType.ForeignKey)
                            {
                                this.Dependencies.Add(con.RelationalTableId, ccolumn.ColumnRelationalId, schema.Id, ccolumn.ColumnRelationalDataTypeId, con);
                            }
                        }
                    }
                    else
                        this.Dependencies.Add(schema.Id, 0, schema.Id, 0, con);
                }
            }
        }
    }
}
