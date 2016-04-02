using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareFullTextIndex : CompareBase<FullTextIndex>
    {
        protected override void DoNew<Root>(SchemaList<FullTextIndex, Root> originFields, FullTextIndex node)
        {
            FullTextIndex newNode = (FullTextIndex)node.Clone(originFields.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            originFields.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<FullTextIndex, Root> originFields, FullTextIndex node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                FullTextIndex newNode = (FullTextIndex)node.Clone(originFields.Parent);
                if (node.IsDisabled != originFields[node.FullName].IsDisabled)
                    newNode.Status += (int)Enums.ObjectStatusType.DisabledStatus;
                else
                    newNode.Status += (int)Enums.ObjectStatusType.AlterStatus;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
