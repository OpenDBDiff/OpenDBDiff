using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class CLRFunction : CLRCode
    {
        private List<Parameter> parameters;
        private Parameter returnType;

        public CLRFunction(ISchemaBase parent)
            : base(parent, Enums.ObjectType.CLRFunction, Enums.ScripActionType.AddFunction, Enums.ScripActionType.DropFunction)
        {
            parameters = new List<Parameter>();
            returnType = new Parameter();
        }

        public List<Parameter> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        public Parameter ReturnType
        {
            get { return returnType; }
        }

        public override string ToSql()
        {
            string sql = "CREATE FUNCTION " + FullName + "";
            string param = "";
            parameters.ForEach(item => param += item.ToSql() + ",");
            if (!String.IsNullOrEmpty(param))
            {
                param = param.Substring(0, param.Length - 1);
                sql += " (" + param + ")\r\n";
            }
            else
                sql += "()\r\n";
            sql += "RETURNS " + returnType.ToSql() + " ";
            sql += "WITH EXECUTE AS " + AssemblyExecuteAs + "\r\n";
            sql += "AS\r\n";
            sql += "EXTERNAL NAME [" + AssemblyName + "].[" + AssemblyClass + "].[" + AssemblyMethod + "]\r\n";
            sql += "GO\r\n";
            return sql;
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList list = new SQLScriptList();

            if (this.HasState(Enums.ObjectStatusType.DropStatus))
                list.Add(Drop());
            if (this.HasState(Enums.ObjectStatusType.CreateStatus))
                list.Add(Create());
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
            {
                list.AddRange(Rebuild());
            }
            list.AddRange(this.ExtendedProperties.ToSqlDiff());
            return list;
        }
    }
}
