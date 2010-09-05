using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.MySQL.Model
{
    public class IndexColumn : MySQLSchemaBase
    {
        private Boolean order;
        private int sequence;

        public IndexColumn(Table parentObject):base(StatusEnum.ObjectTypeEnum.IndexColumn)
        {
            Parent = parentObject;
        }

        public IndexColumn Clone(Table parent)
        {
            IndexColumn column = new IndexColumn(parent);
            column.Id = this.Id;
            column.Sequence = this.Sequence;
            column.Name = this.Name;
            column.Order = this.Order;
            column.Status = this.Status;
            return column;
        }

        public int Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }

        public Boolean Order
        {
            get { return order; }
            set { order = value; }
        }

        public static Boolean Compare(IndexColumn origen, IndexColumn destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.Order != destino.Order) return false;
            if (origen.Sequence != destino.Sequence) return false;
            return true;
        }
    }
}
