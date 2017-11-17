using System;
using System.Collections.Generic;

namespace DBDiff.Schema.Model
{
    public class SearchSchemaBase
    {
        private Dictionary<String, Enums.ObjectType> objectTypes;
        private Dictionary<String, String> objectParent;
        private Dictionary<Int32, String> objectId;

        public SearchSchemaBase()
        {
            objectTypes = new Dictionary<string, Enums.ObjectType>(StringComparer.OrdinalIgnoreCase);
            objectParent = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            objectId = new Dictionary<Int32, string>();
        }

        public void Add(ISchemaBase item)
        {
            objectTypes[item.FullName] = item.ObjectType;

            if ((item.ObjectType == Enums.ObjectType.Constraint) 
                || (item.ObjectType == Enums.ObjectType.Index) 
                || (item.ObjectType == Enums.ObjectType.Trigger) 
                || (item.ObjectType == Enums.ObjectType.CLRTrigger))
            {
                objectParent[item.FullName] = item.Parent.FullName;
                objectId[item.Id] = item.FullName;
            }
        }

        public Nullable<Enums.ObjectType> GetType(string FullName)
        {
            Enums.ObjectType result;
            if (objectTypes.TryGetValue(FullName, out result))
                return result;
                
            return null;
        }

        public string GetParentName(string FullName)
        {
            return objectParent[FullName];
        }

        public string GetFullName(int Id)
        {
            return objectId[Id];
        }
    }
}
