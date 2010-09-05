using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Synonyms : FindBaseList<Synonym,Database>
    {
        public Synonyms(Database parent)
            : base(parent)
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
