using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Rule : SQLServerSchemaBase
    {
        private string value;

        public Rule(ISchemaBase parent)
            : base(StatusEnum.ObjectTypeEnum.Rule)
        {
            this.Parent = parent;
        }

        public Rule Clone(ISchemaBase parent)
        {
            Rule item = new Rule(parent);
            item.Id = this.Id;
            item.Name = this.Name;
            item.Owner = this.Owner;
            item.Value = this.Value;
            item.Guid = this.Guid;
            return item;
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public string ToSQL()
        {
            if ((value.Substring(value.Length -1,1).Equals("\n")) || (value.Substring(value.Length -1,1).Equals(" ")))
                return Value + "GO\r\n";
            else
                return Value + "\r\nGO\r\n";
        }

        public override string ToSQLDrop()
        {
            return "DROP RULE " + FullName + "\r\nGO\r\n";
        }

        public override string ToSQLAdd()
        {
            return ToSQL();
        }

        public string ToSQLAddBind()
        {
            string sql = "";
            sql += "EXEC sp_bindrule N'" + Name + "', N'" + this.Parent.Name + "','futureonly'\r\nGO\r\n";
            return sql;
        }

        public string ToSQLAddUnBind()
        {
            string sql = "";
            sql += "EXEC sp_unbindrule @objname=N'" + this.Parent.Name + "'\r\nGO\r\n";
            return sql;
        }

        private SQLScriptList ToSQLUnBindAll()
        {
            SQLScriptList listDiff = new SQLScriptList();
            UserDataTypes useDataTypes = ((Database)this.Parent).UserTypes.FindRules(this.FullName);
            foreach (UserDataType item in useDataTypes)
            {
                foreach (Column column in item.ColumnsDependencies)
                {
                    if (!column.IsComputed)
                        listDiff.Add("EXEC sp_unbindrule '" + column.FullName + "'\r\nGO\r\n", 0, StatusEnum.ScripActionType.UnbindRuleColumn);
                }
                if (item.Rule.Status != StatusEnum.ObjectStatusType.CreateStatus)
                    listDiff.Add("EXEC sp_unbindrule '" + item.FullName + "'\r\nGO\r\n", 0, StatusEnum.ScripActionType.UnbindRuleType);
            }
            return listDiff;
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == StatusEnum.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSQLUnBindAll());
                listDiff.Add(ToSQLDrop(), 0, StatusEnum.ScripActionType.DropRule);
            }
            if (this.Status == StatusEnum.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSQL(), 0, StatusEnum.ScripActionType.AddRule);
            }
            if (this.Status == StatusEnum.ObjectStatusType.AlterStatus)
            {
                listDiff.Add(ToSQLUnBindAll());
                listDiff.Add(ToSQLDrop(), 0, StatusEnum.ScripActionType.DropRule);
                listDiff.Add(ToSQL(), 0, StatusEnum.ScripActionType.AddRule);
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
            if (!origen.Value.Equals(destino.Value)) return false;            
            return true;
        }
    }
}