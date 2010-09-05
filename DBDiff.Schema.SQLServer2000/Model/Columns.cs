using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    public class Columns : List<Column>
    {
        private Hashtable hash = new Hashtable();
        private Table parent;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="parent">
        /// Objeto Table padre.
        /// </param>
        public Columns(Table parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Clona el objeto Columns en una nueva instancia.
        /// </summary>
        public Columns Clone(Table parentObject)
        {
            Columns columns = new Columns(parentObject);
            for (int index = 0; index < this.Count; index++)
            {
                columns.Add(this[index].Clone(parentObject));
            }
            return columns;
        }

        public Column GetById(int colId)
        {
            for (int index = 0; index < this.Count; index++)
            {
                if (this[index].Id == colId) return this[index];
            }
            return null;
        }

        /// <summary>
        /// Indica si el nombre de la tabla existe en la coleccion de tablas del objeto.
        /// </summary>
        /// <param name="table">
        /// Nombre de la tabla a buscar.
        /// </param>
        /// <returns></returns>
        public Boolean Find(string table)
        {
            return hash.ContainsKey(table);
        }

        /// <summary>
        /// Agrega un objeto columna a la coleccion de columnas.
        /// </summary>
        public new void Add(Column column)
        {
            hash.Add(column.Name, column);
            base.Add(column);
        }

        public Column this[string name]
        {
            get { return (Column)hash[name]; }
            set 
            { 
                hash[name] = value; 
                for (int index = 0;index < base.Count; index++)
                {
                    if (((Column)base[index]).Name.Equals(name))
                    {
                        base[index] = value;
                        break;
                    }
                }                
            }
        }

        /// <summary>
        /// Devuelve la tabla perteneciente a la coleccion de campos.
        /// </summary>
        public Table Parent
        {
            get { return parent; }
        }

        public string ToXML()
        {
            string xml = "";
            int index;
            xml += "<COLUMNS>\n";
            for (index = 0; index < this.Count; index++)
            {
                xml += this[index].ToXML()+"\n";
            }
            xml += "</COLUMNS>\n";
            return xml;
        }

        public string ToSQL()
        {
            string sql = "";
            int index;
            for (index = 0; index < this.Count; index++)
            {
                sql += this[index].ToSQL(true);
                if (index != this.Count-1) 
                {
                    sql += ",";
                    sql += "\r\n";
                }
            }
            return sql;
        }

        public SQLScriptList ToSQLDiff()
        {
            string sqlDrop = "";
            string sqlAdd = "";
            string sqlAlter = "";
            string sqlCons = "";            
            SQLScriptList list = new SQLScriptList();
            int index;
            for (index = 0; index < this.Count; index++)
            {
                if (this[index].Status == StatusEnum.ObjectStatusType.DropStatus)
                    sqlDrop += "[" + this[index].Name + "],";
                if (this[index].Status == StatusEnum.ObjectStatusType.CreateStatus)
                    sqlAdd += "\r\n" + this[index].ToSQL(true) + ",";
                if (this[index].Status == StatusEnum.ObjectStatusType.AlterStatus)
                    sqlAlter += "ALTER TABLE " + parent.FullName + " ALTER COLUMN " + this[index].ToSQL(false) + "\r\nGO\r\n";
                if (this[index].Constraints.Count > 0)
                    sqlCons += this[index].Constraints.ToSQLDiff();
            }
            if (!sqlDrop.Equals("")) 
                sqlDrop = "ALTER TABLE " + parent.FullName + " DROP COLUMN " + sqlDrop.Substring(0,sqlDrop.Length -1) + "\r\nGO\r\n";
            if (!sqlAdd.Equals("")) 
                sqlAdd = "ALTER TABLE " + parent.FullName + " ADD " + sqlAdd.Substring(0,sqlAdd.Length -1)+"\r\nGO\r\n";

            if (!String.IsNullOrEmpty(sqlAlter + sqlDrop + sqlAdd + sqlCons))
                list.Add(sqlAlter + sqlDrop + sqlAdd + sqlCons, 0, StatusEnum.ScripActionType.AlterTable);
            return list;
        }
    }
}
