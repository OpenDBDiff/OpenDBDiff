using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal abstract class CompareBase<T> where T:ISchemaBase
    {
        protected virtual void DoUpdate<Root>(SchemaList<T, Root> CamposOrigen, T node) where Root:ISchemaBase
        {

        }

        protected virtual void DoNew<Root>(SchemaList<T, Root> CamposOrigen, T node) where Root : ISchemaBase
        {
            T newNode = node;//.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }

        protected virtual void DoDelete(T node)
        {
            node.Status = Enums.ObjectStatusType.DropStatus;
        }

        public virtual void GenerateDiferences<Root>(SchemaList<T, Root> CamposOrigen, SchemaList<T, Root> CamposDestino) where Root : ISchemaBase
        {
            bool has = true;
            int DestinoIndex = 0;
            int OrigenIndex = 0;
            int DestinoCount = CamposDestino.Count;
            int OrigenCount = CamposOrigen.Count;
            T node;

            while (has)
            {
                has = false;
                if (DestinoCount > DestinoIndex)
                {
                    node = CamposDestino[DestinoIndex];
                    if (!CamposOrigen.Exists(node.FullName))
                        DoNew<Root>(CamposOrigen, node);
                    else
                        DoUpdate<Root>(CamposOrigen, node);

                    DestinoIndex++;
                    has = true;
                }

                if (OrigenCount > OrigenIndex)
                {
                    node = CamposOrigen[OrigenIndex];
                    if (!CamposDestino.Exists(node.FullName))
                        DoDelete(node);

                    OrigenIndex++;
                    has = true;
                }
            }
        }

        public virtual void GenerateDiferences(SchemaList<T, Database> CamposOrigen, SchemaList<T, Database> CamposDestino)
        {
            GenerateDiferences<Database>(CamposOrigen, CamposDestino); 
        }

        protected static void MarkDrop(List<T> origen, List<T> destino)
        {
            MarkDrop(origen, destino, node => node.Status = Enums.ObjectStatusType.DropStatus);
        }

        protected static void MarkDrop(List<T> origen, List<T> destino, Action<T> action)
        {
            List<T> dropList = (from node in origen
                                where !destino.Exists(item => (item.CompareFullNameTo(node.FullName, item.FullName) == 0))
                                select node).ToList<T>();
            dropList.ForEach(action);
        }

        protected static void CompareExtendedProperties(ISQLServerSchemaBase origen, ISQLServerSchemaBase destino)
        {
            List<ExtendedProperty> dropList = (from node in origen.ExtendedProperties
                                               where !destino.ExtendedProperties.Exists(item => item.Name.Equals(node.Name, StringComparison.CurrentCultureIgnoreCase))
                                               select node).ToList<ExtendedProperty>();
            List<ExtendedProperty> addList = (from node in destino.ExtendedProperties
                                              where !origen.ExtendedProperties.Exists(item => item.Name.Equals(node.Name, StringComparison.CurrentCultureIgnoreCase))
                                               select node).ToList<ExtendedProperty>();
            dropList.ForEach(item => { item.Status = Enums.ObjectStatusType.DropStatus;} );
            addList.ForEach(item => { item.Status = Enums.ObjectStatusType.CreateStatus; });
            origen.ExtendedProperties.AddRange(addList);
        }
    }
}
