using System;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Columns<T> : SchemaList<Column, T> where T : ISchemaBase
    {
        public Columns(T parent) : base(parent)
        {
        }

        /// <summary>
        /// Clona el objeto Columns en una nueva instancia.
        /// </summary>
        public new Columns<T> Clone(T parentObject)
        {
            Columns<T> columns = new Columns<T>(parentObject);
            for (int index = 0; index < this.Count; index++)
            {
                columns.Add(this[index].Clone(parentObject));
            }
            return columns;
        }

        public override string ToSql()
        {
            StringBuilder sql = new StringBuilder();
            for (int index = 0; index < this.Count; index++)
            {
                // Add the coloumn if it's not in DropStatus
                if (!this[index].HasState(Enums.ObjectStatusType.DropStatus))
                {
                    sql.Append("\t" + this[index].ToSql(true));
                    if (index != this.Count - 1)
                    {
                        sql.Append(",");
                        sql.Append("\r\n");
                    }
                }
            }
            return sql.ToString();
        }

        public override SQLScriptList ToSqlDiff()
        {
            string sqlDrop = "";
            string sqlAdd = "";
            string sqlCons = "";
            string sqlBinds = "";
            SQLScriptList list = new SQLScriptList();
            if (Parent.Status != Enums.ObjectStatusType.RebuildStatus)
            {
                this.ForEach(item =>
                {
                    if (item.HasState(Enums.ObjectStatusType.DropStatus))
                    {
                        if (item.DefaultConstraint != null)
                            list.Add(item.DefaultConstraint.Drop());
                        /*Si la columna formula debe ser eliminada y ya fue efectuada la operacion en otro momento, no
                         * se borra nuevamente*/
                        if (!item.GetWasInsertInDiffList(Enums.ScripActionType.AlterColumnFormula))
                            sqlDrop += "[" + item.Name + "],";
                    }
                    if (item.HasState(Enums.ObjectStatusType.CreateStatus))
                        sqlAdd += "\r\n" + item.ToSql(true) + ",";
                    if ((item.HasState(Enums.ObjectStatusType.AlterStatus) || (item.HasState(Enums.ObjectStatusType.RebuildDependenciesStatus))))
                    {
                        if ((!item.Parent.HasState(Enums.ObjectStatusType.RebuildDependenciesStatus) || (!item.Parent.HasState(Enums.ObjectStatusType.RebuildStatus))))
                            list.AddRange(item.RebuildSchemaBindingDependencies());
                        list.AddRange(item.RebuildConstraint(false));
                        list.AddRange(item.RebuildDependencies());
                        list.AddRange(item.Alter(Enums.ScripActionType.AlterTable));
                    }
                    if (item.HasState(Enums.ObjectStatusType.UpdateStatus))
                        list.Add("UPDATE " + Parent.FullName + " SET [" + item.Name + "] = " + item.DefaultForceValue + " WHERE [" + item.Name + "] IS NULL\r\nGO\r\n", 0, Enums.ScripActionType.UpdateTable);
                    if (item.HasState(Enums.ObjectStatusType.BindStatus))
                    {
                        if (item.Rule.Id != 0)
                            sqlBinds += item.Rule.ToSQLAddBind();
                        if (item.Rule.Id == 0)
                            sqlBinds += item.Rule.ToSQLAddUnBind();
                    }
                    if (item.DefaultConstraint != null)
                        list.AddRange(item.DefaultConstraint.ToSqlDiff());
                });
                if (!String.IsNullOrEmpty(sqlDrop))
                    sqlDrop = "ALTER TABLE " + Parent.FullName + " DROP COLUMN " + sqlDrop.Substring(0, sqlDrop.Length - 1) + "\r\nGO\r\n";
                if (!String.IsNullOrEmpty(sqlAdd))
                    sqlAdd = "ALTER TABLE " + Parent.FullName + " ADD " + sqlAdd.Substring(0, sqlAdd.Length - 1) + "\r\nGO\r\n";

                if (!String.IsNullOrEmpty(sqlDrop + sqlAdd + sqlCons + sqlBinds))
                    list.Add(sqlDrop + sqlAdd + sqlBinds, 0, Enums.ScripActionType.AlterTable);
            }
            else
            {
                this.ForEach(item =>
                {
                    if (item.Status != Enums.ObjectStatusType.OriginalStatus)
                        item.RootParent.ActionMessage[item.Parent.FullName].Add(item);
                });
            }
            return list;
        }
    }
}
