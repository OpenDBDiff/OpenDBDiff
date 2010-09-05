using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareAssemblies : CompareBase<Assembly>
    {
        public static void GenerateDiferences(SchemaList<Assembly, Database> CamposOrigen, SchemaList<Assembly, Database> CamposDestino)
        {
            foreach (Assembly node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    Assembly newNode = (Assembly)node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!node.Compare(CamposOrigen[node.FullName]))
                    {
                        Assembly newNode = (Assembly)node.Clone(CamposOrigen.Parent);
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;

                        if (node.Text.Equals(CamposOrigen[node.FullName].Text))
                        {                            
                            if (!node.PermissionSet.Equals(CamposOrigen[node.FullName].PermissionSet))
                                newNode.Status += (int)Enums.ObjectStatusType.PermisionSet;
                            if (!node.Owner.Equals(CamposOrigen[node.FullName].Owner))
                                newNode.Status += (int)Enums.ObjectStatusType.ChangeOwner;
                        }
                        else
                            newNode.Status = Enums.ObjectStatusType.AlterRebuildStatus;
                        
                        CamposOrigen[node.FullName].Files.ForEach(item =>
                        {
                            if (!newNode.Files.Exists(item.FullName))
                                newNode.Files.Add(new AssemblyFile(newNode, item, Enums.ObjectStatusType.DropStatus));
                            else
                                item.Status = Enums.ObjectStatusType.AlterStatus;
                        });
                        newNode.Files.ForEach(item =>
                        {
                            if (!CamposOrigen[node.FullName].Files.Exists(item.FullName))
                            {
                                item.Status = Enums.ObjectStatusType.CreateStatus;
                            }
                        });                        
                        CompareExtendedProperties(CamposOrigen[node.FullName],newNode);
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }

            MarkDrop(CamposOrigen, CamposDestino);            
        }
    }
}
