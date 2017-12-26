using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class AssemblyFile : SQLServerSchemaBase
    {
        public AssemblyFile(ISchemaBase parent, AssemblyFile assemblyFile, Enums.ObjectStatusType status)
            : base(parent, Enums.ObjectType.AssemblyFile)
        {
            this.Name = assemblyFile.Name;
            this.Content = assemblyFile.Content;
            this.Status = status;
        }

        public AssemblyFile(ISchemaBase parent, string name, string content)
            : base(parent, Enums.ObjectType.AssemblyFile)
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

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropAssemblyFile);
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
                listDiff.Add(ToSqlAdd(), 0, Enums.ScripActionType.AddAssemblyFile);
            if (this.HasState(Enums.ObjectStatusType.AlterStatus))
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropAssemblyFile);
                listDiff.Add(ToSqlAdd(), 0, Enums.ScripActionType.AddAssemblyFile);
            }
            return listDiff;
        }
    }
}
