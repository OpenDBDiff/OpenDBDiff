using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema
{
    public class SQLScriptList
    {
        private List<SQLScript> list;

        public void Sort()
        {
            if (list != null) list.Sort();
        }

        public void Add(SQLScriptList items)
        {
            for (int j = 0; j < items.Count; j++)
            {
                if (list == null) list = new List<SQLScript>();
                list.Add(items[j]);
            }
        }

        public void Add(string SQL, int dependencies, StatusEnum.ScripActionType type)
        {
            SQLScript script;
            if (list == null) list = new List<SQLScript>();
            script = new SQLScript();
            script.Dependencies = dependencies;
            script.SQL = SQL;
            script.Status = type;
            list.Add(script);
        }

        public int Count
        {
            get { return (list==null)?0:list.Count; }
        }

        public SQLScript this[int index]
        {
            get { return list[index]; }
        }

        public string ToSQL()
        {
            string sql = "";
            this.Sort(); /*Ordena la lista antes de generar el script*/
            if (list != null)
            {                
                for (int j = 0; j < list.Count; j++)
                {
                    sql += list[j].SQL;
                }
            }
            return sql;
        }
    }
}
