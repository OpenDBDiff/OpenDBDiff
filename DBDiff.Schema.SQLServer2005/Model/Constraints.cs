using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Constraints : FindBaseList<Constraint,Table> 
    {
        public Constraints(Table parent)
            : base(parent)
        {
        }

        public new Constraint this[string name]
        {
            get 
            { 
                return Find(delegate(Constraint item) { return item.FullName.Equals(name); }); 
            }
            set
            {
                for (int index = 0; index < base.Count; index++)
                {
                    if (((Constraint)base[index]).FullName.Equals(name))
                    {
                        if (Parent != null)
                        {
                            ((Database)Parent.Parent).Dependencies.Set(Parent.Id, value);
                        }
                        base[index] = value;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Devuelve una coleccion de objetos Index asociados a la constraint.
        /// </summary>
        public Indexes Indexes
        {
            get
            {
                Indexes lista = new Indexes(this.Parent);
                for (int index = 0; index < this.Count; index++)
                {
                    if (this[index].Index != null)
                    {
                        lista.Add(this[index].Index);
                    }
                }
                return lista;
            }
        }

        public void AddRange(Constraints constraints)
        {
            if (constraints != null)
            {
                foreach (Constraint con in constraints)
                {
                    this.Add(con);
                }
            }
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            for (int index = 0; index < this.Count; index++)
            {
                if (this[index].Type != Constraint.ConstraintType.Check)
                {
                    sql.Append("\t" + this[index].ToSQL());
                    if (index != this.Count - 1)
                        sql.Append(",");
                    sql.Append("\r\n");
                }

            }
            return sql.ToString();
        }

        public string ToSQLCheck()
        {
            StringBuilder sql = new StringBuilder();
            for (int index = 0; index < this.Count; index++)
            {
                if (this[index].Type == Constraint.ConstraintType.Check)
                {
                    sql.Append(this[index].ToSQLAdd());
                    sql.Append("\r\n");
                }

            }
            return sql.ToString();
        }

        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();
            this.ForEach(item => listDiff.Add(item.ToSQLDiff()));

            return listDiff;
        }
    }
}
