using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBDiff.Schema.Model
{
    public class SchemaList<T, P> : List<T>, ISchemaList<T, P>
        where T : ISchemaBase
        where P : ISchemaBase
    {
        private Dictionary<string, int> nameMap = new Dictionary<string, int>();
        private SearchSchemaBase allObjects = null;
        private bool IsCaseSensity = false;

        public SchemaList(P parent, SearchSchemaBase allObjects)
        {
            this.Parent = parent;
            this.allObjects = allObjects;
            this.Comparion = StringComparison.CurrentCultureIgnoreCase;
        }

        public SchemaList<T, P> Clone(P parentObject)
        {
            SchemaList<T, P> options = new SchemaList<T, P>(parentObject, allObjects);
            this.ForEach(item =>
            {
                object cloned = item.Clone(parentObject);

                //Not everything implements the clone methd, so make sure we got some actual cloned data before adding it back to the list
                if (cloned != null)
                    options.Add((T)cloned);
            });

            return options;
        }

        protected StringComparison Comparion { get; private set; }

        public SchemaList(P parent)
        {
            this.Parent = parent;
        }

        public new void Add(T item)
        {
            var db = this.Parent.RootParent;
            if (!db.Options.Filters.IsItemIncluded(item))
                return;

            base.Add(item);
            if (allObjects != null)
                allObjects.Add(item);

            string name = item.FullName;
            IsCaseSensity = item.RootParent.IsCaseSensity;
            if (!IsCaseSensity)
                name = name.ToUpper();

            if (!nameMap.ContainsKey(name))
                nameMap.Add(name, base.Count - 1);
        }
        /// <summary>
        /// Devuelve el objecto Padre perteneciente a la coleccion.
        /// </summary>
        public P Parent { get; private set; }

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
            if (IsCaseSensity)
                return nameMap.ContainsKey(name);
            else
                return nameMap.ContainsKey(name.ToUpper());
        }

        public virtual T this[string name]
        {
            get
            {
                try
                {
                    if (IsCaseSensity)
                        return this[nameMap[name]];
                    else
                        return this[nameMap[name.ToUpper()]];
                }
                catch
                {
                    return default(T);
                }
            }
            set
            {
                if (IsCaseSensity)
                    base[nameMap[name]] = value;
                else
                    base[nameMap[name.ToUpper()]] = value;
            }
        }

        public virtual SQLScriptList ToSqlDiff()
        {
            return ToSqlDiff(new List<ISchemaBase>());
        }
        public virtual SQLScriptList ToSqlDiff(List<ISchemaBase> schemas)
        {
            SQLScriptList listDiff = new SQLScriptList();
            this.Where(item => (schemas.Count == 0 || schemas.FirstOrDefault(sch => sch.Id == item.Id) != default(ISchemaBase))).ToList().ForEach(item => { item.ResetWasInsertInDiffList(); listDiff.AddRange(item.ToSqlDiff().WarnMissingScript(item)); });
            return listDiff;
        }


        public virtual string ToSql()
        {
            StringBuilder sql = new StringBuilder();
            this.ForEach(item =>
                {
                    if (item.Status != Enums.ObjectStatusType.DropStatus)
                        sql.Append(item.ToSql() + "\r\n");
                });
            return sql.ToString();
        }
    }
}
