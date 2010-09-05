using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Triggers:FindBaseList<Trigger,ISchemaBase>
    {
        public Triggers(ISchemaBase parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Clona el objeto Triggers en una nueva instancia.
        /// </summary>
        public Triggers Clone(ISchemaBase parentObject)
        {
            Triggers options = new Triggers(parentObject);
            for (int index = 0; index < this.Count; index++)
            {
                options.Add(this[index].Clone(parentObject));
            }
            return options;
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            this.ForEach(item => sql.Append(item.ToSql() + "\r\n"));
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
