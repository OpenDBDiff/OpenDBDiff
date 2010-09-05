using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareFileGroups:CompareBase<FileGroup>
    {
        public static void GenerateDiferences(SchemaList<FileGroup, Database> CamposOrigen, SchemaList<FileGroup, Database> CamposDestino)
        {
            foreach (FileGroup node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    FileGroup newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!FileGroup.Compare(node, CamposOrigen[node.FullName]))
                    {
                        FileGroup newNode = node.Clone(CamposOrigen.Parent);
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }
            
            MarkDrop(CamposOrigen, CamposDestino);
        }
    }
}