using System;
using System.Collections.Generic;
using System.Text;
using OpenDBDiff.Schema.Model;

namespace OpenDBDiff.Schema
{
    public class SQLScriptList
    {
        private List<SQLScript> list;

        public void Sort()
        {
            if (list != null) list.Sort();
        }

        public void Add(SQLScript item, int deep)
        {
            if (list == null) list = new List<SQLScript>();
            if (item != null)
            {
                item.Deep = deep;
                list.Add(item);
            }
        }

        public void Add(SQLScript item)
        {
            if (list == null) list = new List<SQLScript>();
            if (item != null) list.Add(item);
        }

        public void Add(string SQL, int dependencies, ScriptAction type)
        {
            if (list == null) list = new List<SQLScript>();
            list.Add(new SQLScript(SQL, dependencies, type));
        }

        public void AddRange(SQLScriptList items)
        {
            for (int j = 0; j < items.Count; j++)
            {
                if (list == null) list = new List<SQLScript>();
                list.Add(items[j]);
            }
        }

        public int Count
        {
            get { return (list == null) ? 0 : list.Count; }
        }

        public SQLScript this[int index]
        {
            get { return list[index]; }
        }

        /*private string ToSqlDown(SQLScript item)
        {
            string sql = "";
            for (int i = 0; i < item.Childs.Count; i++)
            {
                for (int k = 0; k < item.Childs[i].Childs.Count; k++)
                {
                    for (int h = 0; h < item.Childs[i].Childs[k].Childs.Count; h++)
                    {
                        for (int l = 0; l < item.Childs[i].Childs[k].Childs[h].Childs.Count; l++)
                        {
                            for (int m = 0; m < item.Childs[i].Childs[k].Childs[h].Childs[l].Childs.Count; m++)
                            {
                                sql += item.Childs[i].Childs[k].Childs[h].Childs[l].Childs[m].SQL;
                            }
                            sql += item.Childs[i].Childs[k].Childs[h].Childs[l].SQL;
                        }
                        sql += item.Childs[i].Childs[k].Childs[h].SQL;
                    }
                    sql += item.Childs[i].Childs[k].SQL;
                }
                sql += item.Childs[i].SQL;
            }
            sql += item.SQL;
            return sql;
        }*/

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            this.Sort(); /*Ordena la lista antes de generar el script*/
            if (list != null)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    //if ((list[j].IsDropAction) || (!list[j].IsAddAction))
                    sql.Append(list[j].SQL); //ToSqlDown(list[j]);
                }
                /*for (int j = list.Count-1; j >= 0; j--)
                {
                    if (list[j].IsAddAction)
                        sql.Append(list[j].SQL);
                }*/

            }
            return sql.ToString();
        }

        public SQLScriptList FindAlter()
        {
            SQLScriptList alter = new SQLScriptList();
            list.ForEach(item => { if ((item.Status == ScriptAction.AlterView) || (item.Status == ScriptAction.AlterFunction) || (item.Status == ScriptAction.AlterProcedure)) alter.Add(item); });
            return alter;
        }
    }

    public static class SQLScriptListExtensionMethod
    {
        public static SQLScriptList WarnMissingScript(this SQLScriptList scriptList, ISchemaBase scriptSource)
        {
            if (scriptList == null || scriptSource == null || scriptSource.Status == ObjectStatus.Original)
            {
                return scriptList;
            }

            for (int i = 0; i < scriptList.Count; ++i)
            {
                if (!String.IsNullOrEmpty(scriptList[i].SQL))
                {
                    return scriptList;
                }
            }

            scriptList.Add(String.Format("\r\n--\r\n-- DIFF-ERROR 0x{0:x8}.{1:d3}: Missing {2} script for {3} '{4}'\r\n--\r\n\r\n", (int)scriptSource.Status, (int)scriptSource.ObjectType, scriptSource.Status, scriptSource.ObjectType, scriptSource.Name), 0, ScriptAction.None);
            return scriptList;
        }
    }
}
