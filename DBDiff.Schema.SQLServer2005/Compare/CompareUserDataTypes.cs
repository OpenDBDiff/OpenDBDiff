using System;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareUserDataTypes : CompareBase<UserDataType>
    {
        protected override void DoNew<Root>(SchemaList<UserDataType, Root> originFields, UserDataType node)
        {
            UserDataType newNode = (UserDataType)node.Clone(originFields.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            Boolean HasAssembly = originFields.Exists(item => item.AssemblyFullName.Equals(node.AssemblyFullName) && item.IsAssembly);
            if (HasAssembly)
                newNode.Status += (int)Enums.ObjectStatusType.DropOlderStatus;
            originFields.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<UserDataType, Root> originFields, UserDataType node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                UserDataType newNode = (UserDataType)node.Clone(originFields.Parent);
                newNode.Dependencys.AddRange(originFields[node.FullName].Dependencys);

                if (!UserDataType.CompareDefault(node, originFields[node.FullName]))
                {
                    if (!String.IsNullOrEmpty(node.Default.Name))
                        newNode.Default.Status = Enums.ObjectStatusType.CreateStatus;
                    else
                        newNode.Default.Status = Enums.ObjectStatusType.DropStatus;
                    newNode.Status = Enums.ObjectStatusType.AlterStatus;
                }
                else
                {
                    if (!UserDataType.CompareRule(node, originFields[node.FullName]))
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
                originFields[node.FullName] = newNode;
            }
        }
    }
}
