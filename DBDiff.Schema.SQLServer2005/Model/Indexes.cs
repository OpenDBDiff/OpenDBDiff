using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Indexes : FindBaseList<Index, ISchemaBase>
    {
        public Indexes(ISchemaBase parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Clona el objeto Triggers en una nueva instancia.
        /// </summary>
        public Indexes Clone(ISchemaBase parentObject)
        {
            Indexes options = new Indexes(parentObject);
            for (int index = 0; index < this.Count; index++)
            {
                options.Add(this[index].Clone(parentObject));
            }
            return options;
        }

        public Index Find(Index.IndexTypeEnum type)
        {
            return Find(delegate(Index item) { return item.Type == type; });
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            this.ForEach(item => sql.Append(item.ToSql()));
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
