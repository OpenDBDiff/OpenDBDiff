using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class CompareTablesOptions : CompareBase<TableOption>
    {
        protected override void DoNew<Root>(SchemaList<TableOption, Root> originFields, TableOption node)
        {
            TableOption newNode = (TableOption)node.Clone(originFields.Parent);
            newNode.Status = ObjectStatus.Create;
            originFields.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<TableOption, Root> originFields, TableOption node)
        {
            if (!TableOption.Compare(node, originFields[node.FullName]))
            {
                TableOption newNode = (TableOption)node.Clone(originFields.Parent);
                newNode.Status = ObjectStatus.Alter;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
