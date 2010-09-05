using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareIndexes:CompareBase<Index>
    {
        private static void DoDelete(Index node)
        {
            node.Status = Enums.ObjectStatusType.DropStatus;
        }

        private static void DoNew(SchemaList<Index, Table> CamposOrigen, Index node)
        {
            Index newNode = (Index)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }

        private static void DoNew(SchemaList<Index, View> CamposOrigen, Index node)
        {
            Index newNode = (Index)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }

        private static void DoUpdate(SchemaList<Index, Table> CamposOrigen, Index node)
        {
            if (!Index.Compare(node, CamposOrigen[node.FullName]))
            {
                Index newNode = (Index)node.Clone(CamposOrigen.Parent);
                if (!Index.CompareExceptIsDisabled(node, CamposOrigen[node.FullName]))
                {
                    /*if (!Index.CompareFileGroup(node,CamposOrigen[node.FullName]))
                    {
                        if (node.Type == Index.IndexTypeEnum.Clustered)
                            newNode.Status = StatusEnum.ObjectStatusType.ChangeFileGroup;   
                        else
                            newNode.Status = StatusEnum.ObjectStatusType.AlterStatus;
                    }
                    else*/
                    newNode.Status = Enums.ObjectStatusType.AlterStatus;
                }
                else
                    newNode.Status = Enums.ObjectStatusType.DisabledStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }

        private static void DoUpdate(SchemaList<Index, View> CamposOrigen, Index node)
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
                CompareExtendedProperties(newNode, CamposOrigen[node.FullName]);
                CamposOrigen[node.FullName] = newNode;
            }
        }

        public static void GenerateDiferences(SchemaList<Index, View> CamposOrigen, SchemaList<Index, View> CamposDestino)
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
                        DoNew(CamposOrigen, node);
                    else
                        DoUpdate(CamposOrigen, node);

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

        public static void GenerateDiferences(SchemaList<Index, Table> CamposOrigen, SchemaList<Index, Table> CamposDestino)
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
                        DoNew(CamposOrigen, node);
                    else
                        DoUpdate(CamposOrigen, node);

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
