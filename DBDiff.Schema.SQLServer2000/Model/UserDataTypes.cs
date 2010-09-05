using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    public class UserDataTypes:List<UserDataType> 
    {
        private Hashtable hash = new Hashtable();
        private Database parent;

        public UserDataTypes(Database parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Devuelve la base perteneciente a la coleccion de UserDataTypes.
        /// </summary>
        public Database Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Indica si el nombre del UserDataType existe en la coleccion de UserDataTypes del objeto.
        /// </summary>
        /// <param name="table">
        /// Nombre de la UserDataType a buscar.
        /// </param>
        /// <returns></returns>
        public Boolean Find(string dataType)
        {
            return hash.ContainsKey(dataType);
        }

        public UserDataType this[string name]
        {
            get { return (UserDataType)hash[name]; }
        }

        public new void Add(UserDataType dataType)
        {
            hash.Add(dataType.FullName, dataType);
            base.Add(dataType);
        }

        public string ToXML()
        {
            string xml = "";
            int index;
            xml += "<DATATYPES>\n";
            for (index = 0; index < this.Count; index++)
            {
                xml += this[index].ToXML() + "\n";
            }
            xml += "</DATATYPES>\n";
            return xml;
        }

        public string ToSQL()
        {
            string sql = "";
            int index;
            for (index = 0; index < this.Count; index++)
            {
                sql += this[index].ToSQL();
                if (index != this.Count - 1)
                {
                    sql += ",";
                    sql += "\r\n";
                }
            }
            return sql;
        }

        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList list = new SQLScriptList();
            int index;
            for (index = 0; index < this.Count; index++)
            {
                if (this[index].Status == StatusEnum.ObjectStatusType.DropStatus)
                    list.Add(this[index].ToSQLDrop(), 0, StatusEnum.ScripActionType.DropUserDataType); 
                if (this[index].Status == StatusEnum.ObjectStatusType.CreateStatus)
                    list.Add(this[index].ToSQL(), 0, StatusEnum.ScripActionType.AddUserDataType); 
                if (this[index].Status == StatusEnum.ObjectStatusType.AlterStatus)
                    list.Add(this[index].ToSQLDrop() + this[index].ToSQL(), 0, StatusEnum.ScripActionType.AddUserDataType); 
            }
            return list;
        }
    }
}
