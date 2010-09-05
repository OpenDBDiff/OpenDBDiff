using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareXMLSchemas:CompareBase<XMLSchema>
    {
        public static XMLSchemas GenerateDiferences(XMLSchemas CamposOrigen, XMLSchemas CamposDestino)
        {
            foreach (XMLSchema node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    node.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(node);
                }
            }

            MarkDrop(CamposOrigen, CamposDestino);

            return CamposOrigen;
        }
    }
}
