using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class ColumnConstraints : List<ColumnConstraint>
    {
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
        public Boolean Exists(string name)
        {
            return Exists(delegate(ColumnConstraint item) { return item.FullName.Equals(name); });
        }

        /// <summary>
        /// Clona el objeto ColumnConstraints en una nueva instancia.
        /// </summary>
        public ColumnConstraints Clone(Column parentObject)
        {
            ColumnConstraints columns = new ColumnConstraints(parentObject);
            for (int index = 0; index < this.Count; index++)
            {
                columns.Add(this[index].Clone(parentObject));
            }
            return columns;
        }

        public ColumnConstraint this[string name]
        {
            get 
            {
                return Find(delegate(ColumnConstraint item) { return item.FullName.Equals(name); }); 
            }
            set
            {
                for (int index = 0; index < base.Count; index++)
                {
                    if (((ColumnConstraint)base[index]).FullName.Equals(name))
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
        public new void Add(ColumnConstraint columnConstraint)
        {
            if (columnConstraint != null)
            {
                base.Add(columnConstraint);
                ((Database)parent.Parent.Parent).AllObjects.Add(columnConstraint);
                /*if (!((Database)parent.Parent.Parent).AllObjects.ContainsKey(columnConstraint.FullName.ToUpper()))
                    ((Database)parent.Parent.Parent).AllObjects.Add(columnConstraint.FullName.ToUpper(), columnConstraint.ObjectType);*/
            }
            else
                throw new ArgumentNullException("columnConstraint");
        }

        public string ToXML()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<COLUMNCONSTRAINT>\n");
            for (int index = 0; index < this.Count; index++)
            {
                xml.Append(this[index].ToXML() + "\n");
            }
            xml.Append("</COLUMNCONSTRAINT>\n");
            return xml.ToString();
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            for (int index = 0; index < this.Count; index++)
            {
                sql.Append("\t" + this[index].ToSql());
                if (index != this.Count - 1)
                    sql.Append(",");
            }
            return sql.ToString();
        }
    }
}
