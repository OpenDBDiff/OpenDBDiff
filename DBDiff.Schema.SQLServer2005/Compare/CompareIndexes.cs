using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareIndexes:CompareBase<Index>
    {
        private static void DoDelete(Index node)
        {
            node.Status = Enums.ObjectStatusType.DropStatus;
        }

        private static void DoNew<T>(SchemaList<Index, T> CamposOrigen, Index node) where T:ISchemaBase
        {
            Index newNode = (Index)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }

        private static void DoUpdate<T>(SchemaList<Index, T> CamposOrigen, Index node) where T:ISchemaBase
        {
            if (!Index.Compare(node, CamposOrigen[node.FullName]))
            {
                Index newNode = (Index)node.Clone(CamposOrigen.Parent);
                if (!Index.CompareExceptIsDisabled(node, CamposOrigen[node.FullName]))
                {
                    newNode.Status = Enums.ObjectStatusType.AlterStatus;
                }
                else
                    newNode.Status = Enums.ObjectStatusType.DisabledStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }

        public static void GenerateDiferences<T>(SchemaList<Index, T> CamposOrigen, SchemaList<Index, T> CamposDestino) where T:ISchemaBase 
        {
            bool has = true;
            int DestinoIndex = 0;
            int OrigenIndex = 0;
            int DestinoCount = CamposDestino.Count;
            int OrigenCount = CamposOrigen.Count;
            Index node;

            while (has)
            {
                has = false;
                if (DestinoCount > DestinoIndex)
                {
                    node = CamposDestino[DestinoIndex];
                    if (!CamposOrigen.Exists(node.FullName))
                        DoNew<T>(CamposOrigen, node);
                    else
                        DoUpdate<T>(CamposOrigen, node);

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
    }
}
