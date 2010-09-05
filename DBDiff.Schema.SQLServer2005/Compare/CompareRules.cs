using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareRules:CompareBase<Rule>
    {
        public static void GenerateDiferences(SchemaList<Rule, Database> CamposOrigen, SchemaList<Rule, Database> CamposDestino)
        {
            foreach (Rule node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    Rule newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!node.Compare(CamposOrigen[node.FullName]))
                    {
                        Rule newNode = node.Clone(CamposOrigen.Parent);
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }
            
            MarkDrop(CamposOrigen, CamposDestino);
        }
    }
}