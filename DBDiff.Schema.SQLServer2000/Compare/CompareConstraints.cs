using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer2000.Model;

namespace DBDiff.Schema.SQLServer2000.Compare
{
    internal class CompareConstraints
    {
        public Constraints GenerateDiferences(Constraints CamposOrigen, Constraints CamposDestino)
        {
            foreach (Constraint node in CamposDestino)
            {
                if (!CamposOrigen.Find(node.Name))
                {
                    node.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    CamposOrigen.Parent.Status = StatusEnum.ObjectStatusType.AlterStatus;
                    CamposOrigen.Add(node);
                }
                else
                {
                    if (!Constraint.Compare(CamposOrigen[node.Name], node))
                    {
                        node.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.Name].Parent.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.Name] = node.Clone((Table)CamposOrigen[node.Name].Parent);                        
                    }
                }
            }
            foreach (Constraint node in CamposOrigen)
            {
                if (!CamposDestino.Find(node.Name))
                {
                    node.Status = StatusEnum.ObjectStatusType.DropStatus;
                    CamposOrigen.Parent.Status = StatusEnum.ObjectStatusType.AlterStatus;
                }
            }
            return CamposOrigen;
        }
    }
}
