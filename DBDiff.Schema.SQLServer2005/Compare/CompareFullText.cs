using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareFullText : CompareBase<FullText>
    {
        protected override void DoUpdate<Root>(SchemaList<FullText, Root> originFields, FullText node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                FullText newNode = node; //.Clone(originFields.Parent);
                if (node.IsDefault != originFields[node.FullName].IsDefault)
                    newNode.Status += (int)Enums.ObjectStatusType.DisabledStatus;
                if (!node.Owner.Equals(originFields[node.FullName].Owner))
                    newNode.Status += (int)Enums.ObjectStatusType.ChangeOwner;
                if (node.IsAccentSensity != originFields[node.FullName].IsAccentSensity)
                    newNode.Status += (int)Enums.ObjectStatusType.AlterStatus;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
