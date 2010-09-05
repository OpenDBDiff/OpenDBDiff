using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Views : FindBaseList<View, Database>
    {
        public Views(Database parent) : base(parent)
        {
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            for (int index = 0; index < this.Count; index++)
            {
                sql.Append(this[index].ToSQL() + "\r\n");
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
