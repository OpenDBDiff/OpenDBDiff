using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareViews : CompareBase<View>
    {
        protected override void DoUpdate<Root>(SchemaList<View, Root> CamposOrigen, View node)
        {
            View original = CamposOrigen[node.FullName];
            if (!node.Compare(original))
            {
                View newNode = (View)node.Clone(CamposOrigen.Parent);
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

                CamposOrigen[node.FullName] = newNode;
                original = newNode;
            }
            (new CompareIndexes()).GenerateDiferences<View>(original.Indexes, node.Indexes);
            (new CompareTriggers()).GenerateDiferences<View>(original.Triggers, node.Triggers);
        }

        protected override void DoNew<Root>(SchemaList<View, Root> CamposOrigen, View node)
        {
            View newNode = (View)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
            newNode.DependenciesIn.ForEach(dep =>
            {
                ISchemaBase item = ((Database)((ISchemaBase)CamposOrigen.Parent)).Find(dep);
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
