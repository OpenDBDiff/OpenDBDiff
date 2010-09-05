using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareUserDataTypes:CompareBase<UserDataType>
    {
        public static UserDataTypes GenerateDiferences(UserDataTypes CamposOrigen, UserDataTypes CamposDestino)
        {
            foreach (UserDataType node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    node.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    CamposOrigen.Add(node);
                }
                else
                {
                    if (!UserDataType.Compare(node, CamposOrigen[node.FullName]))
                    {
                        UserDataType newNode = node.Clone(CamposOrigen.Parent);
                        if (!UserDataType.CompareDefault(node, CamposOrigen[node.FullName]))
                        {
                            if (!String.IsNullOrEmpty(node.Default.Name))
                                newNode.Default.Status = StatusEnum.ObjectStatusType.CreateStatus;
                            else
                                newNode.Default.Status = StatusEnum.ObjectStatusType.DropStatus;
                            newNode.Status = StatusEnum.ObjectStatusType.AlterStatus;
                        }
                        else
                        {
                            if (!UserDataType.CompareRule(node, CamposOrigen[node.FullName]))
                            {
                                if (!String.IsNullOrEmpty(node.Rule.Name))
                                    newNode.Rule.Status = StatusEnum.ObjectStatusType.CreateStatus;
                                else
                                    newNode.Rule.Status = StatusEnum.ObjectStatusType.DropStatus;
                                newNode.Status = StatusEnum.ObjectStatusType.AlterStatus;
                            }
                            else
                                newNode.Status = StatusEnum.ObjectStatusType.AlterRebuildStatus;
                        }                            
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }

            MarkDrop(CamposOrigen, CamposDestino);

            return CamposOrigen;
        }
    }
}
