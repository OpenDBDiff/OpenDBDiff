using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal static class CompareColumns
    {
        public static void GenerateDiferences(Columns CamposOrigen, Columns CamposDestino)
        {
            int restPosition = 0;
            int sumPosition = 0;

            foreach (Column node in CamposOrigen)
            {
                if (!CamposDestino.Exists(node.FullName))
                {
                    node.Status = Enums.ObjectStatusType.DropStatus;
                    restPosition++;
                }
                else
                    CamposOrigen[node.FullName].Position -= restPosition;
            }
            foreach (Column node in CamposDestino)
            {                
                if (!CamposOrigen.Exists(node.FullName))
                {
                    Column newNode = node.Clone(CamposOrigen.Parent);
                    if ((newNode.Position == 1) || ((newNode.DefaultConstraint == null) && (!newNode.IsNullable) && (!newNode.IsComputed) && (!newNode.IsIdentity) && (!newNode.IsIdentityForReplication)))
                    {
                        newNode.Status = Enums.ObjectStatusType.CreateStatus;
                        newNode.Parent.Status = Enums.ObjectStatusType.RebuildStatus;
                    }
                    else
                        newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    sumPosition++;             
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    Column campoOrigen = CamposOrigen[node.FullName];
                    ColumnConstraint oldDefault = null;
                    if (campoOrigen.DefaultConstraint != null)
                        oldDefault = campoOrigen.DefaultConstraint.Clone(campoOrigen);
                    if (!Column.Compare(campoOrigen, node))
                    {
                        if (Column.CompareIdentity(campoOrigen, node))
                        {

                            if (node.HasToRebuildOnlyConstraint)
                            {
                                node.Status = Enums.ObjectStatusType.AlterStatus;
                                if ((campoOrigen.IsNullable) && (!node.IsNullable))
                                    node.Status += (int)Enums.ObjectStatusType.UpdateStatus;
                            }
                            else
                            {
                                if (node.HasToRebuild(campoOrigen.Position + sumPosition, campoOrigen.Type))
                                    node.Status = Enums.ObjectStatusType.RebuildStatus;
                                else
                                {
                                    node.Status = Enums.ObjectStatusType.AlterStatus;
                                    if ((campoOrigen.IsNullable) && (!node.IsNullable))
                                        node.Status += (int)Enums.ObjectStatusType.UpdateStatus;                                        
                                }
                            }
                            if (node.Status != Enums.ObjectStatusType.RebuildStatus)
                            {
                                if (!Column.CompareRule(campoOrigen, node))
                                {
                                    node.Status += (int)Enums.ObjectStatusType.BindStatus;
                                }
                            }
                        }
                        else
                        {
                            node.Status = Enums.ObjectStatusType.RebuildStatus;
                        }                        
                        CamposOrigen[node.FullName] = node.Clone(CamposOrigen.Parent);
                    }
                    CamposOrigen[node.FullName].DefaultConstraint = CompareColumnsConstraints.GenerateDiferences(campoOrigen, node);
                }
            }
        }
    }
}

