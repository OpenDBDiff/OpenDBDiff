using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareSchemas:CompareBase<Model.Schema>
    {
        public static void GenerateDiferences(Schemas CamposOrigen, Schemas CamposDestino)
        {
            foreach (Model.Schema node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    node.Status = Enums.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(node);
                }
            }

            MarkDrop(CamposOrigen, CamposDestino);
        }
    }
}
