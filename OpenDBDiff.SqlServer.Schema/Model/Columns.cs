using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Linq;

namespace OpenDBDiff.SqlServer.Schema.Model
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
            return string.Join
            (
                ",\r\n",
                this
                    .Where(c => !c.HasState(ObjectStatus.Drop))
                    .Select(c => "\t" + c.ToSql(true))
            );
        }

        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            string sqlDrop = "";
            string sqlAdd = "";
            string sqlCons = "";
            string sqlBinds = "";
            SQLScriptList list = new SQLScriptList();
            if (Parent.Status != ObjectStatus.Rebuild)
            {
                this.ForEach(item =>
                {
                    bool isIncluded = schemas.Count == 0;
                    if (!isIncluded)
                    {
                        foreach (var selectedSchema in schemas)
                        {
                            if (selectedSchema.Id == item.Id)
                            {
                                isIncluded = true;
                                break;
                            }
                        }
                    }
                    if (isIncluded)
                    {
                        if (item.HasState(ObjectStatus.Drop))
                        {
                            if (item.DefaultConstraint != null)
                                list.Add(item.DefaultConstraint.Drop());
                            /*Si la columna formula debe ser eliminada y ya fue efectuada la operacion en otro momento, no
                             * se borra nuevamente*/
                            if (!item.GetWasInsertInDiffList(ScriptAction.AlterColumnFormula))
                                sqlDrop += "[" + item.Name + "],";
                        }
                        if (item.HasState(ObjectStatus.Create))
                            sqlAdd += "\r\n" + item.ToSql(true) + ",";
                        if ((item.HasState(ObjectStatus.Alter) || (item.HasState(ObjectStatus.RebuildDependencies))))
                        {
                            if ((!item.Parent.HasState(ObjectStatus.RebuildDependencies) || (!item.Parent.HasState(ObjectStatus.Rebuild))))
                                list.AddRange(item.RebuildSchemaBindingDependencies());
                            list.AddRange(item.RebuildConstraint(false));
                            list.AddRange(item.RebuildDependencies());
                            list.AddRange(item.Alter(ScriptAction.AlterTable));
                        }
                        if (item.HasState(ObjectStatus.Update))
                            list.Add("UPDATE " + Parent.FullName + " SET [" + item.Name + "] = " + item.DefaultForceValue + " WHERE [" + item.Name + "] IS NULL\r\nGO\r\n", 0, ScriptAction.UpdateTable);
                        if (item.HasState(ObjectStatus.Bind))
                        {
                            if (item.Rule.Id != 0)
                                sqlBinds += item.Rule.ToSQLAddBind();
                            if (item.Rule.Id == 0)
                                sqlBinds += item.Rule.ToSQLAddUnBind();
                        }
                        if (item.DefaultConstraint != null)
                            list.AddRange(item.DefaultConstraint.ToSqlDiff(schemas));
                    }
                });
                if (!String.IsNullOrEmpty(sqlDrop))
                    sqlDrop = "ALTER TABLE " + Parent.FullName + " DROP COLUMN " + sqlDrop.Substring(0, sqlDrop.Length - 1) + "\r\nGO\r\n";
                if (!String.IsNullOrEmpty(sqlAdd))
                    sqlAdd = "ALTER TABLE " + Parent.FullName + " ADD " + sqlAdd.Substring(0, sqlAdd.Length - 1) + "\r\nGO\r\n";

                if (!String.IsNullOrEmpty(sqlDrop + sqlAdd + sqlCons + sqlBinds))
                    list.Add(sqlDrop + sqlAdd + sqlBinds, 0, ScriptAction.AlterTable);
            }
            else
            {
                this.ForEach(item =>
                {
                    if (item.Status != ObjectStatus.Original)
                        item.RootParent.ActionMessage[item.Parent.FullName].Add(item);
                });
            }
            return list;
        }
    }
}
