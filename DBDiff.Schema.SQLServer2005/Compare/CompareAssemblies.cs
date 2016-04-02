using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareAssemblies : CompareBase<Assembly>
    {
        protected override void DoUpdate<Root>(SchemaList<Assembly, Root> originFields, Assembly node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                Assembly newNode = (Assembly)node.Clone(originFields.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;

                if (node.Text.Equals(originFields[node.FullName].Text))
                {
                    if (!node.PermissionSet.Equals(originFields[node.FullName].PermissionSet))
                        newNode.Status += (int)Enums.ObjectStatusType.PermissionSet;
                    if (!node.Owner.Equals(originFields[node.FullName].Owner))
                        newNode.Status += (int)Enums.ObjectStatusType.ChangeOwner;
                }
                else
                    newNode.Status = Enums.ObjectStatusType.RebuildStatus;

                originFields[node.FullName].Files.ForEach(item =>
                {
                    if (!newNode.Files.Exists(item.FullName))
                        newNode.Files.Add(new AssemblyFile(newNode, item, Enums.ObjectStatusType.DropStatus));
                    else
                        item.Status = Enums.ObjectStatusType.AlterStatus;
                });
                newNode.Files.ForEach(item =>
                {
                    if (!originFields[node.FullName].Files.Exists(item.FullName))
                    {
                        item.Status = Enums.ObjectStatusType.CreateStatus;
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
            if ((((Database)newNode.RootParent).Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2005)
                && (((Database)node.RootParent).Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2008))
                pass = node.FullName.Equals("Microsoft.SqlServer.Types");
            if (pass)
            {
                newNode.Status = Enums.ObjectStatusType.CreateStatus;
                originFields.Add(newNode);
            }
        }
    }
}
