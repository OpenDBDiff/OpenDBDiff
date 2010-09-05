using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.Sybase.Model
{
    public class Tables:List<Table> 
    {
        private Hashtable hash = new Hashtable();
        private Database parent;
        private string sqlScript;

        public Tables(Database parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Devuelve la base perteneciente a la coleccion de tablas.
        /// </summary>
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
            if (table != null)
            {
                hash.Add(table.FullName, table);
                base.Add(table);
            }
            else
                throw new ArgumentNullException("table");
        }

        public string ToSQL()
        {
            if (sqlScript == null)
            {
                StringBuilder sql = new StringBuilder();
                for (int index = 0; index < this.Count; index++)
                {
                    sql.Append(this[index].ToSQL());
                }
                sqlScript = sql.ToString();
            }
            return sqlScript;
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
            /*Constraints constraints = ((Database)Parent).ConstraintDependencies.FindNotOwner(tableId);
            for (int index = 0; index < constraints.Count; index++)
            {
                Constraint cons = constraints[index];
                relationalTableId = constraints[index].RelationalTableId; //((Table)constraints[index].Parent).Id;
                if ((cons.Type == Constraint.ConstraintType.ForeignKey) && (relationalTableId == tableId))
                    count += 1+FindDependenciesCount(((Table)cons.Parent).Id);
            }*/
            return count;
        }
    }
}
