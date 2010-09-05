using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareConstraints:CompareBase<Constraint>
    {
        public static void GenerateDiferences(Constraints CamposOrigen, Constraints CamposDestino)
        {
            foreach (Constraint node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    Constraint newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    Constraint origen = CamposOrigen[node.FullName];
                    if (!Constraint.Compare(origen, node))
                    {
                        Constraint newNode = node.Clone(CamposOrigen.Parent);
                        if (node.IsDisabled == origen.IsDisabled)
                        {
                            newNode.Status = Enums.ObjectStatusType.AlterStatus;
                        }
                        else
                            newNode.Status = Enums.ObjectStatusType.AlterStatus + (int)Enums.ObjectStatusType.DisabledStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                    else
                    {
                        if (node.IsDisabled != origen.IsDisabled)
                        {
                            Constraint newNode = node.Clone(CamposOrigen.Parent);
                            newNode.Status = Enums.ObjectStatusType.DisabledStatus;
                            CamposOrigen[node.FullName] = newNode;
                        }
                    }
                }
            }
            
            MarkDrop(CamposOrigen, CamposDestino);
        }
    }
}
