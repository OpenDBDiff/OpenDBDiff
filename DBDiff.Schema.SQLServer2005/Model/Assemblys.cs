using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Assemblys : FindBaseList<Assembly, Database>
    {
        public Assemblys(Database parent)
            : base(parent, parent.AllObjects)
        {
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
