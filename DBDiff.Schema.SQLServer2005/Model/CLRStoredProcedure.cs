using System;
using System.Collections.Generic;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class CLRStoredProcedure : CLRCode
    {
        public CLRStoredProcedure(ISchemaBase parent)
            : base(parent, Enums.ObjectType.CLRStoredProcedure, Enums.ScripActionType.AddStoredProcedure, Enums.ScripActionType.DropStoredProcedure)
        {
            Parameters = new List<Parameter>();
        }

        public List<Parameter> Parameters { get; set; }

        public override string ToSql()
        {
            string sql = "CREATE PROCEDURE " + FullName + "\r\n";
            string param = "";
            Parameters.ForEach(item => param += "\t" + item.ToSql() + ",\r\n");
            if (!String.IsNullOrEmpty(param)) param = param.Substring(0, param.Length - 3) + "\r\n";
            sql += param;
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
