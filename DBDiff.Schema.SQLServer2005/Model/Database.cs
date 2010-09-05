using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;
using DBDiff.Schema.Attributes;
using DBDiff.Schema.SQLServer.Options;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Database : SQLServerSchemaBase
    {
        private SqlOption options;
        private Tables tables;
        private Dependencies constraintDependencies;
        private UserDataTypes userTypes;
        private XMLSchemas xmlSchemas;
        private Schemas schemas;
        private StoreProcedures procedures;
        private FileGroups fileGroups;        
        private Rules rules;
        private Defaults defaults;
        private Triggers ddlTriggers;
        private DatabaseInfo info;
        private Synonyms synonyms;
        private Assemblys assemblies;
        private Views views;
        private Users users;
        private Functions functions;
        private Roles roles;
        private Dictionary<string, ISchemaBase> allObjects;

        public Database():base(Enums.ObjectType.Database)
        {
            allObjects = new Dictionary<string, ISchemaBase>();
            constraintDependencies = new Dependencies();
            userTypes = new UserDataTypes(this);
            xmlSchemas = new XMLSchemas(this);
            schemas = new Schemas(this);
            procedures = new StoreProcedures(this);
            fileGroups = new FileGroups(this);
            rules = new Rules(this);
            ddlTriggers = new Triggers(this);
            synonyms = new Synonyms(this);
            assemblies = new Assemblys(this);
            views = new Views(this);
            users = new Users(this);
            functions = new Functions(this);
            roles = new Roles(this);
            tables = new Tables(this);
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
        public Functions Functions
        {
            get { return functions; }
        }

        [ShowItemAttribute("Users","User")]
        public Users Users
        {
            get { return users; }
        }

        [ShowItemAttribute("Views","View",true)]
        public Views Views
        {
            get { return views; }
            set { views = value; }
        }

        [ShowItemAttribute("Assemblies","Assembly")]
        public Assemblys Assemblies
        {
            get { return assemblies; }
        }

        [ShowItemAttribute("Synonyms")]
        public Synonyms Synonyms
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
        public Triggers DDLTriggers
        {
            get { return ddlTriggers; }
        }

        [ShowItemAttribute("File Groups")]
        public FileGroups FileGroups
        {
            get { return fileGroups; }
        }

        [ShowItemAttribute("Rules")]
        public Rules Rules
        {
            get { return rules; }
        }

        /// <summary>
        /// Coleccion de Store Procedures de la base.
        /// </summary>
        [ShowItemAttribute("Store Procedures","Procedure", true)]
        public StoreProcedures Procedures
        {
            get { return procedures; }
        }

        /// <summary>
        /// Coleccion de schemas de la base.
        /// </summary>
        [ShowItemAttribute("Schemas","Schema")]
        public Schemas Schemas
        {
            get { return schemas; }
        }

        /// <summary>
        /// Coleccion de XML schemas de la base
        /// </summary>
        [ShowItemAttribute("XML Schemas","XMLSchema")]
        public XMLSchemas XmlSchemas
        {
            get { return xmlSchemas; }
        }

        /// <summary>
        /// Coleccion de tablas de la base.
        /// </summary>
        [ShowItemAttribute("Tables","Table")]
        public Tables Tables
        {
            get { return tables; }
        }

        /// <summary>
        /// Coleccion de userTypes de la base.
        /// </summary>
        [ShowItemAttribute("User Types","UDT")]
        public UserDataTypes UserTypes
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

        public string ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();
            listDiff.Add("USE " + Name + "\r\nGO\r\n\r\n", 0, Enums.ScripActionType.UseDatabase);
            listDiff.AddRange(assemblies.ToSQLDiff());
            listDiff.AddRange(userTypes.ToSQLDiff());
            listDiff.AddRange(tables.ToSQLDiff());            
            listDiff.AddRange(rules.ToSQLDiff());
            listDiff.AddRange(schemas.ToSQLDiff());
            listDiff.AddRange(xmlSchemas.ToSQLDiff());
            listDiff.AddRange(procedures.ToSQLDiff());
            listDiff.AddRange(fileGroups.ToSQLDiff());
            listDiff.AddRange(ddlTriggers.ToSQLDiff());
            listDiff.AddRange(synonyms.ToSQLDiff());            
            listDiff.AddRange(views.ToSQLDiff());
            listDiff.AddRange(users.ToSQLDiff());
            listDiff.AddRange(functions.ToSQLDiff());
            listDiff.AddRange(roles.ToSQLDiff());
            return listDiff.ToSQL();
        }

        public override string ToSqlDrop()
        {
            return "";
        }

        public override string ToSqlAdd()
        {
            return "";
        }
    }
}
