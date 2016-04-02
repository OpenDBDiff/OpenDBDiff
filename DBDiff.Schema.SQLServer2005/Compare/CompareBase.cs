using System;
using System.Collections.Generic;
using System.Linq;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Generates;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal abstract class CompareBase<T> where T : ISchemaBase
    {
        protected virtual void DoUpdate<Root>(SchemaList<T, Root> originFields, T node) where Root : ISchemaBase
        {

        }

        protected virtual void DoNew<Root>(SchemaList<T, Root> originFields, T node) where Root : ISchemaBase
        {
            T newNode = node;//.Clone(originFields.Parent);
            newNode.Parent = originFields.Parent;
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            originFields.Add(newNode);
        }

        protected void DoDelete(T node)
        {
            node.Status = Enums.ObjectStatusType.DropStatus;
        }

        public void GenerateDifferences<Root>(SchemaList<T, Root> originFields, SchemaList<T, Root> destinationFields) where Root : ISchemaBase
        {
            bool has = true;
            int destinationIndex = 0;
            int originIndex = 0;
            int destinationCount = destinationFields.Count;
            int originCount = originFields.Count;
            T node;

            while (has)
            {
                has = false;
                if (destinationCount > destinationIndex)
                {
                    node = destinationFields[destinationIndex];
                    Generate.RaiseOnCompareProgress("Comparing Destination {0}: [{1}]", node.ObjectType, node.Name);
                    if (!originFields.Exists(node.FullName))
                    {
                        Generate.RaiseOnCompareProgress("Adding {0}: [{1}]", node.ObjectType, node.Name);
                        DoNew<Root>(originFields, node);
                    }
                    else
                    {
                        Generate.RaiseOnCompareProgress("Updating {0}: [{1}]", node.ObjectType, node.Name);
                        DoUpdate<Root>(originFields, node);
                    }

                    destinationIndex++;
                    has = true;
                }

                if (originCount > originIndex)
                {
                    node = originFields[originIndex];
                    Generate.RaiseOnCompareProgress("Comparing Source {0}: [{1}]", node.ObjectType, node.Name);
                    if (!destinationFields.Exists(node.FullName))
                    {
                        Generate.RaiseOnCompareProgress("Deleting {0}: [{1}]", node.ObjectType, node.Name);
                        DoDelete(node);
                    }

                    originIndex++;
                    has = true;
                }
            }
        }

        protected static void CompareExtendedProperties(ISQLServerSchemaBase origin, ISQLServerSchemaBase destination)
        {
            List<ExtendedProperty> dropList = (from node in origin.ExtendedProperties
                                               where !destination.ExtendedProperties.Exists(item => item.Name.Equals(node.Name, StringComparison.CurrentCultureIgnoreCase))
                                               select node).ToList<ExtendedProperty>();
            List<ExtendedProperty> addList = (from node in destination.ExtendedProperties
                                              where !origin.ExtendedProperties.Exists(item => item.Name.Equals(node.Name, StringComparison.CurrentCultureIgnoreCase))
                                              select node).ToList<ExtendedProperty>();
            dropList.ForEach(item => { item.Status = Enums.ObjectStatusType.DropStatus; });
            addList.ForEach(item => { item.Status = Enums.ObjectStatusType.CreateStatus; });
            origin.ExtendedProperties.AddRange(addList);
        }
    }
}
