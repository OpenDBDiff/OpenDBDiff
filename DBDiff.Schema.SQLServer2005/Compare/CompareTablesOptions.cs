using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareTablesOptions:CompareBase<TableOption>
    {
        public static void GenerateDiferences(TableOptions CamposOrigen, TableOptions CamposDestino)
        {
            foreach (TableOption node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    TableOption newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!TableOption.Compare(node, CamposOrigen[node.FullName]))
                    {
                        TableOption newNode = node.Clone(CamposOrigen.Parent);
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }

            MarkDrop(CamposOrigen, CamposDestino);
        }
    }
}
