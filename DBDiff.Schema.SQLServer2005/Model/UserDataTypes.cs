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

        public Boolean ExistsAssembly(string name)
        {
            return Exists(item => item.AssemblyFullName.Equals(name) && item.IsAssembly);
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            this.ForEach(item => sql.Append(item.ToSql() + "\r\n"));
            return sql.ToString();
        }

        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();
            this.ForEach(item => listDiff.AddRange(item.ToSQLDiff()));

            return listDiff;
        }
    }
}
