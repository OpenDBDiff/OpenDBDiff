using System.Globalization;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Parameter
    {
        private string type;
        private int size;
        private string name;
        private byte scale;
        private byte precision;
        private bool output;

        public bool Output
        {
            get { return output; }
            set { output = value; }
        }

        public byte Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public byte Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string ToSql()
        {
            string sql = name + " [" + type + "]";
            if (Type.Equals("binary") || Type.Equals("varbinary") || Type.Equals("varchar") || Type.Equals("char") || Type.Equals("nchar") || Type.Equals("nvarchar"))
            {
                if (Size == -1)
                    sql += "(max)";
                else
                {
                    sql += "(" + Size.ToString(CultureInfo.InvariantCulture) + ")";
                }
            }
            if (type.Equals("numeric") || type.Equals("decimal")) sql += "(" + Precision.ToString(CultureInfo.InvariantCulture) + "," + Scale.ToString(CultureInfo.InvariantCulture) + ")";            
            if (output) sql += " OUTPUT";
            return sql;
        }
    }
}
