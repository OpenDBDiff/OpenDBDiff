using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.MySQL.Model;

namespace DBDiff.Schema.MySQL.Compare
{
    internal static class CompareTriggers
    {
        public static TableTriggers GenerateDiferences(TableTriggers CamposOrigen, TableTriggers CamposDestino)
        {
            foreach (TableTrigger node in CamposDestino)
            {
                if (!CamposOrigen.Find(node.FullName))
                {
                    TableTrigger newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!TableTrigger.Compare(node,CamposOrigen[node.FullName]))
                    {
                        TableTrigger newNode = node.Clone(CamposOrigen.Parent);
                        newNode.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }
            foreach (TableTrigger node in CamposOrigen)
            {
                if (!CamposDestino.Find(node.FullName))
                {
                    node.Status = StatusEnum.ObjectStatusType.DropStatus;
                }
            }
            return CamposOrigen;
        }
    }
}
