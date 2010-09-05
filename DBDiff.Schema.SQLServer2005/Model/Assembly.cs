using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Assembly : SQLServerSchemaBase
    {
        private string permissionSet;
        private string content;
        private bool visible;
        private string clrName;

        public Assembly(ISchemaBase parent)
            : base(StatusEnum.ObjectTypeEnum.Synonym)
        {
            this.Parent = parent;
        }

        public Assembly Clone(ISchemaBase parent)
        {
            Assembly item = new Assembly(parent);
            item.Id = this.Id;
            item.Name = this.Name;
            item.Owner = this.Owner;
            item.Visible = this.Visible;
            item.Content = this.Content;
            item.PermissionSet = this.PermissionSet;
            item.CLRName = this.CLRName;
            item.Guid = this.Guid;
            return item;
        }
        
        public override string FullName
        {
            get { return "[" + Name + "]"; }
        }

        public string CLRName
        {
            get { return clrName; }
            set { clrName = value; }
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public string PermissionSet
        {
            get { return permissionSet; }
            set { permissionSet = value; }
        }

        public string ToSQL()
        {
            string sql = "CREATE ASSEMBLY ";
            sql += FullName + "\r\n";
            sql += "AUTHORIZATION " + Owner + "\r\n";
            sql += "FROM " + content + "\r\n";
            sql += "WITH PERMISSION_SET = " + permissionSet + "\r\n";
            return sql;
        }

        public override string ToSQLDrop()
        {
            return "DROP ASSEMBLY " + FullName + "\r\nGO\r\n";
        }

        public override string ToSQLAdd()
        {
            return ToSQL();
        }

        public string ToSQLAlter()
        {
            return "ALTER ASSEMBLY " + FullName + " WITH PERMISSION_SET = " + PermissionSet + "\r\nGO\r\n";
        }

        public string ToSQLAlterOwner()
        {
            return "ALTER AUTHORIZATION ON ASSEMBLY::" + FullName + " TO " + Owner + "\r\nGO\r\n";
        }

        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == StatusEnum.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSQLDrop(), 0, StatusEnum.ScripActionType.DropSynonyms);
            }
            if (this.Status == StatusEnum.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSQL(), 0, StatusEnum.ScripActionType.AddSynonyms);
            }
            if ((this.Status & StatusEnum.ObjectStatusType.AlterStatus) == StatusEnum.ObjectStatusType.AlterStatus)
            {
                listDiff.Add(ToSQLAlter(), 0, StatusEnum.ScripActionType.AlterAssembly);
            }
            if ((this.Status & StatusEnum.ObjectStatusType.ChangeOwner) == StatusEnum.ObjectStatusType.ChangeOwner)
            {
                listDiff.Add(ToSQLAlterOwner(), 0, StatusEnum.ScripActionType.AlterAssembly);
            }
            return listDiff;
        }

        public static Boolean Compare(Assembly origen, Assembly destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.CLRName.Equals(destino.CLRName)) return false;
            if (!origen.PermissionSet.Equals(destino.PermissionSet)) return false;
            if (!origen.Owner.Equals(destino.Owner)) return false;
            return true;
        }
    }
}
