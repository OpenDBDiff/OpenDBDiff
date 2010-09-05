using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.MySQL.Model
{
    public class IndexColumns:List<IndexColumn>
    {
        private Table parent;

        internal IndexColumns(Table parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Clona el objeto ColumnConstraints en una nueva instancia.
        /// </summary>
        public IndexColumns Clone()
        {
            IndexColumns columns = new IndexColumns(parent);
            for (int index = 0; index < this.Count; index++)
            {
                columns.Add(this[index].Clone(parent));
            }
            return columns;
        }

        public IndexColumn Find(string column)
        {
            for (int index = 0; index < this.Count; index++)
            {
                if (this[index].Name.Equals(column)) return this[index];
            }
            return null;
        }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(IndexColumns origen, IndexColumns destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.Count != destino.Count) return false;
            for (int j = 0; j < origen.Count; j++)
            {
                IndexColumn item = destino.Find(origen[j].Name);
                if (item == null)
                    return false;
                else
                    if (!IndexColumn.Compare(origen[j], item)) return false;
            }
            for (int j = 0; j < destino.Count; j++)
            {
                IndexColumn item = origen.Find(destino[j].Name);
                if (item == null)
                    return false;
                else
                    if (!IndexColumn.Compare(destino[j], item)) return false;
            }
            return true;
        }
    }
}
