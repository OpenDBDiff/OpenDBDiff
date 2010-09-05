using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.MySQL.Model;

namespace DBDiff.Schema.MySQL.Compare
{
    internal static class CompareColumns
    {
        public static Columns GenerateDiferences(Columns CamposOrigen, Columns CamposDestino)
        {
            foreach (Column node in CamposDestino)
            {                
                if (!CamposOrigen.Find(node.Name))
                {
                    node.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    CamposOrigen.Parent.Status = StatusEnum.ObjectStatusType.AlterStatus;
                    CamposOrigen.Add(node);                    
                }
                else
                {
                    if (!Column.Compare(CamposOrigen[node.Name],node))
                    {                        
                        node.Status = StatusEnum.ObjectStatusType.AlterRebuildStatus;
                        CamposOrigen[node.Name].Parent.Status = StatusEnum.ObjectStatusType.AlterRebuildStatus;
                        CamposOrigen[node.Name] = node.Clone((Table)CamposOrigen[node.Name].Parent);
                    }
                }
            }
            foreach (Column node in CamposOrigen)
            {
                if (!CamposDestino.Find(node.Name))
                {
                    node.Status = StatusEnum.ObjectStatusType.DropStatus;
                    CamposOrigen.Parent.Status = StatusEnum.ObjectStatusType.AlterStatus;
                }
            }
            return CamposOrigen;
        }
    }
}

