using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Model.Util;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Rule : Code
    {
        public Rule(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Rule)
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
                sql = String.Format("EXEC sp_bindrule N'{0}', N'[{1}].[{2}]','futureonly'\r\nGO\r\n", Name, this.Parent.Parent.Name,this.Parent.Name);
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
            UserDataTypes useDataTypes = ((Database)this.Parent).UserTypes.FindRules(this.FullName);
            foreach (UserDataType item in useDataTypes)
            {
                foreach (ObjectDependency dependency in item.Dependencys)
                {
                    Column column = ((Database)this.Parent).Tables[dependency.Name].Columns[dependency.ColumnName];
                    if (!column.IsComputed)
                        listDiff.Add("EXEC sp_unbindrule '" + column.FullName + "'\r\nGO\r\n", 0, Enums.ScripActionType.UnbindRuleColumn);
                }
                if (item.Rule.Status != Enums.ObjectStatusType.CreateStatus)
                    listDiff.Add("EXEC sp_unbindrule '" + item.FullName + "'\r\nGO\r\n", 0, Enums.ScripActionType.UnbindRuleType);
            }
            return listDiff;
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.AddRange(ToSQLUnBindAll());
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropRule);
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddRule);
            }
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
            {
                listDiff.AddRange(ToSQLUnBindAll());
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropRule);
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddRule);
            }
            return listDiff;
        }

        /// <summary>
        /// Compara dos Rules y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Rule origen, Rule destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.ToSql().Equals(destino.ToSql())) return false;            
            return true;
        }
    }
}