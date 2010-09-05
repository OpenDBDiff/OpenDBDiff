using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    [Serializable()]
    public class Database:SchemaBase
    {
        private Tables tables;
        private Dependencies dependencies;
        private UserDataTypes userTypes;

        public Database()
            : base("[", "]", StatusEnum.ObjectTypeEnum.Database)
        {
            dependencies = new Dependencies();
        }

        /// <summary>
        /// Coleccion de dependencias de constraints.
        /// </summary>
        internal Dependencies Dependencies
        {
            get { return dependencies; }
            set { dependencies = value; }
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
        /// Convierte el schema de la tabla en XML.
        /// </summary> 
        public string ToXML()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            xml += "<DATABASE driver=\"SQLSERVER2000\">";
            xml += tables.ToXML();
            xml += userTypes.ToXML();
            xml += "</DATABASE>";
            return xml;
        }

        public string ToSQL()
        {
            string sql = "";
            sql += userTypes.ToSQL();
            sql += tables.ToSQL();
            return sql;
        }
        
        public string ToSQLDiff()
        {
            SQLScriptList listDiff;
            string sql = "USE " + Name + "\r\n\r\n";            
            listDiff = tables.ToSQLDiff();
            listDiff.Add(userTypes.ToSQLDiff());
            return listDiff.ToSQL();
        }


    }
}
