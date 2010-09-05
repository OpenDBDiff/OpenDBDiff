using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Assembly : Code
    {
        private string permissionSet;
        private bool visible;
        private string clrName;
        private SchemaList<AssemblyFile,Assembly> files;

        public Assembly(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Assembly, Enums.ScripActionType.AddAssembly, Enums.ScripActionType.DropAssembly)
        {
            this.Parent = parent;
            this.Files = new SchemaList<AssemblyFile,Assembly>(this);
        }

        public override ISchemaBase Clone(ISchemaBase parent)
        {
            Assembly item = new Assembly(parent);
            item.Id = this.Id;
            item.Name = this.Name;
            item.Owner = this.Owner;
            item.Visible = this.Visible;
            item.Text = this.Text;
            item.PermissionSet = this.PermissionSet;
            item.CLRName = this.CLRName;
            item.Guid = this.Guid;
            this.DependenciesOut.ForEach(dep => item.DependenciesOut.Add(dep));
            this.ExtendedProperties.ForEach(ep => item.ExtendedProperties.Add(ep));
            item.Files = this.Files;
            return item;
        }

        public SchemaList<AssemblyFile, Assembly> Files
        {
            get { return files; }
            set { files = value; }
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

        public string PermissionSet
        {
            get { return permissionSet; }
            set { permissionSet = value; }
        }

        public override string ToSql()
        {
            string access = PermissionSet;
            if (PermissionSet.Equals("UNSAFE_ACCESS")) access = "UNSAFE";
            if (PermissionSet.Equals("SAFE_ACCESS")) access = "SAFE";
            string sql = "CREATE ASSEMBLY ";
            sql += FullName + "\r\n";
            sql += "AUTHORIZATION " + Owner + "\r\n";
            sql += "FROM " + Text + "\r\n";
            sql += "WITH PERMISSION_SET = " + access + "\r\n";
            sql += "GO\r\n";
            files.ForEach(item => sql += item.ToSql());
            return sql;
        }

        public override string ToSqlDrop()
        {
            return "DROP ASSEMBLY " + FullName + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public string ToSQLAlter()
        {
            string access = PermissionSet;
            if (PermissionSet.Equals("UNSAFE_ACCESS")) access = "UNSAFE";
            if (PermissionSet.Equals("SAFE_ACCESS")) access = "SAFE";
            return "ALTER ASSEMBLY " + FullName + " WITH PERMISSION_SET = " + access + "\r\nGO\r\n";
        }

        public string ToSQLAlterOwner()
        {
            return "ALTER AUTHORIZATION ON ASSEMBLY::" + FullName + " TO " + Owner + "\r\nGO\r\n";
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.AddRange(RebuildDependencys());
                listDiff.Add(Drop());
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(Create());
                ExtendedProperties.ForEach(item => listDiff.Add(item.Create()));
            }
            if (this.HasState(Enums.ObjectStatusType.AlterRebuildStatus))
                listDiff.AddRange(Rebuild());
            if (this.HasState(Enums.ObjectStatusType.ChangeOwner))
                listDiff.Add(ToSQLAlterOwner(), 0, Enums.ScripActionType.AlterAssembly);
            if (this.HasState(Enums.ObjectStatusType.PermisionSet))
                listDiff.Add(ToSQLAlter(), 0, Enums.ScripActionType.AlterAssembly);
            if (this.HasState(Enums.ObjectStatusType.AlterStatus))
                listDiff.AddRange(Files.ToSqlDiff());
            listDiff.AddRange(this.ToSqlDiffExtendedProperties());
            return listDiff;
        }

        public bool Compare(Assembly obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");            
            if (!this.CLRName.Equals(obj.CLRName)) return false;
            if (!this.PermissionSet.Equals(obj.PermissionSet)) return false;
            if (!this.Owner.Equals(obj.Owner)) return false;
            if (!this.Text.Equals(obj.Text)) return false;
            if (this.Files.Count != obj.Files.Count) return false;
            for (int j = 0; j < this.Files.Count; j++)
                if (!this.Files[j].Content.Equals(obj.Files[j].Content)) return false;
            return true;            
        }

        public override Boolean IsCodeType
        {
            get { return true; }
        }
    }
}
