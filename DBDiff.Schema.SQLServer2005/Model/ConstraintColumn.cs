using System;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class ConstraintColumn : SQLServerSchemaBase, IComparable<ConstraintColumn>
    {
        public ConstraintColumn(Constraint parentObject)
            : base(parentObject, Enums.ObjectType.ConstraintColumn)
        {
        }

        public ConstraintColumn Clone()
        {
            ConstraintColumn ccol = new ConstraintColumn((Constraint)this.Parent);
            ccol.ColumnRelationalName = this.ColumnRelationalName;
            ccol.ColumnRelationalId = this.ColumnRelationalId;
            ccol.Name = this.Name;
            ccol.IsIncluded = this.IsIncluded;
            ccol.Order = this.Order;
            ccol.KeyOrder = this.KeyOrder;
            ccol.Id = this.Id;
            ccol.DataTypeId = this.DataTypeId;
            ccol.ColumnRelationalDataTypeId = this.ColumnRelationalDataTypeId;
            return ccol;
        }

        public int DataTypeId { get; set; }

        public int ColumnRelationalDataTypeId { get; set; }

        public int ColumnRelationalId { get; set; }

        /// <summary>
        /// Gets or sets the column key order in the index.
        /// </summary>
        /// <value>The key order.</value>
        public int KeyOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column is included in the index leaf page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this column is included; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsIncluded { get; set; }

        /// <summary>
        /// Orden de la columna (Ascendente o Descendente). Se usa solo en Primary Keys.
        /// </summary>
        public Boolean Order { get; set; }

        public string ColumnRelationalName { get; set; }

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

        public static Boolean Compare(ConstraintColumn origin, ConstraintColumn destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if ((origin.ColumnRelationalName == null) && (destination.ColumnRelationalName != null)) return false;
            if (origin.ColumnRelationalName != null)
            {
                if (!origin.ColumnRelationalName.Equals(destination.ColumnRelationalName, StringComparison.CurrentCultureIgnoreCase)) return false;
            }
            if (origin.IsIncluded != destination.IsIncluded) return false;
            if (origin.Order != destination.Order) return false;
            if (origin.KeyOrder != destination.KeyOrder) return false;
            return true;
        }

        public int CompareTo(ConstraintColumn other)
        {
            return this.KeyOrder.CompareTo(other.KeyOrder);
        }
    }
}
