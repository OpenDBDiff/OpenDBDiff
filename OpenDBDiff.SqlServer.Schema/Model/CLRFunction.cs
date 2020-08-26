using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections.Generic;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class CLRFunction : CLRCode
    {
        public CLRFunction(ISchemaBase parent)
            : base(parent, ObjectType.CLRFunction, ScriptAction.AddFunction, ScriptAction.DropFunction)
        {
            Parameters = new List<Parameter>();
            ReturnType = new Parameter();
        }

        public List<Parameter> Parameters { get; set; }

        public Parameter ReturnType { get; private set; }

        public override string ToSql()
        {
            string sql = "CREATE FUNCTION " + FullName + "";
            string param = "";
            Parameters.ForEach(item => param += item.ToSql() + ",");
            if (!String.IsNullOrEmpty(param))
            {
                param = param.Substring(0, param.Length - 1);
                sql += " (" + param + ")\r\n";
            }
            else
                sql += "()\r\n";
            sql += "RETURNS " + ReturnType.ToSql() + " ";
            sql += "WITH EXECUTE AS " + AssemblyExecuteAs + "\r\n";
            sql += "AS\r\n";
            sql += "EXTERNAL NAME [" + AssemblyName + "].[" + AssemblyClass + "].[" + AssemblyMethod + "]\r\n";
            sql += "GO\r\n";
            return sql;
        }

        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            SQLScriptList list = new SQLScriptList();

            if (this.HasState(ObjectStatus.Drop))
                list.Add(Drop());
            if (this.HasState(ObjectStatus.Create))
                list.Add(Create());
            if (this.Status == ObjectStatus.Alter)
            {
                list.AddRange(Rebuild());
            }
            list.AddRange(this.ExtendedProperties.ToSqlDiff());
            return list;
        }
    }
}
