using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareTablesOptions : CompareBase<TableOption>
    {
        protected override void DoNew<Root>(SchemaList<TableOption, Root> originFields, TableOption node)
        {
            TableOption newNode = (TableOption)node.Clone(originFields.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            originFields.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<TableOption, Root> originFields, TableOption node)
        {
            if (!TableOption.Compare(node, originFields[node.FullName]))
            {
                TableOption newNode = (TableOption)node.Clone(originFields.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
