using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareStoreProcedures:CompareBase<StoreProcedure>
    {
        public static StoreProcedures GenerateDiferences(StoreProcedures CamposOrigen, StoreProcedures CamposDestino)
        {
            foreach (StoreProcedure node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    StoreProcedure newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!StoreProcedure.Compare(node, CamposOrigen[node.FullName]))
                    {
                        StoreProcedure newNode = node.Clone(CamposOrigen.Parent);
                        newNode.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }

            MarkDrop(CamposOrigen, CamposDestino);

            return CamposOrigen;
        }
    }
}
