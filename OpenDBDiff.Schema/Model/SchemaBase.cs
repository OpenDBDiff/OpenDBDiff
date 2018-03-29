using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenDBDiff.Schema.Model
{
    [DebuggerDisplay("Id: {Id} - Name: {Name} - Status: {status}")]
    public abstract class SchemaBase : ISchemaBase
    {
        private ObjectStatus status;
        private ISchemaBase parent;
        private string nameCharacterOpen;
        private string nameCharacterClose;
        private Hashtable wasInsertInDiffList;
        private IDatabase rootParent = null;

        protected SchemaBase(string nameCharacterOpen, string nameCharacterClose, ObjectType objectType)
        {
            this.Guid = System.Guid.NewGuid().ToString();
            this.ObjectType = objectType;
            this.status = ObjectStatus.Original;
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
        /// Instance's parent object
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
        /// Returns the list of SQL Scripts to execute to sync for the specified schemas
        /// </summary>
        /// <returns>
        /// A list (<see cref="SQLScriptList"/>) of scripts to run to sync
        /// </returns>
        public virtual SQLScriptList ToSqlDiff(ICollection<ISchemaBase> schemas)
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
        /// Returns if the obecet was already inserted in the list of scripts with differencies
        /// </summary>
        /// <param name="action">The action to check in the list</param>
        /// <returns>True if is already inserted. False if it wasn't</returns>
        public Boolean GetWasInsertInDiffList(ScriptAction action)
        {
            if (wasInsertInDiffList != null)
                return (wasInsertInDiffList.ContainsKey(action));
            else
                return false;
        }

        /// <summary>
        /// Sets the object as inserted in the list of differences script
        /// </summary>
        public void SetWasInsertInDiffList(ScriptAction action)
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
        /// Unique GUID identifying the object
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Object type. <seealso cref="ObjectType"/>
        /// </summary>
        public ObjectType ObjectType { get; set; }

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
        /// Username of the owner of the object
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Nombre of the object
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Determine if the database object if a System object or not
        /// </summary>
        public Boolean IsSystem { get; set; }

		/// <summary>
		/// Returns the status of the object. By default is set to <see cref="ObjectStatus.Original"/>. When setting a value, it also affects to the <see cref="Parent"/> status.
		/// </summary>
		public virtual ObjectStatus Status
        {
            get { return status; }
            set
            {
                if (status != ObjectStatus.Rebuild && status != ObjectStatus.RebuildDependencies)
                    status = value;

                if (Parent == null) return;

                // Si el estado de la tabla era el original, lo cambia, sino deja el actual estado.
                // If the state of the table was the original, it changes it, but leaves the current state. (Google translated)
                if (Parent.Status == ObjectStatus.Original
                    || value == ObjectStatus.Rebuild
                    || value == ObjectStatus.RebuildDependencies)
                {
                    switch (value)
                    {
                        case ObjectStatus.RebuildDependencies:
                        case ObjectStatus.Rebuild:
                            Parent.Status = value;
                            break;

                        case ObjectStatus.Original:
                            break;

                        default:
                            Parent.Status = ObjectStatus.Alter;
                            break;
                    }
                }
            }
        }

		public Boolean HasState(ObjectStatus statusFind)
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

        public virtual bool HasDependencies
        {
            get { return DependenciesCount > 0; }
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
