using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareTablesOptions:CompareBase<TableOption>
    {
        public static void GenerateDiferences(SchemaList<TableOption, Table> CamposOrigen, SchemaList<TableOption, Table> CamposDestino)
        {
            foreach (TableOption node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    TableOption newNode = (TableOption)node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!TableOption.Compare(node, CamposOrigen[node.FullName]))
                    {
                        TableOption newNode = (TableOption)node.Clone(CamposOrigen.Parent);
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }

            MarkDrop(CamposOrigen, CamposDestino);
        }
    }
}
