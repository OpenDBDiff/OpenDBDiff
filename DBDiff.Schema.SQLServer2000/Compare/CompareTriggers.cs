using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer2000.Model;

namespace DBDiff.Schema.SQLServer2000.Compare
{
    public class CompareTriggers
    {
        public TableTriggers GenerateDiferences(TableTriggers CamposOrigen, TableTriggers CamposDestino)
        {
            foreach (TableTrigger node in CamposDestino)
            {
                if (!CamposOrigen.Find(node.Name))
                {
                    node.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(node);
                }
                else
                {
                    if (!node.Text.Equals(CamposOrigen[node.Name].Text))
                    {
                        node.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.Name].Parent.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        CamposOrigen[node.Name] = node;
                    }
                }
            }
            foreach (TableTrigger node in CamposOrigen)
            {
                if (!CamposDestino.Find(node.Name))
                {
                    node.Status = StatusEnum.ObjectStatusType.DropStatus;
                }
            }
            return CamposOrigen;
        }
    }
}
