using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareRules:CompareBase<Rule>
    {
        protected override void DoUpdate<Root>(SchemaList<Rule, Root> CamposOrigen, Rule node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                Rule newNode = node.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }

        protected override void DoNew<Root>(SchemaList<Rule, Root> CamposOrigen, Rule node)
        {
            Rule newNode = node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }
    }
}