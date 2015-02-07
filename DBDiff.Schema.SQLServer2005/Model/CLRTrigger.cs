using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class CLRTrigger : CLRCode
    {
        private bool isDelete;
        private bool isInsert;
        private bool isUpdate;
 
        public CLRTrigger(ISchemaBase parent)
            : base(parent, Enums.ObjectType.CLRTrigger, Enums.ScripActionType.AddTrigger, Enums.ScripActionType.DropTrigger)
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

        public bool IsUpdate
        {
            get { return isUpdate; }
            set { isUpdate = value; }
        }

        public bool IsInsert
        {
            get { return isInsert; }
            set { isInsert = value; }
        }

        public bool IsDelete
        {
            get { return isDelete; }
            set { isDelete = value; }
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
