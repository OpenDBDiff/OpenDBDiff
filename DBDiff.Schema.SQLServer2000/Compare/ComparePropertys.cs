using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer2000.Schema;

namespace DBDiff.Schema.SQLServer2000.Compare
{
    public class ComparePropertys
    {
        public TableOptions GenerateDiferences(TableOptions CamposOrigen, TableOptions CamposDestino)
        {
            foreach (TableOption node in CamposDestino)
            {
                if (!CamposOrigen.Find(node.Name))
                {
                    node.Status = Database.ObjectStatusType.CreateStatus;
                    //Si el estado de la tabla era el original, lo cambia, sino deja el actual estado.
                    if (CamposOrigen.Parent.Status == Database.ObjectStatusType.OriginalStatus)
                        CamposOrigen.Parent.Status = Database.ObjectStatusType.AlterStatus;
                    CamposOrigen.Add(node);
                }
            }
            foreach (TableOption node in CamposOrigen)
            {
                if (!CamposDestino.Find(node.Name))
                {
                    node.Status = Database.ObjectStatusType.DropStatus;
                    //Si el estado de la tabla era el original, lo cambia, sino deja el actual estado.
                    if (CamposOrigen.Parent.Status == Database.ObjectStatusType.OriginalStatus)
                        CamposOrigen.Parent.Status = Database.ObjectStatusType.AlterStatus;
                }
            }
            return CamposOrigen;
        }
    }
}
