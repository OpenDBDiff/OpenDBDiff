using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareTriggers:CompareBase<Trigger>
    {
        public static Triggers GenerateDiferences(Triggers CamposOrigen, Triggers CamposDestino)
        {
            foreach (Trigger node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    Trigger newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!Trigger.Compare(node, CamposOrigen[node.FullName]))
                    {
                        Trigger newNode = node.Clone(CamposOrigen.Parent);
                        if (!newNode.Text.Equals(CamposOrigen[node.FullName].Text))
                            newNode.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        if (node.IsDisabled != CamposOrigen[node.FullName].IsDisabled)
                            newNode.Status = newNode.Status + (int)StatusEnum.ObjectStatusType.DisabledStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }
            
            MarkDrop(CamposOrigen, CamposDestino);

            return CamposOrigen;
        }
    }
}
