using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.MySQL.Model
{
    public class ConstraintColumn : MySQLSchemaBase  
    {
        private string referencedColumnName;
        private string referencedTableName;
        private string referencedSchemaName;
        private int ordinalPosition;
        private int positionUniqueConstraint;

        public ConstraintColumn(Constraint parent):base(StatusEnum.ObjectTypeEnum.ConstraintColumn)
        {
            this.Parent = parent;            
        }

        public ConstraintColumn Clone(Constraint parentObject)
        {
            ConstraintColumn item = new ConstraintColumn(parentObject);
            item.Id = this.Id;
            item.Name = this.Name;
            item.OrdinalPosition = this.OrdinalPosition;
            item.Owner = this.Owner;
            item.PositionUniqueConstraint = this.PositionUniqueConstraint;
            item.ReferencedColumnName = this.ReferencedColumnName;
            item.ReferencedSchemaName = this.ReferencedSchemaName;
            item.ReferencedTableName = this.ReferencedTableName;
            return item;
        }

        public int PositionUniqueConstraint
        {
            get { return positionUniqueConstraint; }
            set { positionUniqueConstraint = value; }
        }

        public string ReferencedSchemaName
        {
            get { return referencedSchemaName; }
            set { referencedSchemaName = value; }
        }

        public string ReferencedTableName
        {
            get { return referencedTableName; }
            set { referencedTableName = value; }
        }

        public string ReferencedColumnName
        {
            get { return referencedColumnName; }
            set { referencedColumnName = value; }
        }

        public int OrdinalPosition
        {
            get { return ordinalPosition; }
            set { ordinalPosition = value; }
        }

        public static Boolean Compare(ConstraintColumn origen, ConstraintColumn destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if ((origen.ReferencedColumnName == null) && (destino.ReferencedColumnName != null)) return false;
            if ((origen.ReferencedTableName == null) && (destino.ReferencedTableName != null)) return false;
            if ((origen.ReferencedSchemaName == null) && (destino.ReferencedSchemaName != null)) return false;
            if ((origen.ReferencedColumnName != null) && (origen.ReferencedTableName != null) && (origen.ReferencedSchemaName != null))
            {
                if (!origen.ReferencedColumnName.Equals(destino.ReferencedColumnName)) return false;
                if (!origen.ReferencedTableName.Equals(destino.ReferencedTableName)) return false;
                if (!origen.ReferencedSchemaName.Equals(destino.ReferencedSchemaName)) return false;
            }
            if (origen.OrdinalPosition != destino.OrdinalPosition) return false;
            if (origen.PositionUniqueConstraint != destino.PositionUniqueConstraint) return false;
            return true;
        }
    }
}
