using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareUserDataTypes:CompareBase<UserDataType>
    {
        protected override void DoNew<Root>(SchemaList<UserDataType, Root> CamposOrigen, UserDataType node)
        {
            UserDataType newNode = node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            Boolean HasAssembly = CamposOrigen.Exists(item => item.AssemblyFullName.Equals(node.AssemblyFullName) && item.IsAssembly);
            if (HasAssembly)
                newNode.Status += (int)Enums.ObjectStatusType.DropOlderStatus;
            CamposOrigen.Add(newNode);            
        }

        protected override void DoUpdate<Root>(SchemaList<UserDataType, Root> CamposOrigen, UserDataType node)
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
                        newNode.Status = Enums.ObjectStatusType.RebuildStatus;
                }
                CamposOrigen[node.FullName] = newNode;
            }            
        }
    }
}
