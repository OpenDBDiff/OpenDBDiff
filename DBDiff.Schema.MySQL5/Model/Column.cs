using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Xml.Serialization;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.MySQL.Model
{
    public class Column : MySQLSchemaBase
    {
        private int ordinalPosition;
        private string defaultValue;
        private Boolean nullable;
        private string type;
        private int size;
        private int precision;
        private int scale;
        private string characterSet;
        private string collation;
        private string extra;
        private string comments;

        public Column(Table parent):base(StatusEnum.ObjectTypeEnum.Column)
        {
            this.Parent = parent;
            
        }
        
        public Column Clone(Table parentObject)
        {
            Column col = new Column(parentObject);
            col.CharacterSet = this.CharacterSet;
            col.Id = this.Id;
            col.Collation = this.Collation;
            col.Comments = this.Comments;
            col.DefaultValue = this.DefaultValue;
            col.Extra = this.Extra;
            col.Name = this.Name;
            col.Nullable = this.Nullable;
            col.OrdinalPosition = this.OrdinalPosition;
            col.Owner = this.Owner;
            col.Precision = this.Precision;
            col.Scale = this.Scale;
            col.Size = this.Size;
            col.Type = this.Type;
            return col;
        }
        
        public string Collation
        {
            get { return collation; }
            set { collation = value; }
        }

        public string CharacterSet
        {
            get { return characterSet; }
            set { characterSet = value; }
        }

        public int Precision
        {
            get { return precision; }
            set { precision = value; }
        }
        public int Scale
        {
            get { return scale; }
            set { scale = value; }
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

        public Boolean Nullable
        {
            get { return nullable; }
            set { nullable = value; }
        }

        public string DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

        public int OrdinalPosition
        {
            get { return ordinalPosition; }
            set { ordinalPosition = value; }
        }

        public string Extra
        {
            get { return extra; }
            set { extra = value; }
        }

        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }

        /// <summary>
        /// Devuelve el schema de la columna en formato SQL.
        /// </summary>
        public string ToSQL()
        {
            string sql = "";
            sql += "[" + Name + "] " + Type;
            if (Nullable)
                sql += " NULL ";
            else
                sql += " NOT NULL ";
            sql += Extra;
            return sql.Trim();
        }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Column origen, Column destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.Collation.Equals(destino.Collation)) return false;
            if (!origen.CharacterSet.Equals(destino.CharacterSet)) return false;
            if (!origen.Comments.Equals(destino.Comments)) return false;
            if (!origen.DefaultValue.Equals(destino.DefaultValue)) return false;
            if (!origen.Extra.Equals(destino.Extra)) return false;
            if (!origen.Type.Equals(destino.Type)) return false;
            if (origen.Size != destino.Size) return false;
            if (origen.Scale != destino.Scale) return false;
            if (origen.Precision != destino.Precision) return false;
            if (origen.OrdinalPosition != destino.OrdinalPosition) return false;
            if (origen.Nullable != destino.Nullable) return false;
            return true;
        }
    }
}
