using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.MySQL.Model
{
    public class Database : MySQLSchemaBase
    {
        private Tables tables;

        public Database():base(StatusEnum.ObjectTypeEnum.Database)
        {
            //dependencies = new ConstraintDependencies();
        }

        /// <summary>
        /// Coleccion de dependencias de constraints.
        /// </summary>
        /*internal ConstraintDependencies Dependencies
        {
            get { return dependencies; }
            set { dependencies = value; }
        }*/

        /// <summary>
        /// Coleccion de tablas de la base.
        /// </summary>
        public Tables Tables
        {
            get { return tables; }
            set { tables = value; }
        }

        public string ToSQL()
        {
            string sql = "";
            sql += tables.ToSQL();
            return sql;
        }
        
        public string ToSQLDiff()
        {
            SQLScriptList listDiff;
            listDiff = tables.ToSQLDiff();
            return listDiff.ToSQL();
        }


    }
}
