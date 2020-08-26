using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class CompareViews : CompareBase<View>
    {
        protected override void DoUpdate<Root>(SchemaList<View, Root> originFields, View node)
        {
            View original = originFields[node.FullName];
            if (!node.Compare(original))
            {
                View newNode = (View)node.Clone(originFields.Parent);
                newNode.DependenciesOut.AddRange(original.DependenciesOut);
                newNode.DependenciesIn.AddRange(original.DependenciesIn);

                newNode.Status = ObjectStatus.Alter;
                newNode.Indexes = original.Indexes;
                newNode.Triggers = original.Triggers;

                if (newNode.IsSchemaBinding)
                    newNode.Status += (int)ObjectStatus.RebuildDependencies;
                if (newNode.HasToRebuild)
                    newNode.Status += (int)ObjectStatus.Rebuild;
                else
                    newNode.Status += (int)ObjectStatus.AlterBody;

                originFields[node.FullName] = newNode;
                original = newNode;
            }
            (new CompareIndexes()).GenerateDifferences<View>(original.Indexes, node.Indexes);
            (new CompareTriggers()).GenerateDifferences<View>(original.Triggers, node.Triggers);
        }

        protected override void DoNew<Root>(SchemaList<View, Root> originFields, View node)
        {
            View newNode = (View)node.Clone(originFields.Parent);
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
    }
}
