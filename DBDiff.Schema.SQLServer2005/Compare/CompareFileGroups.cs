using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareFileGroups : CompareBase<FileGroup>
    {
        protected override void DoNew<Root>(SchemaList<FileGroup, Root> originFields, FileGroup node)
        {
            FileGroup newNode = (FileGroup)node.Clone(originFields.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            /*If the Logical File Name exists in another filegroup,
             * we must change the new Logical File Name.
             */
            originFields.ForEach(file =>
            {
                if (file.Status != Enums.ObjectStatusType.DropStatus)
                {
                    file.Files.ForEach(group =>
                    {
                        newNode.Files.ForEach(ngroup =>
                        {
                            if (group.CompareFullNameTo(group.FullName, ngroup.FullName) == 0)
                            {
                                newNode.Files[ngroup.FullName].Name = group.Name + "_2";
                            }
                        });
                    });
                }
            });
            originFields.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<FileGroup, Root> originFields, FileGroup node)
        {
            if (!FileGroup.Compare(node, originFields[node.FullName]))
            {
                FileGroup newNode = (FileGroup)node.Clone(originFields.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
