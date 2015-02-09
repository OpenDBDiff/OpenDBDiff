using System;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class IndexColumn : SQLServerSchemaBase, IComparable<IndexColumn>
    {
        public IndexColumn(ISchemaBase parentObject)
            : base(parentObject, Enums.ObjectType.IndexColumn)
        {
        }

        public IndexColumn Clone(ISchemaBase parent)
        {
            IndexColumn column = new IndexColumn(parent);
            column.Id = this.Id;
            column.IsIncluded = this.IsIncluded;
            column.Name = this.Name;
            column.Order = this.Order;
            column.Status = this.Status;
            column.KeyOrder = this.KeyOrder;
            column.DataTypeId = this.DataTypeId;
            return column;
        }

        public int DataTypeId { get; set; }

        public int KeyOrder { get; set; }

        public Boolean IsIncluded { get; set; }

        public Boolean Order { get; set; }

        public static Boolean Compare(IndexColumn origen, IndexColumn destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.IsIncluded != destino.IsIncluded) return false;
            if (origen.Order != destino.Order) return false;
            if (origen.KeyOrder != destino.KeyOrder) return false;
            return true;
        }
        
        public override string ToSqlDrop()
        {
            return "";
        }

        public override string ToSqlAdd()
        {
            return "";
        }

        public override string ToSql()
        {
            return "";
        }

        public int CompareTo(IndexColumn other)
        {
            /*if (other.Name.Equals(this.Name))
            {*/
                if (other.IsIncluded == this.IsIncluded)
                    return this.KeyOrder.CompareTo(other.KeyOrder);
                else
                    return other.IsIncluded.CompareTo(this.IsIncluded);
            /*}
            else
                return this.Name.CompareTo(other.Name);*/
        }
    }
}
