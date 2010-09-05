using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.MySQL.Model
{
    public class Index : MySQLSchemaBase
    {

        private Boolean isUnique;
        private string type;
        private IndexColumns columns;

        public Index(Table table):base(StatusEnum.ObjectTypeEnum.Index)
        {
            Parent = table;
            columns = new IndexColumns(table);
        }

        public Index Clone(Table parent)
        {
            Index index = new Index(parent);
            index.IsUnique = this.IsUnique;
            index.Id = this.Id;
            index.Columns = this.Columns.Clone();
            index.Name = this.Name;
            index.Type = this.Type;                    
            return index;
        }

        /// <summary>
        /// Compara dos indices y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Index origen, Index destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.IsUnique != destino.IsUnique) return false;
            if (origen.Type != destino.Type) return false;
            if (!IndexColumns.Compare(origen.Columns, destino.Columns)) return false;
            return true;
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public Boolean IsUnique
        {
            get { return isUnique; }
            set { isUnique = value; }
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            return sql.ToString() + "\r\nGO\r\n";
        }

        public override string ToSQLAdd()
        {
            return ToSQL();
        }

        public override string ToSQLDrop()
        {
            return "DROP INDEX [" + Name + "] ON " + ((Table)Parent).FullName + "\r\nGO\r\n";
        }

        public IndexColumns Columns
        {
            get { return columns; }
            set { columns = value; }
        }
    }
}
