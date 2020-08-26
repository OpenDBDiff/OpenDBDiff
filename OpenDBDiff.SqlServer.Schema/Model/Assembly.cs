using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class Assembly : Code
    {
        public Assembly(ISchemaBase parent)
            : base(parent, ObjectType.Assembly, ScriptAction.AddAssembly, ScriptAction.DropAssembly)
        {
            Files = new SchemaList<AssemblyFile, Assembly>(this);
        }

        public override ISchemaBase Clone(ISchemaBase parent)
        {
            Assembly item = new Assembly(parent)
            {
                Id = this.Id,
                Name = this.Name,
                Owner = this.Owner,
                Visible = this.Visible,
                Text = this.Text,
                PermissionSet = this.PermissionSet,
                CLRName = this.CLRName,
                Guid = this.Guid,
                Files = this.Files
            };
            this.DependenciesOut.ForEach(dep => item.DependenciesOut.Add(dep));
            this.ExtendedProperties.ForEach(ep => item.ExtendedProperties.Add(ep));
            return item;
        }

        public SchemaList<AssemblyFile, Assembly> Files { get; set; }

        public override string FullName
        {
            get { return "[" + Name + "]"; }
        }

        public string CLRName { get; set; }

        public bool Visible { get; set; }

        public string PermissionSet { get; set; }

        public override string ToSql()
        {
            string access = PermissionSet;
            if (PermissionSet.Equals("UNSAFE_ACCESS")) access = "UNSAFE";
            if (PermissionSet.Equals("SAFE_ACCESS")) access = "SAFE";
            string toSql = "CREATE ASSEMBLY ";
            toSql += FullName + "\r\n";
            toSql += "AUTHORIZATION " + Owner + "\r\n";
            toSql += "FROM " + Text + "\r\n";
            toSql += "WITH PERMISSION_SET = " + access + "\r\n";
            toSql += "GO\r\n";
            toSql += Files.ToSql();
            toSql += this.ExtendedProperties.ToSql();
            return toSql;
        }

        public override string ToSqlDrop()
        {
            return "DROP ASSEMBLY " + FullName + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        private string ToSQLAlter()
        {
            string access = PermissionSet;
            if (PermissionSet.Equals("UNSAFE_ACCESS")) access = "UNSAFE";
            if (PermissionSet.Equals("SAFE_ACCESS")) access = "SAFE";
            return "ALTER ASSEMBLY " + FullName + " WITH PERMISSION_SET = " + access + "\r\nGO\r\n";
        }

        private string ToSQLAlterOwner()
        {
            return "ALTER AUTHORIZATION ON ASSEMBLY::" + FullName + " TO " + Owner + "\r\nGO\r\n";
        }

        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            SQLScriptList list = new SQLScriptList();

            if (this.Status == ObjectStatus.Drop)
            {
                list.AddRange(RebuildDependencies());
                list.Add(Drop());
            }
            if (this.Status == ObjectStatus.Create)
                list.Add(Create());
            if (this.HasState(ObjectStatus.Rebuild))
                list.AddRange(Rebuild());
            if (this.HasState(ObjectStatus.ChangeOwner))
                list.Add(ToSQLAlterOwner(), 0, ScriptAction.AlterAssembly);
            if (this.HasState(ObjectStatus.PermissionSet))
                list.Add(ToSQLAlter(), 0, ScriptAction.AlterAssembly);
            if (this.HasState(ObjectStatus.Alter))
                list.AddRange(Files.ToSqlDiff());
            list.AddRange(this.ExtendedProperties.ToSqlDiff());
            return list;
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
