using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class CompareFunctions : CompareBase<Function>
    {
        protected override void DoNew<Root>(SchemaList<Function, Root> originFields, Function node)
        {
            Function newNode = (Function)node.Clone(originFields.Parent);
            newNode.Status = ObjectStatus.Create;
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

                newNode.Status = ObjectStatus.Alter;
                if (newNode.IsSchemaBinding)
                    newNode.Status += (int)ObjectStatus.RebuildDependencies;
                if (newNode.HasToRebuild)
                    newNode.Status += (int)ObjectStatus.Rebuild;
                else
                    newNode.Status += (int)ObjectStatus.AlterBody;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
