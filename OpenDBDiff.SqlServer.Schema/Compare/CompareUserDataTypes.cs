using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;
using System;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class CompareUserDataTypes : CompareBase<UserDataType>
    {
        protected override void DoNew<Root>(SchemaList<UserDataType, Root> originFields, UserDataType node)
        {
            UserDataType newNode = (UserDataType)node.Clone(originFields.Parent);
            newNode.Status = ObjectStatus.Create;
            Boolean HasAssembly = originFields.Exists(item => item.AssemblyFullName.Equals(node.AssemblyFullName) && item.IsAssembly);
            if (HasAssembly)
                newNode.Status += (int)ObjectStatus.DropOlder;
            originFields.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<UserDataType, Root> originFields, UserDataType node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                UserDataType newNode = (UserDataType)node.Clone(originFields.Parent);
                newNode.Dependencies.AddRange(originFields[node.FullName].Dependencies);

                if (!UserDataType.CompareDefault(node, originFields[node.FullName]))
                {
                    if (!String.IsNullOrEmpty(node.Default.Name))
                        newNode.Default.Status = ObjectStatus.Create;
                    else
                        newNode.Default.Status = ObjectStatus.Drop;
                    newNode.Status = ObjectStatus.Alter;
                }
                else
                {
                    if (!UserDataType.CompareRule(node, originFields[node.FullName]))
                    {
                        if (!String.IsNullOrEmpty(node.Rule.Name))
                            newNode.Rule.Status = ObjectStatus.Create;
                        else
                            newNode.Rule.Status = ObjectStatus.Drop;
                        newNode.Status = ObjectStatus.Alter;
                    }
                    else
                        newNode.Status = ObjectStatus.Rebuild;
                }
                originFields[node.FullName] = newNode;
            }
        }
    }
}
