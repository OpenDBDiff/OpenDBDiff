using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
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

                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                newNode.Indexes = original.Indexes;
                newNode.Triggers = original.Triggers;

                if (newNode.IsSchemaBinding)
                    newNode.Status += (int)Enums.ObjectStatusType.RebuildDependenciesStatus;
                if (newNode.HasToRebuild)
                    newNode.Status += (int)Enums.ObjectStatusType.RebuildStatus;
                else
                    newNode.Status += (int)Enums.ObjectStatusType.AlterBodyStatus;

                originFields[node.FullName] = newNode;
                original = newNode;
            }
            (new CompareIndexes()).GenerateDifferences<View>(original.Indexes, node.Indexes);
            (new CompareTriggers()).GenerateDifferences<View>(original.Triggers, node.Triggers);
        }

        protected override void DoNew<Root>(SchemaList<View, Root> originFields, View node)
        {
            View newNode = (View)node.Clone(originFields.Parent);
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
    }
}
