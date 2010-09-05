using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal abstract class CompareBase<T> where T:ISchemaBase
    {
        protected static void MarkDrop(List<T> origen, List<T> destino)
        {
            MarkDrop(origen, destino, node => node.Status = Enums.ObjectStatusType.DropStatus);
        }

        protected static void MarkDrop(List<T> origen, List<T> destino, Action<T> action)
        {
            List<T> dropList = (from node in origen
                                where !destino.Exists(item => item.FullName.Equals(node.FullName))
                                select node).ToList<T>();
            dropList.ForEach(action);
        }

        protected static void CompareExtendedProperties(ISQLServerSchemaBase origen, ISQLServerSchemaBase destino)
        {
            List<ExtendedProperty> dropList = (from node in origen.ExtendedProperties
                                               where !destino.ExtendedProperties.Exists(item => item.Name.Equals(node.Name))
                                               select node).ToList<ExtendedProperty>();
            List<ExtendedProperty> addList = (from node in destino.ExtendedProperties
                                               where !origen.ExtendedProperties.Exists(item => item.Name.Equals(node.Name))
                                               select node).ToList<ExtendedProperty>();
            dropList.ForEach(item => { item.Status = Enums.ObjectStatusType.DropStatus;} );
            addList.ForEach(item => { item.Status = Enums.ObjectStatusType.CreateStatus; });
            origen.ExtendedProperties.AddRange(addList);
        }
    }
}
