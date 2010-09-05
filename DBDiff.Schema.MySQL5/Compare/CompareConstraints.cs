using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.MySQL.Model;

namespace DBDiff.Schema.MySQL.Compare
{
    internal static class CompareConstraints
    {
        public static Constraints GenerateDiferences(Constraints CamposOrigen, Constraints CamposDestino)
        {
            foreach (Constraint node in CamposDestino)
            {
                if (!CamposOrigen.Find(node.Name))
                {
                    Constraint newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!Constraint.Compare(CamposOrigen[node.Name], node))
                    {
                        Constraint newNode = node.Clone(CamposOrigen.Parent);
                        newNode.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.Name] = newNode;                    
                    }
                }
            }
            foreach (Constraint node in CamposOrigen)
            {
                if (!CamposDestino.Find(node.Name))
                {
                    node.Status = StatusEnum.ObjectStatusType.DropStatus;
                }
            }
            return CamposOrigen;
        }
    }
}
