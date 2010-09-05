using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class ConstraintColumns:List<ConstraintColumn>
    {
        private Constraint parent;

        internal ConstraintColumns(Constraint parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Clona el objeto ColumnConstraints en una nueva instancia.
        /// </summary>
        public ConstraintColumns Clone()
        {
            ConstraintColumns columns = new ConstraintColumns(parent);
            for (int index = 0; index < this.Count; index++)
            {
                columns.Add(this[index].Clone());
            }
            return columns;
        }

        public ConstraintColumn this[string name]
        {
            get
            {
                return Find(delegate(ConstraintColumn item) { return item.FullName.Equals(name); });
            }
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
