using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareStoreProcedures:CompareBase<StoreProcedure>
    {
        protected override void DoUpdate<Root>(SchemaList<StoreProcedure, Root> CamposOrigen, StoreProcedure node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                StoreProcedure newNode = node;//.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
