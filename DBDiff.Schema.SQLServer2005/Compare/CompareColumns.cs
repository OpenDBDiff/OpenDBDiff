using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal static class CompareColumns
    {
        public static Columns GenerateDiferences(Columns CamposOrigen, Columns CamposDestino)
        {
            int restPosition = 0;
            int sumPosition = 0;

            foreach (Column node in CamposOrigen)
            {
                if (!CamposDestino.Exists(node.FullName))
                {
                    node.Status = StatusEnum.ObjectStatusType.DropStatus;
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
                    if ((newNode.Position == 1) || ((newNode.Constraints.Count == 0) && (!newNode.Nullable) && (!newNode.IsComputed) && (!newNode.IsIdentity) && (!newNode.IsIdentityForReplication)))
                    {
                        newNode.Status = StatusEnum.ObjectStatusType.CreateStatus;
                        newNode.Parent.Status = StatusEnum.ObjectStatusType.AlterRebuildStatus;
                    }
                    else
                        newNode.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    sumPosition++;             
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    Column campoOrigen = CamposOrigen[node.FullName];
                    ColumnConstraints oldDefault = campoOrigen.Constraints.Clone(campoOrigen);
                    if (!Column.Compare(campoOrigen, node))
                    {
                        if (Column.CompareIdentity(campoOrigen, node))
                        {

                            if (node.HasToRebuildOnlyConstraint)
                            {
                                if ((campoOrigen.Nullable) && (!node.Nullable))
                                    node.Status = StatusEnum.ObjectStatusType.AlterStatusUpdate;
                                else
                                    node.Status = StatusEnum.ObjectStatusType.AlterStatus;
                                CamposOrigen.Parent.Status = StatusEnum.ObjectStatusType.AlterRebuildDependeciesStatus;
                            }
                            else
                            {
                                if (node.HasToRebuild(campoOrigen.Position + sumPosition, campoOrigen.Type))
                                    node.Status = StatusEnum.ObjectStatusType.AlterRebuildStatus;
                                else
                                {
                                    if ((campoOrigen.Nullable) && (!node.Nullable))
                                        node.Status = StatusEnum.ObjectStatusType.AlterStatusUpdate;
                                    else
                                        node.Status = StatusEnum.ObjectStatusType.AlterStatus;                                    
                                }
                            }
                        }
                        else
                        {
                            node.Status = StatusEnum.ObjectStatusType.AlterRebuildStatus;
                        }                        
                        CamposOrigen[node.FullName] = node.Clone(CamposOrigen.Parent);
                    }
                    CamposOrigen[node.FullName].Constraints = CompareColumnsConstraints.GenerateDiferences(oldDefault, node.Constraints);
                }
            }            
            return CamposOrigen;
        }
    }
}

