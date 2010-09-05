using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    internal class ConstraintDependence
    {
        private int tableId;
        private int columnId;
        private int ownerTableId;
        private Constraint constraint;

        public Constraint Constraint
        {
            get { return constraint; }
            set { constraint = value; }
        }

        public int ColumnId
        {
            get { return columnId; }
            set { columnId = value; }
        }

        /// <summary>
        /// ID de la tabla a la que hace referencia la constraint.
        /// </summary>
        public int TableId
        {
            get { return tableId; }
            set { tableId = value; }
        }

        /// <summary>
        /// ID de la tabla a la que pertenece la constraint.
        /// </summary>
        public int OwnerTableId
        {
            get { return ownerTableId; }
            set { ownerTableId = value; }
        }
    }
}
