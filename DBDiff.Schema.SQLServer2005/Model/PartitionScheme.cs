using System;
using System.Collections.Generic;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class PartitionScheme : SQLServerSchemaBase
    {
        public PartitionScheme(ISchemaBase parent)
            : base(parent, Enums.ObjectType.PartitionFunction)
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

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropPartitionScheme);
            }
            if (this.Status == Enums.ObjectStatusType.RebuildStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropPartitionScheme);
                listDiff.Add(ToSqlAdd(), 0, Enums.ScripActionType.AddPartitionScheme);
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSqlAdd(), 0, Enums.ScripActionType.AddPartitionScheme);
            }
            return listDiff;
        }

        public static Boolean Compare(PartitionScheme origen, PartitionScheme destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.PartitionFunction.Equals(destino.PartitionFunction)) return false;
            if (origen.FileGroups.Count != destino.FileGroups.Count) return false;
            for (int j = 0; j < origen.FileGroups.Count; j++)
            {
                if (origen.CompareFullNameTo(origen.FileGroups[j], destino.FileGroups[j]) != 0)
                    return false;
            }
            return true;
        }
    }
}
