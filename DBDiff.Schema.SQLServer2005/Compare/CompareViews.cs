using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareViews:CompareBase<View>
    {
        public static void GenerateDiferences(Views CamposOrigen, Views CamposDestino)
        {
            foreach (View node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    View newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!View.Compare(node, CamposOrigen[node.FullName]))
                    {
                        View newNode = node.Clone(CamposOrigen.Parent);
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                    CompareIndexes.GenerateDiferences(CamposOrigen[node.FullName].Indexes,node.Indexes);
                }
            }

            MarkDrop(CamposOrigen, CamposDestino);
        }
    }
}
