using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    public class ColumnPropertys:List<ColumnProperty>
    {
        private Hashtable hash = new Hashtable();
        private Column parent;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="parent">
        /// Objeto padre.
        /// </param>
        public ColumnPropertys(Column parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Indica si el nombre de la propiedad existe en la coleccion de tablas del objeto.
        /// </summary>
        /// <param name="table">
        /// Nombre de la tabla a buscar.
        /// </param>
        /// <returns></returns>
        public Boolean Find(string columnProperty)
        {
            return hash.ContainsKey(columnProperty);
        }


    }
}
