using OpenDBDiff.Schema.Model;
using OpenDBDiff.Schema.SQLServer.Generates.Model;

namespace OpenDBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareFullText : CompareBase<FullText>
    {
        protected override void DoUpdate<Root>(SchemaList<FullText, Root> originFields, FullText node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                FullText newNode = node; //.Clone(originFields.Parent);
                if (node.IsDefault != originFields[node.FullName].IsDefault)
                    newNode.Status += (int)ObjectStatus.Disabled;
                if (!node.Owner.Equals(originFields[node.FullName].Owner))
                    newNode.Status += (int)ObjectStatus.ChangeOwner;
                if (node.IsAccentSensity != originFields[node.FullName].IsAccentSensity)
                    newNode.Status += (int)ObjectStatus.Alter;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
