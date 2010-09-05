using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Constraints : SchemaList<Constraint,Table> 
    {
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
    }
}
