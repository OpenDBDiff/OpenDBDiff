using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Rules : FindBaseList<Rule,Database>
    {
        public Rules(Database parent)
            : base(parent)
        {
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            this.ForEach(item => sql.Append(item.ToSQL() + "\r\n"));
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
