using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    internal class Dependence
    {
        public enum DependencieTypeEnum
        {
            Constraint = 1,
            Index = 2,
            Default = 3,
            View = 4
        }

        private int tableId;
        private int columnId;
        private int ownerTableId;
        private Index index;
        private Constraint constraint;
        private View view;
        private ColumnConstraint defaultConstraint;
        private DependencieTypeEnum type;
        private int typeId;

        public View View
        {
            get { return view; }
            set { view = value; }
        }

        public int DataTypeId
        {
            get { return typeId; }
            set { typeId = value; }
        }

        public DependencieTypeEnum Type
        {
            get { return type; }
            set { type = value; }
        }

        public ColumnConstraint Default 
        {
            get { return defaultConstraint; }
            set { defaultConstraint = value; }
        }

        public Index Index
        {
            get { return index; }
            set { index = value; }
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
