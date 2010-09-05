using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareIndexes:CompareBase<Index>
    {
        public static void GenerateDiferences(Indexes CamposOrigen, Indexes CamposDestino)
        {
            foreach (Index node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    Index newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!Index.Compare(node, CamposOrigen[node.FullName]))
                    {
                        Index newNode = node.Clone(CamposOrigen.Parent);
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
            }

            MarkDrop(CamposOrigen, CamposDestino);
        }
    }
}
