using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class CompareIndexes : CompareBase<Index>
    {
        protected override void DoNew<Root>(SchemaList<Index, Root> originFields, Index node)
        {
            Index newNode = (Index)node.Clone(originFields.Parent);
            newNode.Status = ObjectStatus.Create;
            originFields.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<Index, Root> originFields, Index node)
        {
            if (!Index.Compare(node, originFields[node.FullName]))
            {
                Index newNode = (Index)node.Clone(originFields.Parent);
                if (!Index.CompareExceptIsDisabled(node, originFields[node.FullName]))
                {
                    newNode.Status = ObjectStatus.Alter;
                }
                else
                    newNode.Status = ObjectStatus.Disabled;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
