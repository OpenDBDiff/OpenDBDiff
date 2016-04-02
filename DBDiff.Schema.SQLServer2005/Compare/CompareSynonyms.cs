using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareSynonyms : CompareBase<Synonym>
    {
        protected override void DoUpdate<Root>(SchemaList<Synonym, Root> originFields, Synonym node)
        {
            if (!Synonym.Compare(node, originFields[node.FullName]))
            {
                Synonym newNode = node; //.Clone(originFields.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
