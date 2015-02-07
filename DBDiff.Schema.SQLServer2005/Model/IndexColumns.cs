using System;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class IndexColumns : SchemaList<IndexColumn,ISchemaBase>
    {
        public IndexColumns(ISchemaBase parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Clona el objeto ColumnConstraints en una nueva instancia.
        /// </summary>
        public IndexColumns Clone()
        {
            IndexColumns columns = new IndexColumns(Parent);
            for (int index = 0; index < this.Count; index++)
            {
                columns.Add(this[index].Clone(Parent));
            }
            return columns;
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
                IndexColumn item = destino[origen[j].FullName];
                if (item == null)
                    return false;
                else
                    if (!IndexColumn.Compare(origen[j], item)) return false;
            }
            for (int j = 0; j < destino.Count; j++)
            {
                IndexColumn item = origen[destino[j].FullName];
                if (item == null)
                    return false;
                else
                    if (!IndexColumn.Compare(destino[j], item)) return false;
            }
            return true;
        }
    }
}
