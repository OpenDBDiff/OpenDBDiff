using System;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class IndexColumns : SchemaList<IndexColumn, ISchemaBase>
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
        public static Boolean Compare(IndexColumns origin, IndexColumns destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if (origin.Count != destination.Count) return false;
            for (int j = 0; j < origin.Count; j++)
            {
                IndexColumn item = destination[origin[j].FullName];
                if (item == null)
                    return false;
                else
                    if (!IndexColumn.Compare(origin[j], item)) return false;
            }
            for (int j = 0; j < destination.Count; j++)
            {
                IndexColumn item = origin[destination[j].FullName];
                if (item == null)
                    return false;
                else
                    if (!IndexColumn.Compare(destination[j], item)) return false;
            }
            return true;
        }
    }
}
