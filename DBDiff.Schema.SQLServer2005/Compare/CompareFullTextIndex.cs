using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareFullTextIndex : CompareBase<FullTextIndex>
    {
        protected override void DoNew<Root>(SchemaList<FullTextIndex, Root> CamposOrigen, FullTextIndex node)
        {
            FullTextIndex newNode = (FullTextIndex)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<FullTextIndex, Root> CamposOrigen, FullTextIndex node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                FullTextIndex newNode = (FullTextIndex)node.Clone(CamposOrigen.Parent);
                if (node.IsDisabled != CamposOrigen[node.FullName].IsDisabled)
                    newNode.Status += (int)Enums.ObjectStatusType.DisabledStatus;
                else
                    newNode.Status += (int)Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
