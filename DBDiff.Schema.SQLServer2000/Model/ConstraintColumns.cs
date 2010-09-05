using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
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

        public Boolean Find(string column)
        {
            for (int index = 0;index < this.Count;index++)
            {
                if (this[index].Name.Equals(column)) return true;
            }
            return false;
        }

        public string ToXML()
        {
            string xml = "";
            int index;
            xml += "<COLUMNS>\n";
            for (index = 0; index < this.Count; index++)
            {
                xml += this[index].ToXML() + "\n";
            }
            xml += "</COLUMNS>\n";
            return xml;
        }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(ConstraintColumns origen, ConstraintColumns destino)
        {
            if (origen.Count != destino.Count) return false;
            for (int j = 0; j < origen.Count; j++)
            {
                if (!destino.Find(origen[j].Name)) return false;
            }
            for (int j = 0; j < destino.Count; j++)
            {
                if (!origen.Find(destino[j].Name)) return false;
            }
            return true;
        }
    }
}
