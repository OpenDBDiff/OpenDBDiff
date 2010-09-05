using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareTablesOptions:CompareBase<TableOption>
    {
        protected override void DoNew<Root>(SchemaList<TableOption, Root> CamposOrigen, TableOption node)
        {
            TableOption newNode = (TableOption)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<TableOption, Root> CamposOrigen, TableOption node)
        {
            if (!TableOption.Compare(node, CamposOrigen[node.FullName]))
            {
                TableOption newNode = (TableOption)node.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }       
    }
}
