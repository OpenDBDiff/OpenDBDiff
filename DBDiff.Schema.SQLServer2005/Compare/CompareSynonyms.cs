using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareSynonyms:CompareBase<Synonym>
    {
        public static void GenerateDiferences(Synonyms CamposOrigen, Synonyms CamposDestino)
        {
            foreach (Synonym node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    Synonym newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!Synonym.Compare(node, CamposOrigen[node.FullName]))
                    {
                        Synonym newNode = node.Clone(CamposOrigen.Parent);
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }

            MarkDrop(CamposOrigen, CamposDestino);
        }
    }
}
