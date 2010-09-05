using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareConstraints:CompareBase<Constraint>
    {
        public static Constraints GenerateDiferences(Constraints CamposOrigen, Constraints CamposDestino)
        {
            foreach (Constraint node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    Constraint newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!Constraint.Compare(CamposOrigen[node.FullName], node))
                    {
                        Constraint newNode = node.Clone(CamposOrigen.Parent);
                        if (node.IsDisabled == CamposOrigen[node.FullName].IsDisabled)
                        {
                            if (newNode.HasClusteredIndex)
                            {
                                if (!Index.CompareFileGroup(CamposOrigen[node.FullName].Index, node.Index))
                                    newNode.Status = StatusEnum.ObjectStatusType.ChangeFileGroup;
                                else
                                    newNode.Status = StatusEnum.ObjectStatusType.AlterStatus;
                            }
                            else
                                newNode.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        }
                        else
                            newNode.Status = StatusEnum.ObjectStatusType.AlterDisabledStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                    else
                    {
                        if (node.IsDisabled != CamposOrigen[node.FullName].IsDisabled)
                        {
                            Constraint newNode = node.Clone(CamposOrigen.Parent);
                            newNode.Status = StatusEnum.ObjectStatusType.DisabledStatus;
                            CamposOrigen[node.FullName] = newNode;
                        }
                    }
                }
            }
            
            MarkDrop(CamposOrigen, CamposDestino);

            return CamposOrigen;
        }
    }
}
