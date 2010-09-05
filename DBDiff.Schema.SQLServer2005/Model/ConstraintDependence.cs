using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    internal class ConstraintDependence
    {
        private int tableId;
        private int columnId;
        private int ownerTableId;
        private ISchemaBase objectConstraint;
        private Constraint constraint;

        public ISchemaBase ObjectConstraint
        {
            get { return objectConstraint; }
            set { objectConstraint = value; }
        }

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
