using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DBDiff.Schema.Model;

namespace DBDiff.Schema
{
    public abstract class FindBaseList<T,P>: List<T> where T: ISchemaBase where P:ISchemaBase 
    {
        private P parent;
        private Dictionary<string, int> nameMap = new Dictionary<string, int>();

        public FindBaseList(P parent)
        {
            this.parent = parent;
        }

        public new void Add(T item)
        {
            base.Add(item);
            if (!nameMap.ContainsKey(item.FullName))
                nameMap.Add(item.FullName, base.Count-1);
        }
        /// <summary>
        /// Devuelve el objecto Padre perteneciente a la coleccion.
        /// </summary>
        public P Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Devuelve el objeto correspondiente a un ID especifico.
        /// </summary>
        /// <param name="id">ID del objecto a buscar</param>
        /// <returns>Si no encontro nada, devuelve null, de lo contrario, el objeto</returns>
        public T Find(int id)
        {
            return Find(Item => Item.Id == id);
        }

        /// <summary>
        /// Indica si el nombre del objecto existe en la coleccion de objectos del mismo tipo.
        /// </summary>
        /// <param name="table">
        /// Nombre del objecto a buscar.
        /// </param>
        /// <returns></returns>
        public Boolean Exists(string name)
        {
            return nameMap.ContainsKey(name);
        }

        public T this[string name]
        {
            get
            {
                return this[nameMap[name]];
            }
            set
            {
                base[nameMap[name]] = value;
            }
        }
    }
}
