using System;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class ConstraintColumns : SchemaList<ConstraintColumn, Constraint>
    {
        public ConstraintColumns(Constraint parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Clona el objeto ColumnConstraints en una nueva instancia.
        /// </summary>
        public ConstraintColumns Clone()
        {
            ConstraintColumns columns = new ConstraintColumns(this.Parent);
            for (int index = 0; index < this.Count; index++)
            {
                columns.Add(this[index].Clone());
            }
            return columns;
        }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(ConstraintColumns origin, ConstraintColumns destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if (origin.Count != destination.Count) return false;
            for (int j = 0; j < origin.Count; j++)
            {
                ConstraintColumn item = destination[origin[j].FullName];
                if (item == null)
                    return false;
                else
                    if (!ConstraintColumn.Compare(origin[j], item)) return false;
            }
            for (int j = 0; j < destination.Count; j++)
            {
                ConstraintColumn item = origin[destination[j].FullName];
                if (item == null)
                    return false;
                else
                    if (!ConstraintColumn.Compare(destination[j], item)) return false;
            }
            return true;
        }
    }
}
