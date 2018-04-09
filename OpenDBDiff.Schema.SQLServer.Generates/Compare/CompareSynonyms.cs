using OpenDBDiff.Schema.Model;
using OpenDBDiff.Schema.SQLServer.Generates.Model;

namespace OpenDBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareSynonyms : CompareBase<Synonym>
    {
        protected override void DoUpdate<Root>(SchemaList<Synonym, Root> originFields, Synonym node)
        {
            if (!Synonym.Compare(node, originFields[node.FullName]))
            {
                Synonym newNode = node; //.Clone(originFields.Parent);
                newNode.Status = ObjectStatus.Alter;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
