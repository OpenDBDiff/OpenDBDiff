using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer2000.Model;

namespace DBDiff.Schema.SQLServer2000.Compare
{
    public class CompareTablesOptions
    {
        public TableOptions GenerateDiferences(TableOptions CamposOrigen, TableOptions CamposDestino)
        {
            foreach (TableOption node in CamposDestino)
            {
                if (!CamposOrigen.Find(node.Name))
                {
                    node.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(node);
                }
            }
            foreach (TableOption node in CamposOrigen)
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
