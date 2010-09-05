using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    internal class ConstraintDependencies:List<ConstraintDependence>
    {
        public void Add(int tableId, int columnId, int ownerTableId, Constraint constraint)
        {
            ConstraintDependence depends = new ConstraintDependence();
            depends.ColumnId = columnId;
            depends.TableId = tableId;
            depends.OwnerTableId = ownerTableId;
            depends.Constraint = constraint;
            base.Add(depends);
        }

        public void Add(int tableId, int columnId, int ownerTableId, ISchemaBase constraint)
        {
            ConstraintDependence depends = new ConstraintDependence();
            depends.ColumnId = columnId;
            depends.TableId = tableId;
            depends.OwnerTableId = ownerTableId;
            depends.ObjectConstraint = constraint; //SE COMENTO SIMPLEMENTE QUE QUE SIGA, HAY QUE ARREGLARLO!!!
            base.Add(depends);
        }

        /// <summary>
        /// Devuelve todos las constraints dependientes de una tabla.
        /// </summary>
        public Constraints FindNotOwner(int tableId)
        {
            Constraints cons = new Constraints(null);
            foreach (ConstraintDependence depens in this)
            {
                if ((depens.TableId == tableId) && (depens.Constraint.Type == Constraint.ConstraintType.ForeignKey) /*&& (depens.OwnerTableId != tableId)*/)
                    cons.Add(depens.Constraint);
            }
            return cons;
        }

        /// <summary>
        /// Devuelve todos las constraints dependientes de una tabla.
        /// </summary>
        public Constraints Find(int tableId)
        {
            return Find(tableId, 0);
        }

        /// <summary>
        /// Devuelve todos las constraints dependientes de una tabla y una columna.
        /// </summary>
        public Constraints Find(int tableId, int columnId)
        {
            Constraints cons = new Constraints(null);
            //Primero busca los defaults
            foreach (ConstraintDependence depens in this)
            {
                if ((depens.ColumnId == columnId || columnId == 0) && (depens.TableId == tableId) && (depens.Constraint.Type == Constraint.ConstraintType.Default))
                    cons.Add(depens.Constraint);
            }
            //Luego los checks
            foreach (ConstraintDependence depens in this)
            {
                if ((depens.ColumnId == columnId || columnId == 0) && (depens.TableId == tableId) && (depens.Constraint.Type == Constraint.ConstraintType.Check))
                    cons.Add(depens.Constraint);
            }
            //Luego las FK
            foreach (ConstraintDependence depens in this)
            {
                if ((depens.ColumnId == columnId || columnId == 0) && (depens.TableId == tableId) && (depens.Constraint.Type == Constraint.ConstraintType.ForeignKey))
                    cons.Add(depens.Constraint);
            }
            //Luego las Unique
            foreach (ConstraintDependence depens in this)
            {
                if ((depens.ColumnId == columnId || columnId == 0) && (depens.TableId == tableId) && (depens.Constraint.Type == Constraint.ConstraintType.Unique))
                    cons.Add(depens.Constraint);
            }
            //Por ultimo los PK
            foreach (ConstraintDependence depens in this)
            {
                if ((depens.ColumnId == columnId || columnId == 0) && (depens.TableId == tableId) && (depens.Constraint.Type == Constraint.ConstraintType.PrimaryKey))
                    cons.Add(depens.Constraint);
            }
            return cons;
        }
    }
}
