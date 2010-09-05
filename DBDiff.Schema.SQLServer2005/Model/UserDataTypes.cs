using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class UserDataTypes : FindBaseList<UserDataType,Database> 
    {
        public UserDataTypes(Database parent)
            : base(parent)
        {
        }
       
        public UserDataTypes FindRules(string RuleFullName)
        {
            UserDataTypes items = new UserDataTypes(Parent);
            foreach (UserDataType item in this)
            {
                if (item.Rule.FullName.Equals(RuleFullName))
                    items.Add(item);
            }
            return items;
        }

        public string ToSQLChangeColumns()
        {
            Hashtable fields = new Hashtable();
            string originalType;
            int originalSize;
            StringBuilder sql = new StringBuilder();
            foreach (UserDataType type in this)
            {
                if ((type.Status == StatusEnum.ObjectStatusType.AlterStatus) || (type.Status == StatusEnum.ObjectStatusType.AlterRebuildStatus))
                {
                    foreach (Column col in type.ColumnsDependencies)
                    {
                        if (!fields.ContainsKey(col.FullName))
                        {
                            if (!col.IsComputed)
                            {
                                originalType = col.Type;
                                originalSize = col.Size;
                                col.Type = type.Type;
                                col.Size = type.Size;
                                sql.Append("ALTER TABLE " + col.Parent.FullName + " ALTER COLUMN " + col.ToSQL(false) + "\r\nGO\r\n");
                                col.Type = originalType;
                                col.Size = originalSize;
                            }
                            else
                                sql.Append(col.ToSQLDrop());
                            fields.Add(col.FullName, col.FullName);
                        }
                    }
                }
            }
            return sql.ToString();
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            this.ForEach(item => sql.Append(item.ToSQL() + "\r\n"));
            return sql.ToString();
        }

        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList list = new SQLScriptList();
            string sql = "";
            int index;            
            for (index = 0; index < this.Count; index++)
            {
                if (this[index].Status == StatusEnum.ObjectStatusType.DropStatus)
                    list.Add(this[index].ToSQLDrop(), 0, StatusEnum.ScripActionType.DropUserDataType); 
                if (this[index].Status == StatusEnum.ObjectStatusType.CreateStatus)
                    list.Add(this[index].ToSQL(), 0, StatusEnum.ScripActionType.AddUserDataType);
                if (this[index].Status == StatusEnum.ObjectStatusType.AlterStatus)
                {
                    if (this[index].Default.Status == StatusEnum.ObjectStatusType.CreateStatus)
                        list.Add(this[index].Default.ToSQLAddBind(), 0, StatusEnum.ScripActionType.AddUserDataType);
                    if (this[index].Default.Status == StatusEnum.ObjectStatusType.DropStatus)
                        list.Add(this[index].Default.ToSQLAddUnBind(), 0, StatusEnum.ScripActionType.UnbindRuleType);
                    if (this[index].Rule.Status == StatusEnum.ObjectStatusType.CreateStatus)
                        list.Add(this[index].Rule.ToSQLAddBind(), 0, StatusEnum.ScripActionType.AddUserDataType);
                    if (this[index].Rule.Status == StatusEnum.ObjectStatusType.DropStatus)
                        list.Add(this[index].Rule.ToSQLAddUnBind(), 0, StatusEnum.ScripActionType.UnbindRuleType); 
                }
                if (this[index].Status == StatusEnum.ObjectStatusType.AlterRebuildStatus)
                    sql += this[index].ToSQLDrop() + this[index].ToSQL();
            }
            list.Add(ToSQLChangeColumns() + sql,0, StatusEnum.ScripActionType.AddUserDataType);
            return list;
        }
    }
}
