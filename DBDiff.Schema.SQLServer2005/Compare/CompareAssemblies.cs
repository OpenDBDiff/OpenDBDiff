using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareAssemblies : CompareBase<Assembly>
    {
        protected override void DoUpdate<Root>(SchemaList<Assembly, Root> CamposOrigen, Assembly node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                Assembly newNode = (Assembly)node.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;

                if (node.Text.Equals(CamposOrigen[node.FullName].Text))
                {
                    if (!node.PermissionSet.Equals(CamposOrigen[node.FullName].PermissionSet))
                        newNode.Status += (int)Enums.ObjectStatusType.PermisionSet;
                    if (!node.Owner.Equals(CamposOrigen[node.FullName].Owner))
                        newNode.Status += (int)Enums.ObjectStatusType.ChangeOwner;
                }
                else
                    newNode.Status = Enums.ObjectStatusType.RebuildStatus;

                CamposOrigen[node.FullName].Files.ForEach(item =>
                {
                    if (!newNode.Files.Exists(item.FullName))
                        newNode.Files.Add(new AssemblyFile(newNode, item, Enums.ObjectStatusType.DropStatus));
                    else
                        item.Status = Enums.ObjectStatusType.AlterStatus;
                });
                newNode.Files.ForEach(item =>
                {
                    if (!CamposOrigen[node.FullName].Files.Exists(item.FullName))
                    {
                        item.Status = Enums.ObjectStatusType.CreateStatus;
                    }
                });
                CompareExtendedProperties(CamposOrigen[node.FullName], newNode);
                CamposOrigen[node.FullName] = newNode;
            }
        }

        protected override void DoNew<Root>(SchemaList<Assembly, Root> CamposOrigen, Assembly node)
        {
            bool pass = true;    
            Assembly newNode = (Assembly)node.Clone(CamposOrigen.Parent);               
            if ((((Database)newNode.RootParent).Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2005) && (((Database)node.RootParent).Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2008))
                pass = node.FullName.Equals("Microsoft.SqlServer.Types");
            if (pass)
            {            
                newNode.Status = Enums.ObjectStatusType.CreateStatus;
                CamposOrigen.Add(newNode);
            }            
        }
    }
}
