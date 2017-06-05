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
            objectTypes = new Dictionary<string, Enums.ObjectType>();
            objectParent = new Dictionary<string, string>();
            objectId = new Dictionary<Int32, string>();
        }

        public void Add(ISchemaBase item)
        {
            if (objectTypes.ContainsKey(item.FullName.ToUpper()))
                objectTypes.Remove(item.FullName.ToUpper());
            objectTypes.Add(item.FullName.ToUpper(), item.ObjectType);
            if ((item.ObjectType == Enums.ObjectType.Constraint) || (item.ObjectType == Enums.ObjectType.Index) || (item.ObjectType == Enums.ObjectType.Trigger) || (item.ObjectType == Enums.ObjectType.CLRTrigger))
            {
                if (objectParent.ContainsKey(item.FullName.ToUpper()))
                    objectParent.Remove(item.FullName.ToUpper());
                objectParent.Add(item.FullName.ToUpper(), item.Parent.FullName);

                if (objectId.ContainsKey(item.Id))
                    objectId.Remove(item.Id);
                objectId.Add(item.Id, item.FullName);
            }
        }


		public Nullable<Enums.ObjectType> GetType(string FullName)
		{
			if (objectTypes.ContainsKey(FullName.ToUpper()))
				return objectTypes[FullName.ToUpper()];
			return null;
		}

        public string GetParentName(string FullName)
        {
            return objectParent[FullName.ToUpper()];
        }

        public string GetFullName(int Id)
        {
            return objectId[Id];
        }
    }
}
