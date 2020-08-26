using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections.Generic;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class PartitionScheme : SQLServerSchemaBase
    {
        public PartitionScheme(ISchemaBase parent)
            : base(parent, ObjectType.PartitionFunction)
        {
            FileGroups = new List<string>();
        }

        public List<string> FileGroups { get; set; }

        public string PartitionFunction { get; set; }

        public override string ToSqlAdd()
        {
            string sql = "CREATE PARTITION SCHEME " + FullName + "\r\n";
            sql += " AS PARTITION " + PartitionFunction + "\r\n";
            sql += "TO (";
            FileGroups.ForEach(item => sql += "[" + item + "],");
            sql = sql.Substring(0, sql.Length - 1);
            sql += ")\r\nGO\r\n";
            return sql;
        }

        public override string ToSqlDrop()
        {
            return "DROP PARTITION SCHEME " + FullName + "\r\nGO\r\n";
        }

        public override string ToSql()
        {
            return ToSqlAdd();
        }

        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == ObjectStatus.Drop)
            {
                listDiff.Add(ToSqlDrop(), 0, ScriptAction.DropPartitionScheme);
            }
            if (this.Status == ObjectStatus.Rebuild)
            {
                listDiff.Add(ToSqlDrop(), 0, ScriptAction.DropPartitionScheme);
                listDiff.Add(ToSqlAdd(), 0, ScriptAction.AddPartitionScheme);
            }
            if (this.Status == ObjectStatus.Create)
            {
                listDiff.Add(ToSqlAdd(), 0, ScriptAction.AddPartitionScheme);
            }
            return listDiff;
        }

        public static Boolean Compare(PartitionScheme origin, PartitionScheme destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if (!origin.PartitionFunction.Equals(destination.PartitionFunction)) return false;
            if (origin.FileGroups.Count != destination.FileGroups.Count) return false;
            for (int j = 0; j < origin.FileGroups.Count; j++)
            {
                if (origin.CompareFullNameTo(origin.FileGroups[j], destination.FileGroups[j]) != 0)
                    return false;
            }
            return true;
        }
    }
}
