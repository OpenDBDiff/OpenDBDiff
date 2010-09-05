using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareFunctions : CompareBase<Function>
    {
        protected override void DoNew<Root>(SchemaList<Function, Root> CamposOrigen, Function node)
        {
            Function newNode = (Function)node.Clone(CamposOrigen.Parent);
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

        protected override void DoUpdate<Root>(SchemaList<Function, Root> CamposOrigen, Function node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                Function newNode = (Function)node.Clone(CamposOrigen.Parent);
                newNode.DependenciesIn.AddRange(CamposOrigen[node.FullName].DependenciesIn);
                newNode.DependenciesOut.AddRange(CamposOrigen[node.FullName].DependenciesOut);

                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                if (newNode.IsSchemaBinding)
                    newNode.Status += (int)Enums.ObjectStatusType.RebuildDependenciesStatus;
                if (newNode.HasToRebuild)
                    newNode.Status += (int)Enums.ObjectStatusType.RebuildStatus;
                else
                    newNode.Status += (int)Enums.ObjectStatusType.AlterBodyStatus;
                CamposOrigen[node.FullName] = newNode;
            }            
        }
    }
}
