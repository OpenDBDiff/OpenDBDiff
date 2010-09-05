using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    [Serializable()]
    public class Tables:List<Table> 
    {
        private Hashtable hash = new Hashtable();
        private Database parent;

        public Tables(Database parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Devuelve la base perteneciente a la coleccion de tablas.
        /// </summary>
        [XmlIgnore()]
        public Database Parent
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

        public Table this[string name]
        {
            get { return (Table)hash[name]; }
        }

        public new void Add(Table table)
        {
            hash.Add(table.FullName, table);
            base.Add(table);
        }

        public string ToXML()
        {
            string xml = "";
            int index;
            xml += "<TABLES>\n";
            for (index = 0; index < this.Count; index++)
            {
                xml += this[index].ToXML() + "\n";
            }
            xml += "</TABLES>\n";
            return xml;
        }

        public string ToSQL()
        {
            string sql = "";
            int index;
            for (index = 0; index < this.Count; index++)
            {
                sql += this[index].ToSQL() + "\r\n";
            }
            return sql;
        }

        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();
            int index;
            for (index = 0; index < this.Count; index++)
            {
                listDiff.Add(this[index].ToSQLDiff());
            }
            return listDiff;
        }

        public new void Sort()
        {
            BuildDependenciesTree();
            base.Sort(); //Ordena las tablas en funcion de su dependencias
        }

        /// <summary>
        /// Reordena la tabla en funcion de sus dependencias.
        /// </summary>
        private void BuildDependenciesTree()
        {
            int index;
            for (index = 0; index < this.Count; index++)
            {
                this[index].DependenciesCount += FindDependenciesCount(this[index].Id);
            }
        }

        private int FindDependenciesCount(int tableId)
        {
            int count = 0;
            int relationalTableId;
            Constraints constraints = ((Database)Parent).Dependencies.FindNotOwner(tableId);
            for (int index = 0; index < constraints.Count; index++)
            {
                Constraint cons = constraints[index];
                relationalTableId = constraints[index].RelationalTableId; //((Table)constraints[index].Parent).Id;
                if ((cons.Type == Constraint.ConstraintType.ForeignKey) && (relationalTableId == tableId))
                    count += FindDependenciesCount(cons.Parent.Id);
            }
            return count;
        }
    }
}
