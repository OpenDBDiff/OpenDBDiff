using System;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Role : SQLServerSchemaBase
    {
        public enum RoleTypeEnum
        {
            ApplicationRole = 1,
            DatabaseRole = 2
        }

        public Role(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Role)
        {
        }

        public override string FullName
        {
            get { return "[" + Name + "]"; }
        }

        public RoleTypeEnum Type { get; set; }

        public string Password { get; set; }

        public override string ToSql()
        {
            string sql = "";
            sql += "CREATE " + ((Type == RoleTypeEnum.ApplicationRole) ? "APPLICATION" : "") + " ROLE ";
            sql += FullName + " ";
            sql += "WITH PASSWORD = N'" + Password + "'";
            if (!String.IsNullOrEmpty(Owner))
                sql += " ,DEFAULT_SCHEMA=[" + Owner + "]";
            return sql.Trim() + "\r\nGO\r\n";
        }

        public override string ToSqlDrop()
        {
            return "DROP " + ((Type == RoleTypeEnum.ApplicationRole) ? "APPLICATION" : "") + " ROLE " + FullName + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropRole);
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddRole);
            }
            if ((this.Status & Enums.ObjectStatusType.AlterStatus) == Enums.ObjectStatusType.AlterStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropRole);
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddRole);
            }
            return listDiff;
        }


        public Boolean Compare(Role obj)
        {
            if (obj == null) throw new ArgumentNullException("destination");
            if (this.Type != obj.Type) return false;
            if (!this.Password.Equals(obj.Password)) return false;
            if (!this.Owner.Equals(obj.Owner)) return false;
            return true;
        }
    }
}
