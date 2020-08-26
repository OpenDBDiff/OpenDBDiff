using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class AssemblyFile : SQLServerSchemaBase
    {
        public AssemblyFile(ISchemaBase parent, AssemblyFile assemblyFile, ObjectStatus status)
            : base(parent, ObjectType.AssemblyFile)
        {
            this.Name = assemblyFile.Name;
            this.Content = assemblyFile.Content;
            this.Status = status;
        }

        public AssemblyFile(ISchemaBase parent, string name, string content)
            : base(parent, ObjectType.AssemblyFile)
        {
            this.Name = name;
            this.Content = content;
        }

        public override string FullName
        {
            get { return "[" + Name + "]"; }
        }

        public string Content { get; set; }

        public override string ToSqlAdd()
        {
            string sql = "ALTER ASSEMBLY ";
            sql += this.Parent.FullName + "\r\n";
            sql += "ADD FILE FROM " + this.Content + "\r\n";
            sql += "AS N'" + this.Name + "'\r\n";
            return sql + "GO\r\n";
        }

        public override string ToSql()
        {
            return ToSqlAdd();
        }

        public override string ToSqlDrop()
        {
            string sql = "ALTER ASSEMBLY ";
            sql += this.Parent.FullName + "\r\n";
            sql += "DROP FILE N'" + this.Name + "'\r\n";
            return sql + "GO\r\n";
        }

        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == ObjectStatus.Drop)
                listDiff.Add(ToSqlDrop(), 0, ScriptAction.DropAssemblyFile);
            if (this.Status == ObjectStatus.Create)
                listDiff.Add(ToSqlAdd(), 0, ScriptAction.AddAssemblyFile);
            if (this.HasState(ObjectStatus.Alter))
            {
                listDiff.Add(ToSqlDrop(), 0, ScriptAction.DropAssemblyFile);
                listDiff.Add(ToSqlAdd(), 0, ScriptAction.AddAssemblyFile);
            }
            return listDiff;
        }
    }
}
