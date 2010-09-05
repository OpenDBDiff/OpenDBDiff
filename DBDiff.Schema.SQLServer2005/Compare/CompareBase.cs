using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.Model;

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
    }
}
