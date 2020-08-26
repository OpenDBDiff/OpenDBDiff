using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections.Generic;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class CLRStoredProcedure : CLRCode
    {
        public CLRStoredProcedure(ISchemaBase parent)
            : base(parent, ObjectType.CLRStoredProcedure, ScriptAction.AddStoredProcedure, ScriptAction.DropStoredProcedure)
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
