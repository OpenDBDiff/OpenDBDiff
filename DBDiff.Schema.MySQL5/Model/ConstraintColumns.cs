using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.MySQL.Model
{
    public class ConstraintColumns:List<ConstraintColumn>
    {
        private Constraint parent;

        public ConstraintColumns(Constraint parent)
        {
            this.parent = parent;
        }
        
        /// <summary>
        /// Clona el objeto Columns en una nueva instancia.
        /// </summary>
        public ConstraintColumns Clone(Constraint parentObject)
        {
            ConstraintColumns columns = new ConstraintColumns(parentObject);
            for (int index = 0; index < this.Count; index++)
            {
                columns.Add(this[index].Clone(parentObject));
            }
            return columns;
        }

        public ConstraintColumn Find(string column)
        {
            for (int index = 0;index < this.Count;index++)
            {
                if (this[index].Name.Equals(column)) return this[index];
            }
            return null;
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
                ConstraintColumn item = destino.Find(origen[j].Name);
                if (item == null)
                    return false;
                else
                    if (!ConstraintColumn.Compare(origen[j], item)) return false;
            }
            for (int j = 0; j < destino.Count; j++)
            {
                ConstraintColumn item = origen.Find(destino[j].Name);
                if (item == null)
                    return false;
                else
                    if (!ConstraintColumn.Compare(destino[j], item)) return false;
            }
            return true;
        }
    }
}
