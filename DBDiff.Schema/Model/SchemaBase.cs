using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace DBDiff.Schema.Model
{
    [DebuggerDisplay("Id: {Id} - Name: {Name} - Status: {status}")]
    public abstract class SchemaBase : ISchemaBase
    {
        private Enums.ObjectStatusType status;
        private ISchemaBase parent;
        private string nameCharacterOpen;
        private string nameCharacterClose;
        private Hashtable wasInsertInDiffList;
        private IDatabase rootParent = null;

        protected SchemaBase(string nameCharacterOpen, string nameCharacterClose, Enums.ObjectType objectType)
        {
            this.Guid = System.Guid.NewGuid().ToString();
            this.ObjectType = objectType;
            this.status = Enums.ObjectStatusType.OriginalStatus;
            this.nameCharacterClose = nameCharacterClose;
            this.nameCharacterOpen = nameCharacterOpen;
        }

        /*protected object Clone(object vObj, ISchemaBase parentObject)
        {
            if (vObj.GetType().IsValueType || vObj.GetType() == Type.GetType("System.String"))
                return vObj;
            else
            {
                object newObject = Activator.CreateInstance(vObj.GetType(), new object[] {parentObject });
                foreach (PropertyInfo Item in newObject.GetType().GetProperties())
                {
                    if (Item.GetType().GetInterface("ICloneable") != null)
                    {
                        ICloneable IClone = (ICloneable)Item.GetValue(vObj, null);
                        Item.SetValue(newObject, IClone.Clone(), null);
                    }
                    else
                        Item.SetValue(newObject, Clone(Item.GetValue(vObj, null),parentObject), null);
                }
                foreach (FieldInfo Item in newObject.GetType().GetFields())
                {
                    if (Item.GetType().GetInterface("ICloneable") != null)
                    {
                        ICloneable IClone = (ICloneable)Item.GetValue(vObj);
                        Item.SetValue(newObject, IClone.Clone());
                    }
                    else
                        Item.SetValue(newObject, Clone(Item.GetValue(vObj),parentObject));
                }
                return newObject;
            }
         } */

        /// <summary>
        /// Objeto padre de la instancia.
        /// </summary>
        public ISchemaBase Parent
        {
            get { return parent; }
            set
            {
                rootParent = null;
                parent = value;
            }
        }

        public IDatabase RootParent
        {
            get
            {
                if (rootParent != null) return rootParent;
                if (this.Parent != null)
                {
                    if (this.Parent.Parent != null)
                        if (this.Parent.Parent.Parent != null)
                            rootParent = (IDatabase)this.Parent.Parent.Parent;
                        else
                            rootParent = (IDatabase)this.Parent.Parent;
                    else
                        rootParent = (IDatabase)this.Parent;
                }
                else if (this is IDatabase)
                {
                    rootParent = (IDatabase)this;
                }
                return rootParent;
            }
        }

        public int CompareFullNameTo(string name, string myName)
        {
            if (!RootParent.IsCaseSensity)
                return myName.ToUpper().CompareTo(name.ToUpper());
            else
                return myName.CompareTo(name);
        }

        /// <summary>
        /// SQL Code for the database object
        /// </summary>
        public abstract string ToSql();

        /// <summary>
        /// SQL Code for drop the database object
        /// </summary>
        public abstract string ToSqlDrop();

        /// <summary>
        /// SQL Code for add the database object
        /// </summary>
        public abstract string ToSqlAdd();

        /// <summary>
        /// Deep clone the object
        /// </summary>
        /// <param name="parent">Parent of the object</param>
        /// <returns></returns>
        public virtual ISchemaBase Clone(ISchemaBase parent)
        {
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public virtual SQLScriptList ToSqlDiff()
        {
            return null;
        }
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public virtual SQLScriptList ToSqlDiff(List<ISchemaBase> schemas)
        {
            return null;
        }

        public virtual SQLScript Create()
        {
            throw new NotImplementedException();
        }

        public virtual SQLScript Drop()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Devuelve si el objeto ya fue insertado en la lista de script con diferencias.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Boolean GetWasInsertInDiffList(Enums.ScripActionType action)
        {
            if (wasInsertInDiffList != null)
                return (wasInsertInDiffList.ContainsKey(action));
            else
                return false;
        }

        /// <summary>
        /// Setea que el objeto ya fue insertado en la lista de script con diferencias.
        /// </summary>
        public void SetWasInsertInDiffList(Enums.ScripActionType action)
        {
            if (wasInsertInDiffList == null) wasInsertInDiffList = new Hashtable();
            if (!wasInsertInDiffList.ContainsKey(action))
                wasInsertInDiffList.Add(action, true);
        }

        public void ResetWasInsertInDiffList()
        {
            this.wasInsertInDiffList = null;
        }

        /// <summary>
        /// GUID unico que identifica al objeto.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Tipo de objeto (Tabla, Column, Vista, etc)
        /// </summary>
        public Enums.ObjectType ObjectType { get; set; }

        /// <summary>
        /// ID del objeto.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre completo del objeto, incluyendo el owner.
        /// </summary>
        public virtual string FullName
        {
            get
            {
                if (String.IsNullOrEmpty(Owner))
                    return nameCharacterOpen + Name + nameCharacterClose;
                else
                    return nameCharacterOpen + Owner + nameCharacterClose + "." + nameCharacterOpen + Name + nameCharacterClose;
            }
        }

        /// <summary>
        /// Nombre de usuario del owner de la tabla.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Nombre del objecto
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Determine if the database object if a System object or not
        /// </summary>
        public Boolean IsSystem { get; set; }

        /// <summary>
        /// Indica el estado del objeto (si es propio, si debe borrarse o si es nuevo). Es solo valido
        /// para generar el SQL de diferencias entre 2 bases.
        /// Por defecto es siempre Original.
        /// </summary>
        public virtual Enums.ObjectStatusType Status
        {
            get { return status; }
            set
            {
                if ((status != Enums.ObjectStatusType.RebuildStatus) && (status != Enums.ObjectStatusType.RebuildDependenciesStatus))
                    status = value;
                if (Parent != null)
                {
                    //Si el estado de la tabla era el original, lo cambia, sino deja el actual estado.
                    if (Parent.Status == Enums.ObjectStatusType.OriginalStatus || value == Enums.ObjectStatusType.RebuildStatus || value == Enums.ObjectStatusType.RebuildDependenciesStatus)
                    {
                        if ((value != Enums.ObjectStatusType.OriginalStatus) && (value != Enums.ObjectStatusType.RebuildStatus) && (value != Enums.ObjectStatusType.RebuildDependenciesStatus))
                            Parent.Status = Enums.ObjectStatusType.AlterStatus;
                        if (value == Enums.ObjectStatusType.RebuildDependenciesStatus)
                            Parent.Status = Enums.ObjectStatusType.RebuildDependenciesStatus;
                        if (value == Enums.ObjectStatusType.RebuildStatus)
                            Parent.Status = Enums.ObjectStatusType.RebuildStatus;
                    }
                }
            }
        }

        public Boolean HasState(Enums.ObjectStatusType statusFind)
        {
            return ((this.Status & statusFind) == statusFind);
        }

        public virtual Boolean IsCodeType
        {
            get { return false; }
        }

        public virtual int DependenciesCount
        {
            get { return 0; }
        }

        /// <summary>
        /// Get if the SQL commands for the collection must build in one single statement
        /// or one statmente for each item of the collection.
        /// </summary>
        public virtual Boolean MustBuildSqlInLine
        {
            get { return false; }
        }
    }
}
