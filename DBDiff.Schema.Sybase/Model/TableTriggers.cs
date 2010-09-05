using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.Sybase.Model
{
    public class TableTriggers:List<TableTrigger>
    {
        private Hashtable hash = new Hashtable();
        private Table parent;

        public TableTriggers(Table parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Clona el objeto Triggers en una nueva instancia.
        /// </summary>
        public TableTriggers Clone(Table parentObject)
        {
            TableTriggers options = new TableTriggers(parentObject);
            for (int index = 0; index < this.Count; index++)
            {
                options.Add(this[index].Clone(parentObject));
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

        public TableTrigger this[string name]
        {
            get { return (TableTrigger)hash[name]; }
            set
            {
                hash[name] = value;
                for (int index = 0; index < base.Count; index++)
                {
                    if (((TableTrigger)base[index]).FullName.Equals(name))
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
        public new void Add(TableTrigger trigger)
        {
            if (trigger != null)
            {
                hash.Add(trigger.FullName, trigger);
                base.Add(trigger);
            }
            else
                throw new ArgumentNullException("trigger");
        }

        public string ToXML()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<TRIGGERS>\n");
            for (int index = 0; index < this.Count; index++)
            {
                xml.Append(this[index].ToXML() + "\n");
            }
            xml.Append("</TRIGGERS>\n");
            return xml.ToString();
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            for (int index = 0; index < this.Count; index++)
            {
                sql.Append(this[index].ToSQL());
            }
            return sql.ToString();
        }

        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList list = new SQLScriptList();
            int index;
            for (index = 0; index < this.Count; index++)
            {
                if (this[index].Status == StatusEnum.ObjectStatusType.DropStatus)
                    list.Add(this[index].ToSQLDrop(),parent.DependenciesCount, StatusEnum.ScripActionType.DropTrigger);
                if (this[index].Status == StatusEnum.ObjectStatusType.CreateStatus)
                    list.Add(this[index].ToSQL(), parent.DependenciesCount, StatusEnum.ScripActionType.AddTrigger);
                if (this[index].Status == StatusEnum.ObjectStatusType.AlterStatus)
                {
                    list.Add(this[index].ToSQLDrop(), parent.DependenciesCount, StatusEnum.ScripActionType.DropTrigger);
                    list.Add(this[index].ToSQL(), parent.DependenciesCount, StatusEnum.ScripActionType.AddTrigger);
                }
            }
            return list;
        }
    }
}
