using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class PartitionFunction : SQLServerSchemaBase
    {
        private const int IS_STRING = 0;
        private const int IS_UNIQUE = 1;
        private const int IS_DATE = 2;
        private const int IS_NUMERIC = 3;

        public PartitionFunction(ISchemaBase parent)
            : base(parent, Enums.ObjectType.PartitionFunction)
        {
            Values = new List<string>();
        }

        public PartitionFunction Clone(ISchemaBase parent)
        {
            PartitionFunction item = new PartitionFunction(parent);
            item.Id = this.Id;
            item.IsBoundaryRight = this.IsBoundaryRight;
            item.Name = this.Name;
            item.Precision = this.Precision;
            item.Scale = this.Scale;
            item.Size = this.Size;
            item.Type = this.Type;
            this.Values.ForEach(value => { item.Values.Add(value); });
            return item;
        }

        public List<string> Values { get; set; }

        public PartitionFunction Old { get; set; }

        public int Precision { get; set; }

        public int Scale { get; set; }

        public int Size { get; set; }

        public bool IsBoundaryRight { get; set; }

        public string Type { get; set; }

        private int ValueItem(string typeName)
        {
            if ((typeName.Equals("nchar") || typeName.Equals("nvarchar") || typeName.Equals("varchar") || typeName.Equals("char")))
                return IS_STRING;
            if (typeName.Equals("uniqueidentifier"))
                return IS_UNIQUE;
            if (typeName.Equals("datetime") || typeName.Equals("smalldatetime") || typeName.Equals("datetime2") || typeName.Equals("time") || typeName.Equals("datetimeoffset"))
                return IS_DATE;
            if (typeName.Equals("numeric") || typeName.Equals("decimal") || typeName.Equals("float") || typeName.Equals("real") || typeName.Equals("money") || typeName.Equals("smallmoney"))
                return IS_NUMERIC;

            return IS_NUMERIC;
        }

        public override string ToSql()
        {
            string sqltype = Type;

            if (Type.Equals("binary") || Type.Equals("varbinary") || Type.Equals("varchar") || Type.Equals("char") || Type.Equals("nchar") || Type.Equals("nvarchar"))
            {
                if (Type.Equals("nchar") || Type.Equals("nvarchar"))
                    sqltype += " (" + (Size / 2).ToString(CultureInfo.InvariantCulture) + ")";
                else
                    sqltype += " (" + Size.ToString(CultureInfo.InvariantCulture) + ")";
            }
            if (Type.Equals("numeric") || Type.Equals("decimal")) sqltype += " (" + Precision.ToString(CultureInfo.InvariantCulture) + "," + Scale.ToString(CultureInfo.InvariantCulture) + ")";
            if (((Database)Parent).Info.Version >= DatabaseInfo.VersionTypeEnum.SQLServer2008)
            {
                if (Type.Equals("datetime2") || Type.Equals("datetimeoffset") || Type.Equals("time")) sqltype += "(" + Scale.ToString(CultureInfo.InvariantCulture) + ")";
            }

            string sql = "CREATE PARTITION FUNCTION [" + Name + "](" + sqltype + ") AS RANGE\r\n ";
            if (IsBoundaryRight)
                sql += "RIGHT";
            else
                sql += "LEFT";
            sql += " FOR VALUES (";

            string sqlvalues = "";
            int valueType = ValueItem(Type);

            if (valueType == IS_STRING)
                Values.ForEach(item => { sqlvalues += "N'" + item + "',"; });
            else
                if (valueType == IS_DATE)
                Values.ForEach(item => { sqlvalues += "'" + DateTime.Parse(item).ToString("yyyyMMdd HH:mm:ss.fff") + "',"; });
            else
                    if (valueType == IS_UNIQUE)
                Values.ForEach(item => { sqlvalues += "'{" + item + "}',"; });
            else
                        if (valueType == IS_NUMERIC)
                Values.ForEach(item => { sqlvalues += item.Replace(",", ".") + ","; });
            else
                Values.ForEach(item => { sqlvalues += item + ","; });
            sql += sqlvalues.Substring(0, sqlvalues.Length - 1) + ")";

            return sql + "\r\nGO\r\n";
        }

        public override string ToSqlDrop()
        {
            return "DROP PARTITION FUNCTION [" + Name + "]\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public string ToSqlAlter()
        {
            StringBuilder sqlFinal = new StringBuilder();
            string sql = "ALTER PARTITION FUNCTION [" + Name + "]()\r\n";
            string sqlmergue = "";
            string sqsplit = "";
            IEnumerable<string> items = Old.Values.Except<string>(this.Values);
            int valueType = ValueItem(Type);
            foreach (var item in items)
            {
                sqlmergue = "MERGE RANGE (";
                if (valueType == IS_STRING)
                    sqlmergue += "N'" + item + "'";
                else
                    if (valueType == IS_DATE)
                    sqlmergue += "'" + DateTime.Parse(item).ToString("yyyyMMdd HH:mm:ss.fff") + "'";
                else
                        if (valueType == IS_UNIQUE)
                    sqlmergue += "'{" + item + "}'";
                else
                            if (valueType == IS_NUMERIC)
                    sqlmergue += item.Replace(",", ".");
                else
                    sqlmergue += item;
                sqlFinal.Append(sql + sqlmergue + ")\r\nGO\r\n");
            }
            IEnumerable<string> items2 = this.Values.Except<string>(this.Old.Values);
            foreach (var item in items2)
            {
                sqsplit = "SPLIT RANGE (";
                if (valueType == IS_STRING)
                    sqsplit += "N'" + item + "'";
                else
                    if (valueType == IS_DATE)
                    sqsplit += "'" + DateTime.Parse(item).ToString("yyyyMMdd HH:mm:ss.fff") + "'";
                else
                        if (valueType == IS_UNIQUE)
                    sqsplit += "'{" + item + "}'";
                else
                            if (valueType == IS_NUMERIC)
                    sqsplit += item.Replace(",", ".");
                else
                    sqsplit += item;
                sqlFinal.Append(sql + sqsplit + ")\r\nGO\r\n");
            }
            return sqlFinal.ToString();
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropPartitionFunction);
            }
            if (this.Status == Enums.ObjectStatusType.RebuildStatus)
            {
                listDiff.Add(ToSqlDrop() + ToSqlAdd(), 0, Enums.ScripActionType.AlterPartitionFunction);
            }
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
                listDiff.Add(ToSqlAlter(), 0, Enums.ScripActionType.AlterPartitionFunction);

            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSqlAdd(), 0, Enums.ScripActionType.AddPartitionFunction);
            }
            return listDiff;
        }

        public static Boolean Compare(PartitionFunction origin, PartitionFunction destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if (!origin.Type.Equals(destination.Type)) return false;
            if (origin.Size != destination.Size) return false;
            if (origin.Precision != destination.Precision) return false;
            if (origin.Scale != destination.Scale) return false;
            if (origin.IsBoundaryRight != destination.IsBoundaryRight) return false;
            return true;
        }

        public static Boolean CompareValues(PartitionFunction origin, PartitionFunction destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if (origin.Values.Count != destination.Values.Count) return false;
            if (origin.Values.Except(destination.Values).ToList().Count != 0) return false;
            return true;
        }
    }
}
