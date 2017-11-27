using System;
using System.Collections;
using System.Collections.Generic;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Rule : Code
    {
        public Rule(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Rule, Enums.ScripActionType.AddRule, Enums.ScripActionType.DropRule)
        {
        }

        public Rule Clone(ISchemaBase parent)
        {
            Rule item = new Rule(parent);
            item.Id = this.Id;
            item.Name = this.Name;
            item.Owner = this.Owner;
            item.Text = this.Text;
            item.Guid = this.Guid;
            return item;
        }

        public string ToSQLAddBind()
        {
            string sql;
            if (this.Parent.ObjectType == Enums.ObjectType.Column)
                sql = String.Format("EXEC sp_bindrule N'{0}', N'[{1}].[{2}]','futureonly'\r\nGO\r\n", Name, this.Parent.Parent.Name, this.Parent.Name);
            else
                sql = String.Format("EXEC sp_bindrule N'{0}', N'{1}','futureonly'\r\nGO\r\n", Name, this.Parent.Name);
            return sql;
        }

        public string ToSQLAddUnBind()
        {
            string sql;
            if (this.Parent.ObjectType == Enums.ObjectType.Column)
                sql = String.Format("EXEC sp_unbindrule @objname=N'[{0}].[{1}]'\r\nGO\r\n", this.Parent.Parent.Name, this.Parent.Name);
            else
                sql = String.Format("EXEC sp_unbindrule @objname=N'{0}'\r\nGO\r\n", this.Parent.Name);
            return sql;
        }

        private SQLScriptList ToSQLUnBindAll()
        {
            SQLScriptList listDiff = new SQLScriptList();
            Hashtable items = new Hashtable();
            List<UserDataType> useDataTypes = ((Database)this.Parent).UserTypes.FindAll(item => { return item.Rule.FullName.Equals(this.FullName); });
            foreach (UserDataType item in useDataTypes)
            {
                foreach (ObjectDependency dependency in item.Dependencys)
                {
                    Column column = ((Database)this.Parent).Tables[dependency.Name].Columns[dependency.ColumnName];
                    if ((!column.IsComputed) && (column.Status != Enums.ObjectStatusType.CreateStatus))
                    {
                        if (!items.ContainsKey(column.FullName))
                        {
                            listDiff.Add("EXEC sp_unbindrule '" + column.FullName + "'\r\nGO\r\n", 0, Enums.ScripActionType.UnbindRuleColumn);
                            items.Add(column.FullName, column.FullName);
                        }
                    }
                }
                if (item.Rule.Status != Enums.ObjectStatusType.CreateStatus)
                    listDiff.Add("EXEC sp_unbindrule '" + item.FullName + "'\r\nGO\r\n", 0, Enums.ScripActionType.UnbindRuleType);
            }
            return listDiff;
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.AddRange(ToSQLUnBindAll());
                listDiff.Add(Drop());
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
                listDiff.Add(Create());
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
            {
                listDiff.AddRange(ToSQLUnBindAll());
                listDiff.AddRange(Rebuild());
            }
            return listDiff;
        }
    }
}
