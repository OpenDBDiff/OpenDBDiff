using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareIndexes:CompareBase<Index>
    {
        protected override void DoNew<Root>(SchemaList<Index, Root> CamposOrigen, Index node)
        {
            Index newNode = (Index)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<Index, Root> CamposOrigen, Index node)
        {
            if (!Index.Compare(node, CamposOrigen[node.FullName]))
            {
                Index newNode = (Index)node.Clone(CamposOrigen.Parent);
                if (!Index.CompareExceptIsDisabled(node, CamposOrigen[node.FullName]))
                {
                    newNode.Status = Enums.ObjectStatusType.AlterStatus;
                }
                else
                    newNode.Status = Enums.ObjectStatusType.DisabledStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
