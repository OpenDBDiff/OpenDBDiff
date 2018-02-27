﻿using System;
using System.Collections.Generic;

namespace OpenDBDiff.Schema.Model
{
    public class SearchSchemaBase
    {
        private Dictionary<String, ObjectType> objectTypes;
        private Dictionary<String, String> objectParent;
        private Dictionary<Int32, String> objectId;

        public SearchSchemaBase()
        {
            objectTypes = new Dictionary<string, ObjectType>();
            objectParent = new Dictionary<string, string>();
            objectId = new Dictionary<Int32, string>();
        }

        public void Add(ISchemaBase item)
        {
            if (objectTypes.ContainsKey(item.FullName.ToUpper()))
                objectTypes.Remove(item.FullName.ToUpper());
            objectTypes.Add(item.FullName.ToUpper(), item.ObjectType);
            if ((item.ObjectType == ObjectType.Constraint) || (item.ObjectType == ObjectType.Index) || (item.ObjectType == ObjectType.Trigger) || (item.ObjectType == ObjectType.CLRTrigger))
            {
                if (objectParent.ContainsKey(item.FullName.ToUpper()))
                    objectParent.Remove(item.FullName.ToUpper());
                objectParent.Add(item.FullName.ToUpper(), item.Parent.FullName);

                if (objectId.ContainsKey(item.Id))
                    objectId.Remove(item.Id);
                objectId.Add(item.Id, item.FullName);
            }
        }


        public Nullable<ObjectType> GetType(string FullName)
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
