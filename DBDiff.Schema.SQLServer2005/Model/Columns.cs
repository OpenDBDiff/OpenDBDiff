using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Columns : FindBaseList<Column,Table>
    {
        public Columns(Table parent):base(parent)
        {
        }

        public void AddRange(Columns columns)
        {
            columns.ForEach(item =>
            {
                if (!this.Exists(item.FullName)) this.Add(item);
            });
        }

        /// <summary>
        /// Clona el objeto Columns en una nueva instancia.
        /// </summary>
        public Columns Clone(Table parentObject)
        {
            Columns columns = new Columns(parentObject);
            for (int index = 0; index < this.Count; index++)
            {
                columns.Add(this[index].Clone(parentObject));
            }
            return columns;
        }

        public Columns GetComputedColumns()
        {
            Columns columns = new Columns(Parent);
            foreach (Column col in this)
            {
                if (col.IsComputed)
                    columns.Add(col);
            }
            return columns;
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            for (int index = 0; index < this.Count; index++)
            {
                sql.Append("\t" + this[index].ToSQL(true));
                if (index != this.Count - 1)
                {
                    sql.Append(",");
                    sql.Append("\r\n");
                }
            }
            return sql.ToString();
        }

        public SQLScriptList ToSQLDiff()
        {
            string sqlDrop = "";
            string sqlAdd = "";
            string sqlCons = "";
            string sqlBinds = "";
            SQLScriptList list = new SQLScriptList();
            if (Parent.Status != Enums.ObjectStatusType.AlterRebuildStatus)
            {
                this.ForEach(item =>
                {
                    if (item.HasState(Enums.ObjectStatusType.DropStatus))
                        sqlDrop += "[" + item.Name + "],";
                    if (item.HasState(Enums.ObjectStatusType.CreateStatus))
                        sqlAdd += "\r\n" + item.ToSQL(true) + ",";
                    if ((item.HasState(Enums.ObjectStatusType.AlterStatus) || (item.HasState(Enums.ObjectStatusType.AlterRebuildDependenciesStatus))))
                    {
                        if ((!item.Parent.HasState(Enums.ObjectStatusType.AlterRebuildDependenciesStatus) || (!item.Parent.HasState(Enums.ObjectStatusType.AlterRebuildStatus))))
                            list.AddRange(item.RebuildSchemaBindingDependencies());
                        list.AddRange(item.RebuildConstraint(false));
                        list.AddRange(item.Alter(Enums.ScripActionType.AlterTable));
                    }
                    if (item.HasState(Enums.ObjectStatusType.UpdateStatus))
                        list.Add("UPDATE " + Parent.FullName + " SET [" + item.Name + "] = " + item.DefaultForceValue + " WHERE [" + item.Name + "] IS NULL\r\nGO\r\n",0, Enums.ScripActionType.UpdateTable);
                    if (item.HasState(Enums.ObjectStatusType.BindStatus))
                    {
                        if (item.Rule.Id != 0)
                            sqlBinds += item.Rule.ToSQLAddBind();
                        if (item.Rule.Id == 0)
                            sqlBinds += item.Rule.ToSQLAddUnBind();
                    }
                    if (item.Constraints.Count > 0)
                        list.AddRange(item.Constraints.ToSQLDiff());
                });
                if (!String.IsNullOrEmpty(sqlDrop))
                    sqlDrop = "ALTER TABLE " + Parent.FullName + " DROP COLUMN " + sqlDrop.Substring(0, sqlDrop.Length - 1) + "\r\nGO\r\n";
                if (!String.IsNullOrEmpty(sqlAdd))
                    sqlAdd = "ALTER TABLE " + Parent.FullName + " ADD " + sqlAdd.Substring(0, sqlAdd.Length - 1) + "\r\nGO\r\n";

                if (!String.IsNullOrEmpty(sqlDrop + sqlAdd + sqlCons + sqlBinds))
                    list.Add(sqlDrop + sqlAdd + sqlBinds, 0, Enums.ScripActionType.AlterTable);
            }
            return list;
        }
    }
}
