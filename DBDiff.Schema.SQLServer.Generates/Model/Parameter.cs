using System.Globalization;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Parameter
    {
        public bool Output { get; set; }

        public byte Scale { get; set; }

        public byte Precision { get; set; }

        public string Name { get; set; }

        public int Size { get; set; }

        public string Type { get; set; }

        public string ToSql()
        {
            string sql = Name + " [" + Type + "]";
            if (Type.Equals("binary") || Type.Equals("varbinary") || Type.Equals("varchar") || Type.Equals("char") || Type.Equals("nchar") || Type.Equals("nvarchar"))
            {
                if (Size == -1)
                    sql += "(max)";
                else
                {
                    sql += "(" + Size.ToString(CultureInfo.InvariantCulture) + ")";
                }
            }
            if (Type.Equals("numeric") || Type.Equals("decimal")) sql += "(" + Precision.ToString(CultureInfo.InvariantCulture) + "," + Scale.ToString(CultureInfo.InvariantCulture) + ")";
            if (Output) sql += " OUTPUT";
            return sql;
        }
    }
}
