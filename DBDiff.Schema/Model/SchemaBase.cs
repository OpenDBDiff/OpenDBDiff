using System;
using System.Collections;
using System.Text;
using System.Reflection;

namespace DBDiff.Schema.Model
{
    public abstract class SchemaBase:ISchemaBase
    {
        private StatusEnum.ObjectStatusType status;
        private StatusEnum.ObjectTypeEnum objectType;
        private ISchemaBase parent;                
        private int id;
        private string name;
        private string owner;
        private string guid;
        private string nameCharacterOpen;
        private string nameCharacterClose;
        private Hashtable wasInsertInDiffList;

        public SchemaBase(string nameCharacterOpen, string nameCharacterClose, StatusEnum.ObjectTypeEnum objectType)
        {
            this.guid = System.Guid.NewGuid().ToString();
            this.objectType = objectType;
            this.status = StatusEnum.ObjectStatusType.OriginalStatus;
            this.nameCharacterClose = nameCharacterClose;
            this.nameCharacterOpen = nameCharacterOpen;
        }
        
        protected object Clone(object vObj, ISchemaBase parentObject)
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
         } 

        /// <summary>
        /// Objeto padre de la instancia.
        /// </summary>
        public ISchemaBase Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public virtual string ToSQLDrop() { return ""; }

        public virtual string ToSQLAdd() { return ""; }
        /// <summary>
        /// Devuelve si el objeto ya fue insertado en la lista de script con diferencias.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Boolean GetWasInsertInDiffList(StatusEnum.ScripActionType action)
        {
            if (wasInsertInDiffList != null)
                return (wasInsertInDiffList.ContainsKey(action));
            else
                return false;
        }

        /// <summary>
        /// Setea que el objeto ya fue insertado en la lista de script con diferencias.
        /// </summary>
        public void SetWasInsertInDiffList(StatusEnum.ScripActionType action)
        {
            if (wasInsertInDiffList == null) wasInsertInDiffList = new Hashtable();
            if (!wasInsertInDiffList.ContainsKey(action))
                wasInsertInDiffList.Add(action, true);
        }

        /// <summary>
        /// GUID unico que identifica al objeto.
        /// </summary>
        public string Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        /// <summary>
        /// Tipo de objeto (Tabla, Column, Vista, etc)
        /// </summary>
        public StatusEnum.ObjectTypeEnum ObjectType
        {
            get { return objectType; }
            set { objectType = value; }
        }

        /// <summary>
        /// ID del objeto.
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Nombre completo del objeto, incluyendo el owner.
        /// </summary>
        public virtual string FullName
        {
            get 
            { 
                if (String.IsNullOrEmpty(owner))
                    return nameCharacterOpen + Name + nameCharacterClose; 
                else
                    return nameCharacterOpen + owner + nameCharacterClose + "." + nameCharacterOpen + Name + nameCharacterClose; 
            }
        }

        /// <summary>
        /// Nombre de usuario del owner de la tabla.
        /// </summary>
        public string Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        /// <summary>
        /// Nombre del objecto
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Indica el estado del objeto (si es propio, si debe borrarse o si es nuevo). Es solo valido
        /// para generar el SQL de diferencias entre 2 bases. 
        /// Por defecto es siempre Original.
        /// </summary>
        public virtual StatusEnum.ObjectStatusType Status
        {
            get { return status; }
            set
            {
                if ((status != StatusEnum.ObjectStatusType.AlterRebuildStatus) && (status != StatusEnum.ObjectStatusType.AlterRebuildDependeciesStatus)) status = value;
                if (Parent != null)
                {
                    //Si el estado de la tabla era el original, lo cambia, sino deja el actual estado.
                    if (Parent.Status == StatusEnum.ObjectStatusType.OriginalStatus || value == StatusEnum.ObjectStatusType.AlterRebuildStatus || value == StatusEnum.ObjectStatusType.AlterRebuildDependeciesStatus)
                    {
                        if ((value != StatusEnum.ObjectStatusType.OriginalStatus) && (value != StatusEnum.ObjectStatusType.AlterRebuildStatus) && (value != StatusEnum.ObjectStatusType.AlterRebuildDependeciesStatus))
                            Parent.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        if (value == StatusEnum.ObjectStatusType.AlterRebuildDependeciesStatus)
                            Parent.Status = StatusEnum.ObjectStatusType.AlterRebuildDependeciesStatus;
                        if (value == StatusEnum.ObjectStatusType.AlterRebuildStatus)
                            Parent.Status = StatusEnum.ObjectStatusType.AlterRebuildStatus;
                    }
                }
            }
        }

        public Boolean HasState(StatusEnum.ObjectStatusType statusFind)
        {
            return ((this.Status & statusFind) == statusFind);
        }
    }
}
