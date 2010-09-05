using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class ConstraintColumns:SchemaList<ConstraintColumn, Constraint>
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
        public static Boolean Compare(ConstraintColumns origen, ConstraintColumns destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.Count != destino.Count) return false;
            for (int j = 0; j < origen.Count; j++)
            {
                ConstraintColumn item = destino[origen[j].FullName];
                if (item == null)
                    return false;
                else
                    if (!ConstraintColumn.Compare(origen[j], item)) return false;
            }
            for (int j = 0; j < destino.Count; j++)
            {
                ConstraintColumn item = origen[destino[j].FullName];
                if (item == null)
                    return false;
                else
                    if (!ConstraintColumn.Compare(destino[j], item)) return false;
            }
            return true;
        }
    }
}
