using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    public class ColumnConstraints : List<ColumnConstraint>
    {
        private Hashtable hash = new Hashtable();
        private Column parent;

        public ColumnConstraints(Column parent)
        {
            this.parent = parent;
        }

        public Column Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>
        /// Indica si el nombre de la constraint existe en la coleccion de constraints del objeto.
        /// </summary>
        /// <param name="table">
        /// Nombre de la constraint a buscar.
        /// </param>
        /// <returns></returns>
        public Boolean Find(string name)
        {
            return hash.ContainsKey(name);
        }

        /// <summary>
        /// Clona el objeto ColumnConstraints en una nueva instancia.
        /// </summary>
        public ColumnConstraints Clone(Column parent)
        {
            ColumnConstraints columns = new ColumnConstraints(parent);
            for (int index = 0; index < this.Count; index++)
            {
                columns.Add(this[index].Clone(parent));
            }
            return columns;
        }

        public ColumnConstraint this[string name]
        {
            get { return (ColumnConstraint)hash[name]; }
            set
            {
                hash[name] = value;
                for (int index = 0; index < base.Count; index++)
                {
                    if (((ColumnConstraint)base[index]).Name.Equals(name))
                    {
                        base[index] = value;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Agrega un objeto ColumnConstraints a la coleccion de ColumnConstraintss.
        /// </summary>
        public new void Add(ColumnConstraint column)
        {
            hash.Add(column.Name, column);
            base.Add(column);
        }

        public string ToXML()
        {
            string xml = "";
            int index;
            xml += "<COLUMNCONSTRAINTS>\n";
            for (index = 0; index < this.Count; index++)
            {
                xml += this[index].ToXML() + "\n";
            }
            xml += "</COLUMNCONSTRAINTS>\n";
            return xml;
        }

        public string ToSQL()
        {
            string sql = "";
            int index;
            for (index = 0; index < this.Count; index++)
            {
                sql += this[index].ToSQL();
                if (index != this.Count-1) 
                {
                    sql += ",";
                    sql += "\r\n";
                }
            }
            return sql;
        }

        public string ToSQLDiff()
        {
            string sql = "";
            int index;
            for (index = 0; index < this.Count; index++)
            {
                if (this[index].Status == StatusEnum.ObjectStatusType.DropStatus)
                    sql += this[index].ToSQLDrop();
                if (this[index].Status == StatusEnum.ObjectStatusType.CreateStatus)
                    sql += this[index].ToSQLAdd();
                if (this[index].Status == StatusEnum.ObjectStatusType.AlterStatus)
                {
                    sql += this[index].ToSQLDrop();
                    sql += this[index].ToSQLAdd();
                }
            }
            return sql;
        }
    }
}
