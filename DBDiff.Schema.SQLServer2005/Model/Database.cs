using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Options;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Database : SQLServerSchemaBase
    {
        private Tables tables;
        private Dependencies constraintDependencies;
        private UserDataTypes userTypes;
        private XMLSchemas xmlSchemas;
        private Schemas schemas;
        private StoreProcedures procedures;
        private FileGroups fileGroups;
        private SqlOption options;
        private Rules rules;
        private Triggers ddlTriggers;
        private DatabaseInfo info;
        private Synonyms synonyms;
        private Assemblys assemblies;
        private Views views;

        public Database():base(StatusEnum.ObjectTypeEnum.Database)
        {
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
        }

        public Views Views
        {
            get { return views; }
            set { views = value; }
        }

        public Assemblys Assemblies
        {
            get { return assemblies; }
            set { assemblies = value; }
        }

        public Synonyms Synonyms
        {
            get { return synonyms; }
            set { synonyms = value; }
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

        public Triggers DDLTriggers
        {
            get { return ddlTriggers; }
            set { ddlTriggers = value; }
        }

        public FileGroups FileGroups
        {
            get { return fileGroups; }
            set { fileGroups = value; }
        }

        public Rules Rules
        {
            get { return rules; }
            set { rules = value; }
        }

        /// <summary>
        /// Coleccion de Store Procedures de la base.
        /// </summary>
        public StoreProcedures Procedures
        {
            get { return procedures; }
            set { procedures = value; }
        }

        /// <summary>
        /// Coleccion de schemas de la base.
        /// </summary>
        public Schemas Schemas
        {
            get { return schemas; }
            set { schemas = value; }
        }

        /// <summary>
        /// Coleccion de XML schemas de la base
        /// </summary>
        public XMLSchemas XmlSchemas
        {
            get { return xmlSchemas; }
            set { xmlSchemas = value; }
        }

        /// <summary>
        /// Coleccion de tablas de la base.
        /// </summary>
        public Tables Tables
        {
            get { return tables; }
            set { tables = value; }
        }

        /// <summary>
        /// Coleccion de userTypes de la base.
        /// </summary>
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

        public string ToSQL()
        {
            string sql = "";
            sql += fileGroups.ToSQL();
            sql += schemas.ToSQL();
            sql += xmlSchemas.ToSQL();
            sql += rules.ToSQL();
            sql += userTypes.ToSQL();
            sql += assemblies.ToSQL();
            sql += tables.ToSQL();
            sql += procedures.ToSQL();
            sql += ddlTriggers.ToSQL();
            sql += synonyms.ToSQL();
            sql += views.ToSQL();
            return sql;
        }
        
        public string ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();
            listDiff.Add("USE " + Name + "\r\nGO\r\n\r\n", 0, StatusEnum.ScripActionType.UseDatabase);
            listDiff.Add(tables.ToSQLDiff());
            listDiff.Add(userTypes.ToSQLDiff());
            listDiff.Add(rules.ToSQLDiff());
            listDiff.Add(schemas.ToSQLDiff());
            listDiff.Add(xmlSchemas.ToSQLDiff());
            listDiff.Add(procedures.ToSQLDiff());
            listDiff.Add(fileGroups.ToSQLDiff());
            listDiff.Add(ddlTriggers.ToSQLDiff());
            listDiff.Add(synonyms.ToSQLDiff());
            listDiff.Add(assemblies.ToSQLDiff());
            listDiff.Add(views.ToSQLDiff());
            return listDiff.ToSQL();
        }

        public override string ToSQLDrop()
        {
            return "";
        }

        public override string ToSQLAdd()
        {
            return "";
        }
    }
}
