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
            string sqlAlter = "";
            string sqlCons = "";            
            SQLScriptList list = new SQLScriptList();
            if (Parent.Status != StatusEnum.ObjectStatusType.AlterRebuildStatus)
            {
                this.ForEach(item =>
                {
                    if (item.Status == StatusEnum.ObjectStatusType.DropStatus)
                        sqlDrop += "[" + item.Name + "],";
                    if (item.Status == StatusEnum.ObjectStatusType.CreateStatus)
                        sqlAdd += "\r\n" + item.ToSQL(true) + ",";
                    if ((item.Status == StatusEnum.ObjectStatusType.AlterStatus) || (item.Status == StatusEnum.ObjectStatusType.AlterRebuildDependeciesStatus))
                        sqlAlter += "ALTER TABLE " + Parent.FullName + " ALTER COLUMN " + item.ToSQL(false) + "\r\nGO\r\n";
                    if (item.Status == StatusEnum.ObjectStatusType.AlterStatusUpdate)
                    {
                        sqlAlter += "UPDATE " + Parent.FullName + " SET [" + item.Name + "] = " + item.DefaultForceValue + " WHERE [" + item.Name + "] IS NULL\r\nGO\r\n";
                        sqlAlter += "ALTER TABLE " + Parent.FullName + " ALTER COLUMN " + item.ToSQL(false) + "\r\nGO\r\n";
                    }
                    if (item.Constraints.Count > 0)
                        list.Add(item.Constraints.ToSQLDiff());
                });
                if (!String.IsNullOrEmpty(sqlDrop))
                    sqlDrop = "ALTER TABLE " + Parent.FullName + " DROP COLUMN " + sqlDrop.Substring(0, sqlDrop.Length - 1) + "\r\nGO\r\n";
                if (!String.IsNullOrEmpty(sqlAdd))
                    sqlAdd = "ALTER TABLE " + Parent.FullName + " ADD " + sqlAdd.Substring(0, sqlAdd.Length - 1) + "\r\nGO\r\n";

                if (!String.IsNullOrEmpty(sqlAlter + sqlDrop + sqlAdd + sqlCons))
                    list.Add(sqlAlter + sqlDrop + sqlAdd, 0, StatusEnum.ScripActionType.AlterTable);
            }
            return list;
        }
    }
}
