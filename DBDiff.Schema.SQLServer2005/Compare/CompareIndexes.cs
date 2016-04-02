using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareIndexes : CompareBase<Index>
    {
        protected override void DoNew<Root>(SchemaList<Index, Root> originFields, Index node)
        {
            Index newNode = (Index)node.Clone(originFields.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            originFields.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<Index, Root> originFields, Index node)
        {
            if (!Index.Compare(node, originFields[node.FullName]))
            {
                Index newNode = (Index)node.Clone(originFields.Parent);
                if (!Index.CompareExceptIsDisabled(node, originFields[node.FullName]))
                {
                    newNode.Status = Enums.ObjectStatusType.AlterStatus;
                }
                else
                    newNode.Status = Enums.ObjectStatusType.DisabledStatus;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
