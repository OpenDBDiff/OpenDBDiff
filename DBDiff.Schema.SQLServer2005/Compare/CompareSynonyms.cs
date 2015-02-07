using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareSynonyms:CompareBase<Synonym>
    {
        protected override void DoUpdate<Root>(SchemaList<Synonym, Root> CamposOrigen, Synonym node)
        {
            if (!Synonym.Compare(node, CamposOrigen[node.FullName]))
            {
                Synonym newNode = node;//.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
