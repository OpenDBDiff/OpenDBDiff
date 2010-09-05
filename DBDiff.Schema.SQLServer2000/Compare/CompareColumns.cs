using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer2000.Model;

namespace DBDiff.Schema.SQLServer2000.Compare
{
    internal class CompareColumns
    {
        public Columns GenerateDiferences(Columns CamposOrigen, Columns CamposDestino)
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
                        if (Column.CompareIdentity(CamposOrigen[node.Name], node))
                        {
                            if (node.HasComputedDependencies || node.HasIndexDependencies || node.IsComputed)
                                node.Status = StatusEnum.ObjectStatusType.AlterRebuildStatus;
                            else
                                node.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        }
                        else
                        {
                            node.Status = StatusEnum.ObjectStatusType.AlterRebuildStatus;
                            CamposOrigen[node.Name].Parent.Status = StatusEnum.ObjectStatusType.AlterRebuildStatus;
                        }
                        CamposOrigen[node.Name] = node.Clone((Table)CamposOrigen[node.Name].Parent);
                    }
                    CamposOrigen[node.Name].Constraints = (new CompareColumnsConstraints()).GenerateDiferences(CamposOrigen[node.Name].Constraints, node.Constraints);
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

