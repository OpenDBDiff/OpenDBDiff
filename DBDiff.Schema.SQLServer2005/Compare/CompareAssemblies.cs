using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareAssemblies : CompareBase<Assembly>
    {
        public static void GenerateDiferences(Assemblys CamposOrigen, Assemblys CamposDestino)
        {
            foreach (Assembly node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    Assembly newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!Assembly.Compare(node, CamposOrigen[node.FullName]))
                    {
                        Assembly newNode = node.Clone(CamposOrigen.Parent);
                        if (node.Content.Equals(CamposOrigen[node.FullName].Content))
                        {
                            if (!node.PermissionSet.Equals(CamposOrigen[node.FullName].PermissionSet))
                                newNode.Status += (int)Enums.ObjectStatusType.AlterStatus;
                            if (!node.Owner.Equals(CamposOrigen[node.FullName].Owner))
                                newNode.Status += (int)Enums.ObjectStatusType.ChangeOwner;
                        }
                        else
                            newNode.Status = Enums.ObjectStatusType.AlterRebuildStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }

            MarkDrop(CamposOrigen, CamposDestino);
        }
    }
}
