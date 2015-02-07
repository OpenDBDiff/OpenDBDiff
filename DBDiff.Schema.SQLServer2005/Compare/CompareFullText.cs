using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareFullText : CompareBase<FullText>
    {
        protected override void DoUpdate<Root>(SchemaList<FullText, Root> CamposOrigen, FullText node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                FullText newNode = node;//.Clone(CamposOrigen.Parent);                
                if (node.IsDefault != CamposOrigen[node.FullName].IsDefault)                
                    newNode.Status += (int)Enums.ObjectStatusType.DisabledStatus;
                if (!node.Owner.Equals(CamposOrigen[node.FullName].Owner))
                    newNode.Status += (int)Enums.ObjectStatusType.ChangeOwner;
                if (node.IsAccentSensity != CamposOrigen[node.FullName].IsAccentSensity)
                    newNode.Status += (int)Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
