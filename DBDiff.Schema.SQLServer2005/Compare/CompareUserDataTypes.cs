using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareUserDataTypes:CompareBase<UserDataType>
    {
        public static void GenerateDiferences(SchemaList<UserDataType, Database> CamposOrigen, SchemaList<UserDataType, Database> CamposDestino)
        {
            foreach (UserDataType node in CamposDestino)
            {
                if (!CamposOrigen.Exists(node.FullName))
                {
                    UserDataType newNode = node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    Boolean HasAssembly = CamposOrigen.Exists(item => item.AssemblyFullName.Equals(node.AssemblyFullName) && item.IsAssembly);
                    if (HasAssembly)
                        newNode.Status += (int)Enums.ObjectStatusType.DropOlderStatus;
                    CamposOrigen.Add(newNode);
                }
                else
                {
                    if (!node.Compare(CamposOrigen[node.FullName]))
                    {
                        UserDataType newNode = node.Clone(CamposOrigen.Parent);
                        newNode.Dependencys.AddRange(CamposOrigen[node.FullName].Dependencys);

                        if (!UserDataType.CompareDefault(node, CamposOrigen[node.FullName]))
                        {
                            if (!String.IsNullOrEmpty(node.Default.Name))
                                newNode.Default.Status = Enums.ObjectStatusType.CreateStatus;
                            else
                                newNode.Default.Status = Enums.ObjectStatusType.DropStatus;
                            newNode.Status = Enums.ObjectStatusType.AlterStatus;
                        }
                        else
                        {
                            if (!UserDataType.CompareRule(node, CamposOrigen[node.FullName]))
                            {
                                if (!String.IsNullOrEmpty(node.Rule.Name))
                                    newNode.Rule.Status = Enums.ObjectStatusType.CreateStatus;
                                else
                                    newNode.Rule.Status = Enums.ObjectStatusType.DropStatus;
                                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                            }
                            else
                                newNode.Status = Enums.ObjectStatusType.AlterRebuildStatus;
                        }
                        CamposOrigen[node.FullName] = newNode;
                    }
                }
            }

            MarkDrop(CamposOrigen, CamposDestino);
        }
    }
}
