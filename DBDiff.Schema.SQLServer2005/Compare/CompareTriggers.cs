using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareTriggers:CompareBase<Trigger>
    {
        public static void GenerateDiferences(SchemaList<Trigger, Table> CamposOrigen, SchemaList<Trigger, Table> CamposDestino)
        {
            foreach (Trigger node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    Trigger newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!node.Compare(CamposOrigen[node.FullName]))
                    {
                        Trigger newNode = node.Clone(CamposOrigen.Parent);
                        if (!newNode.Text.Equals(CamposOrigen[node.FullName].Text))
                            newNode.Status = Enums.ObjectStatusType.AlterStatus;
                        if (node.IsDisabled != CamposOrigen[node.FullName].IsDisabled)
                            newNode.Status = newNode.Status + (int)Enums.ObjectStatusType.DisabledStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }
            
            MarkDrop(CamposOrigen, CamposDestino);
        }
    }
}
