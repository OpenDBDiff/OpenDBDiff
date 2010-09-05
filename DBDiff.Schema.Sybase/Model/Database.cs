using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.Sybase.Model
{
    public class Database : SybaseSchemaBase
    {
        private Tables tables;
        /*private Dependencies constraintDependencies;
        private UserDataTypes userTypes;
        private StoreProcedures procedures;*/

        public Database():base(StatusEnum.ObjectTypeEnum.Database)
        {
            /*constraintDependencies = new Dependencies();
            userTypes = new UserDataTypes(this);
            procedures = new StoreProcedures(this);*/
        }

        /// <summary>
        /// Coleccion de dependencias de constraints.
        /// </summary>
        /*internal Dependencies ConstraintDependencies
        {
            get { return constraintDependencies; }
            set { constraintDependencies = value; }
        }*/

        /// <summary>
        /// Coleccion de Store Procedures de la base.
        /// </summary>
        /*public StoreProcedures Procedures
        {
            get { return procedures; }
            set { procedures = value; }
        }*/

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
        /*public UserDataTypes UserTypes
        {
            get { return userTypes; }
            set { userTypes = value; }
        }*/

        public string ToSQL()
        {
            string sql = "";
            /*sql += userTypes.ToSQL();*/
            sql += tables.ToSQL();
            /*sql += procedures.ToSQL();*/
            return sql;
        }
        
        public string ToSQLDiff()
        {
            SQLScriptList listDiff;
            //string sql = "USE " + Name + "\r\n\r\n";            
            listDiff = tables.ToSQLDiff();
            //listDiff.Add(userTypes.ToSQLDiff());
            //listDiff.Add(procedures.ToSQLDiff());
            return listDiff.ToSQL();
        }


    }
}
