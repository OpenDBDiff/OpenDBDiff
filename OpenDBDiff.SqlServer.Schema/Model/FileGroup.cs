using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class FileGroup : SQLServerSchemaBase
    {
        public FileGroup(ISchemaBase parent)
            : base(parent, ObjectType.FileGroup)
        {
            Files = new FileGroupFiles(this);
        }

        public override ISchemaBase Clone(ISchemaBase parent)
        {
            FileGroup file = new FileGroup(parent);
            file.IsDefaultFileGroup = this.IsDefaultFileGroup;
            file.IsReadOnly = this.IsReadOnly;
            file.Name = this.Name;
            file.Id = this.Id;
            file.Files = this.Files.Clone(file);
            file.Guid = this.Guid;
            file.IsFileStream = this.IsFileStream;
            return file;
        }

        public FileGroupFiles Files { get; set; }

        public Boolean IsFileStream { get; set; }

        public Boolean IsDefaultFileGroup { get; set; }

        public Boolean IsReadOnly { get; set; }

        public static Boolean Compare(FileGroup origin, FileGroup destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if (origin.IsReadOnly != destination.IsReadOnly) return false;
            if (origin.IsDefaultFileGroup != destination.IsDefaultFileGroup) return false;
            if (origin.IsFileStream != destination.IsFileStream) return false;
            return true;
        }

        private string ToSQL(string action)
        {
            string sql = "ALTER DATABASE [" + Parent.Name + "] " + action + " ";
            sql += "FILEGROUP [" + Name + "]";
            if (action.Equals("MODIFY"))
            {
                if (IsDefaultFileGroup) sql += " DEFAULT";
            }
            else
                if (IsFileStream) sql += " CONTAINS FILESTREAM";
            if (IsReadOnly) sql += " READONLY";
            sql += "\r\nGO\r\n";
            return sql;
        }

        public override string ToSql()
        {
            string sql = ToSQL("ADD");
            foreach (FileGroupFile file in this.Files)
                sql += file.ToSql();
            if (IsDefaultFileGroup)
                sql += ToSQL("MODIFY");
            return sql;
        }

        public override string ToSqlAdd()
        {
            string sql = ToSQL("ADD");
            foreach (FileGroupFile file in this.Files)
                sql += file.ToSqlAdd();
            if (IsDefaultFileGroup)
                sql += ToSQL("MODIFY");
            return sql;
        }

        public string ToSQLAlter()
        {
            return ToSQL("MODIFY");
        }

        public override string ToSqlDrop()
        {
            string sql = "";
            sql = Files.ToSQLDrop();
            return sql + "ALTER DATABASE [" + Parent.Name + "] REMOVE FILEGROUP [" + Name + "]\r\nGO\r\n\r\n";
        }

        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == ObjectStatus.Drop)
                listDiff.Add(this.ToSqlDrop(), 1, ScriptAction.DropFileGroup);
            if (this.Status == ObjectStatus.Create)
                listDiff.Add(this.ToSqlAdd(), 1, ScriptAction.AddFileGroup);
            if (this.Status == ObjectStatus.Alter)
                listDiff.Add(this.ToSQLAlter(), 1, ScriptAction.AlterFileGroup);

            return listDiff;
        }
    }
}
