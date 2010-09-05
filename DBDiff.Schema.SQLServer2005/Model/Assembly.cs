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
        private List<ObjectDependency> dependencys;
        private List<AssemblyFile> files;

        public Assembly(ISchemaBase parent)
            : base(Enums.ObjectType.Assembly)
        {
            this.Parent = parent;
            this.dependencys = new List<ObjectDependency>();
            this.Files = new List<AssemblyFile>();
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
            item.Dependencys = this.Dependencys;
            item.Files = this.Files;
            return item;
        }

        public List<AssemblyFile> Files
        {
            get { return files; }
            set { files = value; }
        }

        public List<ObjectDependency> Dependencys
        {
            get { return dependencys; }
            set { dependencys = value; }
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

        public override string ToSql()
        {
            string sql = "CREATE ASSEMBLY ";
            sql += FullName + "\r\n";
            sql += "AUTHORIZATION " + Owner + "\r\n";
            sql += "FROM " + content + "\r\n";
            sql += "WITH PERMISSION_SET = " + permissionSet.Replace("_ACCESS", "") +"\r\n";
            sql += "GO\r\n";
            files.ForEach(item => sql += ToSqlFile(item));
            return sql;
        }

        private string ToSqlFile(AssemblyFile file)
        {
            string sql = "ALTER ASSEMBLY ";
            sql += FullName + "\r\n";
            sql += "ADD FILE FROM " + file.Content + "\r\n";
            sql += "AS N'" + file.File + "'\r\n";
            return sql + "GO\r\n";
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
            return "ALTER ASSEMBLY " + FullName + " WITH PERMISSION_SET = " + PermissionSet + "\r\nGO\r\n";
        }

        public string ToSQLAlterOwner()
        {
            return "ALTER AUTHORIZATION ON ASSEMBLY::" + FullName + " TO " + Owner + "\r\nGO\r\n";
        }

        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropAssembly);
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddAssembly);
            }
            if (this.HasState(Enums.ObjectStatusType.AlterStatus))
            {
                listDiff.Add(ToSQLAlter(), 0, Enums.ScripActionType.AlterAssembly);
            }
            if (this.HasState(Enums.ObjectStatusType.AlterRebuildStatus))
            {
                string sql = "";
                dependencys.ForEach(item =>
                {
                    ((Database)Parent).UserTypes[item.Name].Status = Enums.ObjectStatusType.AlterRebuildStatus;
                    ((Database)Parent).UserTypes[item.Name].SetWasInsertInDiffList(Enums.ScripActionType.DropUserDataType);
                    sql += ((Database)Parent).UserTypes[item.Name].ToSqlDrop();
                });
                listDiff.Add(sql + ToSqlDrop() + ToSql(), 0, Enums.ScripActionType.AddAssembly);
            }
            if (this.HasState(Enums.ObjectStatusType.ChangeOwner))
            {
                listDiff.Add(ToSQLAlterOwner(), 0, Enums.ScripActionType.AlterAssembly);
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
            if (!origen.Content.Equals(destino.Content)) return false;
            return true;
        }
    }
}
