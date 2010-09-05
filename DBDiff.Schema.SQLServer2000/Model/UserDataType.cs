using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    public class UserDataType:SchemaBase
    {
        private string type;
        private int size;
        private Boolean allowNull;
        private int precision;
        private int scale;

        public UserDataType(Database parent)
            : base("[", "]", StatusEnum.ObjectTypeEnum.UserDataType)
        {                      
            this.Parent = parent;
        }

        /// <summary>
        /// Cantidad de digitos que permite el campo (solo para campos Numeric).
        /// </summary>
        public int Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Cantidad de decimales que permite el campo (solo para campos Numeric).
        /// </summary>
        public int Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        public Boolean AllowNull
        {
            get { return allowNull; }
            set { allowNull = value; }
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

        /// <summary>
        /// Convierte el schema de la tabla en XML.
        /// </summary>        
        public string ToXML()
        {
            string xml = "";
            xml += "<DATATYPE owner=\"" + Owner + "\" name=\"" + Name + "\">\r\n";
            xml += "<TYPE>" + type + "</TYPE>";
            xml += "<LENGTH>" + size + "</LENGTH>";
            xml += "<ALLOWNULL>" + (AllowNull ? "1" : "0") + "</ALLOWNULL>";
            xml += "</DATATYPE>r\n";
            return xml;
        }

        public string ToSQL()
        {
            string sql = "EXEC sp_addtype N'" + Name + "',N'" + type;
            if (Type.Equals("varbinary") || Type.Equals("varchar") || Type.Equals("char") || Type.Equals("nchar") || Type.Equals("nvarchar")) sql += " (" + Size.ToString() + ")";
            sql += "',";
            if (AllowNull) 
                sql += "N'null'";
            else
                sql += "N'not null'";
            return sql + "\r\nGO\r\n";
        }

        public override string ToSQLDrop()
        {
            return "EXEC sp_droptype N'" + Name + "'\r\nGO\r\n";
        }
    }
}
