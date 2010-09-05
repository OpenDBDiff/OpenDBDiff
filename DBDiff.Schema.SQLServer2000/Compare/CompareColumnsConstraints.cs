using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer2000.Model;

namespace DBDiff.Schema.SQLServer2000.Compare
{
    internal class CompareColumnsConstraints
    {
        public ColumnConstraints GenerateDiferences(ColumnConstraints CamposOrigen, ColumnConstraints CamposDestino)
        {
            foreach (ColumnConstraint node in CamposDestino)
            {
                if (!CamposOrigen.Find(node.Name))
                {
                    node.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    CamposOrigen.Parent.Status = StatusEnum.ObjectStatusType.OriginalStatus;
                    CamposOrigen.Parent.Parent.Status = StatusEnum.ObjectStatusType.AlterStatus;
                    CamposOrigen.Add(node);
                }
                else
                {
                    if (!ColumnConstraint.Compare(CamposOrigen[node.Name], node))
                    {
                        node.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        //Indico que hay un ALTER TABLE, pero sobre la columna, no seteo ningun estado.
                        CamposOrigen[node.Name].Parent.Parent.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.Name] = node.Clone((Column)CamposOrigen[node.Name].Parent);
                    }
                }
            }
            foreach (ColumnConstraint node in CamposOrigen)
            {
                if (!CamposDestino.Find(node.Name))
                {
                    node.Status = StatusEnum.ObjectStatusType.DropStatus;
                    CamposOrigen.Parent.Status = StatusEnum.ObjectStatusType.OriginalStatus;
                    CamposOrigen.Parent.Parent.Status = StatusEnum.ObjectStatusType.AlterStatus;
                }
            }
            return CamposOrigen;
        }
    }
}
