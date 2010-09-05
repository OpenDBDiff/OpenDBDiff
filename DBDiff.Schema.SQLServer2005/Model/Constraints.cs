using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Constraints : FindBaseList<Constraint,Table> 
    {
        public Constraints()
            : base(null)
        {
        }

        public Constraints(Table parent)
            : base(parent, ((parent == null || parent.Parent == null)?null:((Database)parent.Parent).AllObjects))
        {
        }

        public new Constraint this[string name]
        {
            get 
            {
                return Find(delegate(Constraint item) { return item.FullName.Equals(name, base.Comparion); }); 
            }
            set
            {
                for (int index = 0; index < base.Count; index++)
                {
                    if (((Constraint)base[index]).FullName.Equals(name, base.Comparion))
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

        public string ToSQL(Constraint.ConstraintType type)
        {
            StringBuilder sql = new StringBuilder();
            for (int index = 0; index < this.Count; index++)
            {
                if (this[index].Type == type)
                {
                    sql.Append("\t" + this[index].ToSql());
                    if (index != this.Count - 1)
                        sql.Append(",");
                    sql.Append("\r\n");
                }
            }
            return sql.ToString();
        }

        public string ToSQLAdd(Constraint.ConstraintType type)
        {
            StringBuilder sql = new StringBuilder();
            for (int index = 0; index < this.Count; index++)
            {
                if (this[index].Type == type)
                {
                    sql.Append(this[index].ToSqlAdd());
                    sql.Append("\r\n");
                }
            }
            return sql.ToString();
        }

        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();
            this.ForEach(item => listDiff.AddRange(item.ToSQLDiff()));

            return listDiff;
        }
    }
}
