using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class ComparePartitionFunction:CompareBase<PartitionFunction>
    {
        protected override void DoUpdate<Root>(SchemaList<PartitionFunction, Root> CamposOrigen, PartitionFunction node)
        {
            if (!PartitionFunction.Compare(node, CamposOrigen[node.FullName]))
            {
                PartitionFunction newNode = node;//.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.RebuildStatus;
                CamposOrigen[node.FullName] = newNode;
            }
            else
            {
                if (!PartitionFunction.CompareValues(node, CamposOrigen[node.FullName]))
                {
                    PartitionFunction newNode = node.Clone(CamposOrigen.Parent);
                    if (newNode.Values.Count == CamposOrigen[node.FullName].Values.Count)
                        newNode.Status = Enums.ObjectStatusType.RebuildStatus;
                    else
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;
                    newNode.Old = CamposOrigen[node.FullName].Clone(CamposOrigen.Parent);
                    CamposOrigen[node.FullName] = newNode;
                }
            }            
        }
    }
}
