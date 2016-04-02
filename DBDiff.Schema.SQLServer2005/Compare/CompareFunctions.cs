using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareFunctions : CompareBase<Function>
    {
        protected override void DoNew<Root>(SchemaList<Function, Root> originFields, Function node)
        {
            Function newNode = (Function)node.Clone(originFields.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            originFields.Add(newNode);
            newNode.DependenciesIn.ForEach(dep =>
            {
                ISchemaBase item = ((Database)((ISchemaBase)originFields.Parent)).Find(dep);
                if (item != null)
                {
                    if (item.IsCodeType)
                        ((ICode)item).DependenciesOut.Add(newNode.FullName);
                }
            }
            );
        }

        protected override void DoUpdate<Root>(SchemaList<Function, Root> originFields, Function node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                Function newNode = (Function)node.Clone(originFields.Parent);
                newNode.DependenciesIn.AddRange(originFields[node.FullName].DependenciesIn);
                newNode.DependenciesOut.AddRange(originFields[node.FullName].DependenciesOut);

                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                if (newNode.IsSchemaBinding)
                    newNode.Status += (int)Enums.ObjectStatusType.RebuildDependenciesStatus;
                if (newNode.HasToRebuild)
                    newNode.Status += (int)Enums.ObjectStatusType.RebuildStatus;
                else
                    newNode.Status += (int)Enums.ObjectStatusType.AlterBodyStatus;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
