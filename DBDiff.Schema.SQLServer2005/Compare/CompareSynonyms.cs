using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareSynonyms:CompareBase<Synonym>
    {
        public static void GenerateDiferences(SchemaList<Synonym, Database> CamposOrigen, SchemaList<Synonym, Database> CamposDestino)
        {
            foreach (Synonym node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    Synonym newNode = (Synonym)node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!Synonym.Compare(node, CamposOrigen[node.FullName]))
                    {
                        Synonym newNode = (Synonym)node.Clone(CamposOrigen.Parent);
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }

            MarkDrop(CamposOrigen, CamposDestino);
        }
    }
}
