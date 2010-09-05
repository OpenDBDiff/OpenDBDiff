using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Constraints<T> : SchemaList<Constraint,T> where T:ISchemaBase
    {
        public Constraints(T parent)
            : base(parent, ((parent == null || parent.Parent == null)?null:((Database)parent.Parent).AllObjects))
        {            
        }

        public string ToSql(Constraint.ConstraintType type)
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
