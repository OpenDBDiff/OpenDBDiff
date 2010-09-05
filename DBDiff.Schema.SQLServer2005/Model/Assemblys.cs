using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Assemblys : FindBaseList<Assembly, Database>
    {
        public Assemblys(Database parent)
            : base(parent)
        {
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            this.ForEach(item => sql.Append(item.ToSQL()));

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
