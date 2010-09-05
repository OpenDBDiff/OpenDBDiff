using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.MySQL.Model
{
    public class Constraints:List<Constraint> 
    {
        private Hashtable hash = new Hashtable();
        private Table parent;
        
        public Constraints(Table parent)
        {
            this.parent = parent;
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
        /// Clona el objeto Columns en una nueva instancia.
        /// </summary>
        public Constraints Clone(Table parentObject)
        {
            Constraints columns = new Constraints(parentObject);
            for (int index = 0; index < this.Count; index++)
            {
                columns.Add(this[index].Clone(parentObject));
            }
            return columns;
        }

        public Constraint this[string name]
        {
            get { return (Constraint)hash[name]; }
            set
            {
                hash[name] = value;
                for (int index = 0; index < base.Count; index++)
                {
                    if (((Constraint)base[index]).Name.Equals(name))
                    {
                        base[index] = value;
                        break;
                    }
                }
            }
        }

        public Constraint Get(string columnName)
        {
            for (int index = 0; index < this.Count; index++)
            {
                if (this[index].Columns[0].Name.Equals(columnName))
                    return this[index];
            }
            return null;
        }

        public new void Add(Constraint column)
        {
            if (column != null)
            {
                if (!hash.ContainsKey(column.Name))
                {
                    hash.Add(column.Name, column);
                    base.Add(column);
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

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            for (int index = 0; index < this.Count; index++)
            {
                sql.Append("\t" + this[index].ToSQL());
                if (index != this.Count - 1)
                    sql.Append(",");
                sql.Append("\r\n");
            }
            return sql.ToString();
        }

        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList list = new SQLScriptList();
            int index;
            for (index = 0; index < this.Count; index++)
            {
                StatusEnum.ScripActionType actionDrop = StatusEnum.ScripActionType.DropConstraint;
                StatusEnum.ScripActionType actionAdd = StatusEnum.ScripActionType.AddConstraint;

                if (this[index].Type == Constraint.ConstraintType.ForeignKey)
                {
                    actionDrop = StatusEnum.ScripActionType.DropConstraintFK;
                    actionAdd = StatusEnum.ScripActionType.AddConstraintFK;
                }
                if (this[index].Type == Constraint.ConstraintType.PrimaryKey)
                {
                    actionAdd = StatusEnum.ScripActionType.AddConstraintPK;
                    actionDrop = StatusEnum.ScripActionType.DropConstraintPK;
                }

                if (this[index].Status == StatusEnum.ObjectStatusType.DropStatus)
                    list.Add(this[index].ToSQLDrop(), parent.DependenciesCount, actionDrop);
                if (this[index].Status == StatusEnum.ObjectStatusType.CreateStatus)
                    list.Add(this[index].ToSQLAdd(), parent.DependenciesCount, actionAdd);
                if (this[index].Status == StatusEnum.ObjectStatusType.AlterStatus)
                {
                    list.Add(this[index].ToSQLDrop(), parent.DependenciesCount, actionDrop);
                    list.Add(this[index].ToSQLAdd(), parent.DependenciesCount, actionAdd);
                }
            }
            return list;
        }
    }
}
