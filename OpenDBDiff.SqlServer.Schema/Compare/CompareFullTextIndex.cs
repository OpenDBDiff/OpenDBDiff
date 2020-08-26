using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class CompareFullTextIndex : CompareBase<FullTextIndex>
    {
        protected override void DoNew<Root>(SchemaList<FullTextIndex, Root> originFields, FullTextIndex node)
        {
            FullTextIndex newNode = (FullTextIndex)node.Clone(originFields.Parent);
            newNode.Status = ObjectStatus.Create;
            originFields.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<FullTextIndex, Root> originFields, FullTextIndex node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                FullTextIndex newNode = (FullTextIndex)node.Clone(originFields.Parent);
                if (node.IsDisabled != originFields[node.FullName].IsDisabled)
                    newNode.Status += (int)ObjectStatus.Disabled;
                else
                    newNode.Status += (int)ObjectStatus.Alter;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
