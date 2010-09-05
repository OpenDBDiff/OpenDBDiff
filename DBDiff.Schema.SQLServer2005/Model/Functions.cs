using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Functions : FindBaseList<Function,Database> 
    {
        public Functions(Database parent)
            : base(parent, parent.AllObjects)
        {
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
