using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer2000.Model;

namespace DBDiff.Schema.SQLServer2000.Compare
{
    public class CompareUserDataTypes
    {
        public UserDataTypes GenerateDiferences(UserDataTypes CamposOrigen, UserDataTypes CamposDestino)
        {
            foreach (UserDataType node in CamposDestino)
            {
                if (!CamposOrigen.Find(node.FullName))
                {
                    node.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(node);
                }
            }
            foreach (UserDataType node in CamposOrigen)
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
