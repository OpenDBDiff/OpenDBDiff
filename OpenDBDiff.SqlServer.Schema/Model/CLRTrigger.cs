using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class CLRTrigger : CLRCode
    {
        public CLRTrigger(ISchemaBase parent)
            : base(parent, ObjectType.CLRTrigger, ScriptAction.AddTrigger, ScriptAction.DropTrigger)
        {
        }

        public override string ToSql()
        {
            string sql = "CREATE TRIGGER " + FullName + " ON " + Parent.FullName;
            sql += " AFTER ";
            if (IsInsert) sql += "INSERT,";
            if (IsUpdate) sql += "UPDATE,";
            if (IsDelete) sql += "DELETE,";
            sql = sql.Substring(0, sql.Length - 1) + " ";
            sql += "AS\r\n";
            sql += "EXTERNAL NAME [" + AssemblyName + "].[" + AssemblyClass + "].[" + AssemblyMethod + "]\r\n";
            sql += "GO\r\n";
            return sql;
        }

        public bool IsUpdate { get; set; }

        public bool IsInsert { get; set; }

        public bool IsDelete { get; set; }

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
