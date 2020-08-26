using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class CompareAssemblies : CompareBase<Assembly>
    {
        protected override void DoUpdate<Root>(SchemaList<Assembly, Root> originFields, Assembly node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                Assembly newNode = (Assembly)node.Clone(originFields.Parent);
                newNode.Status = ObjectStatus.Alter;

                if (node.Text.Equals(originFields[node.FullName].Text))
                {
                    if (!node.PermissionSet.Equals(originFields[node.FullName].PermissionSet))
                        newNode.Status += (int)ObjectStatus.PermissionSet;
                    if (!node.Owner.Equals(originFields[node.FullName].Owner))
                        newNode.Status += (int)ObjectStatus.ChangeOwner;
                }
                else
                    newNode.Status = ObjectStatus.Rebuild;

                originFields[node.FullName].Files.ForEach(item =>
                {
                    if (!newNode.Files.Contains(item.FullName))
                        newNode.Files.Add(new AssemblyFile(newNode, item, ObjectStatus.Drop));
                    else
                        item.Status = ObjectStatus.Alter;
                });
                newNode.Files.ForEach(item =>
                {
                    if (!originFields[node.FullName].Files.Contains(item.FullName))
                    {
                        item.Status = ObjectStatus.Create;
                    }
                });
                CompareExtendedProperties(originFields[node.FullName], newNode);
                originFields[node.FullName] = newNode;
            }
        }

        protected override void DoNew<Root>(SchemaList<Assembly, Root> originFields, Assembly node)
        {
            bool pass = true;
            Assembly newNode = (Assembly)node.Clone(originFields.Parent);
            if ((((Database)newNode.RootParent).Info.Version == DatabaseInfo.SQLServerVersion.SQLServer2005)
                && (((Database)node.RootParent).Info.Version == DatabaseInfo.SQLServerVersion.SQLServer2008))
                pass = node.FullName.Equals("Microsoft.SqlServer.Types");
            if (pass)
            {
                newNode.Status = ObjectStatus.Create;
                originFields.Add(newNode);
            }
        }
    }
}
