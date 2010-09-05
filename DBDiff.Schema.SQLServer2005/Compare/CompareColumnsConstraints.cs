using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareColumnsConstraints:CompareBase<ColumnConstraint>
    {
        public static ColumnConstraints GenerateDiferences(ColumnConstraints CamposOrigen, ColumnConstraints CamposDestino)
        {
            foreach (ColumnConstraint node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    node.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    CamposOrigen.Parent.Status = StatusEnum.ObjectStatusType.OriginalStatus;
                    CamposOrigen.Parent.Parent.Status = StatusEnum.ObjectStatusType.AlterStatus;
                    CamposOrigen.Add(node);
                }
                else
                {
                    if (!ColumnConstraint.Compare(CamposOrigen[node.FullName], node))
                    {
                        ColumnConstraint newNode = node.Clone(CamposOrigen.Parent);
                        //Indico que hay un ALTER TABLE, pero sobre la columna, no seteo ningun estado.
                        newNode.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        newNode.Parent.Status = StatusEnum.ObjectStatusType.OriginalStatus;
                        newNode.Parent.Parent.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.FullName] = newNode;
                        
                    }
                }
            }

            MarkDrop(CamposOrigen, CamposDestino, node => 
            {
                node.Status = StatusEnum.ObjectStatusType.DropStatus;
                CamposOrigen.Parent.Status = StatusEnum.ObjectStatusType.OriginalStatus;
                CamposOrigen.Parent.Parent.Status = StatusEnum.ObjectStatusType.AlterStatus;
            }
            );

            return CamposOrigen;
        }
    }
}
