using System;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class TablePartition : SQLServerSchemaBase
    {
        public TablePartition(Table parent)
            : base(parent, Enums.ObjectType.Partition)
        {
        }

        public string CompressType { get; set; }


        public override string ToSql()
        {
            throw new NotImplementedException();
        }

        public override string ToSqlDrop()
        {
            throw new NotImplementedException();
        }

        public override string ToSqlAdd()
        {
            throw new NotImplementedException();
        }
    }
}
