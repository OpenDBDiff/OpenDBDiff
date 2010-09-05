using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    public class TableOptions:List<TableOption> 
    {
        private Hashtable hash = new Hashtable();
        private Table parent;

        public TableOptions(Table parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Clona el objeto Columns en una nueva instancia.
        /// </summary>
        public TableOptions Clone()
        {
            TableOptions options = new TableOptions(parent);
            for (int index = 0; index < this.Count; index++)
            {
                options.Add(this[index].Clone());
            }
            return options;
        }

        /// <summary>
        /// Devuelve la tabla perteneciente a la coleccion de campos.
        /// </summary>
        public Table Parent
        {
            get { return parent; }
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

        public TableOption this[string name]
        {
            get { return (TableOption)hash[name]; }
            set
            {
                hash[name] = value;
                for (int index = 0; index < base.Count; index++)
                {
                    if (((TableOption)base[index]).Name.Equals(name))
                    {
                        base[index] = value;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Agrega un objeto columna a la coleccion de columnas.
        /// </summary>
        public void Add(string name, string value)
        {
            TableOption prop = new TableOption(parent);
            prop.Name = name;
            prop.Value = value;
            prop.Status = StatusEnum.ObjectStatusType.OriginalStatus;
            hash.Add(name, prop);
            base.Add(prop);
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
            string sqlDrop = "";
            string sqlAdd = "";
            string sqlAlter = "";
            SQLScriptList list = new SQLScriptList();
            int index;
            for (index = 0; index < this.Count; index++)
            {
                if (this[index].Status == StatusEnum.ObjectStatusType.DropStatus)
                    sqlDrop += this[index].ToSQLDrop();
                if (this[index].Status == StatusEnum.ObjectStatusType.CreateStatus)
                    sqlAdd += this[index].ToSQL();
                if (this[index].Status == StatusEnum.ObjectStatusType.AlterStatus)
                    sqlAlter += this[index].ToSQLDrop() + this[index].ToSQL();
            }
            if (!String.IsNullOrEmpty(sqlAlter + sqlDrop + sqlAdd))
                list.Add(sqlAlter + sqlDrop + sqlAdd, parent.DependenciesCount, StatusEnum.ScripActionType.AlterOptions);
            return list;
        }
    }
}
