using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareStoreProcedures:CompareBase<StoreProcedure>
    {
        public static void GenerateDiferences(SchemaList<StoreProcedure, Database> CamposOrigen, SchemaList<StoreProcedure, Database> CamposDestino)
        {
            foreach (StoreProcedure node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    StoreProcedure newNode = node;//.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!node.Compare(CamposOrigen[node.FullName]))
                    {
                        StoreProcedure newNode = node;//.Clone(CamposOrigen.Parent);
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }

            MarkDrop(CamposOrigen, CamposDestino);
        }
    }
}
