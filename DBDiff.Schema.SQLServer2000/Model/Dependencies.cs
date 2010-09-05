using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    internal class Dependencies:List<Dependence>
    {
        public void Add(int tableId, int columnId, int ownerTableId, Constraint constraint)
        {
            Dependence depends = new Dependence();
            depends.ColumnId = columnId;
            depends.TableId = tableId;
            depends.OwnerTableId = ownerTableId;
            depends.Constraint = constraint;
            base.Add(depends);
        }

        /// <summary>
        /// Devuelve todos las constraints dependientes de una tabla.
        /// </summary>
        public Constraints FindNotOwner(int tableId)
        {
            Constraints cons = new Constraints();
            foreach (Dependence depens in this)
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
            Constraints cons = new Constraints();
            //Primero busca los defaults
            foreach (Dependence depens in this)
            {
                if ((depens.ColumnId == columnId || columnId == 0) && (depens.TableId == tableId) && (depens.Constraint.Type == Constraint.ConstraintType.Default))
                    cons.Add(depens.Constraint);
            }
            //Luego los checks
            foreach (Dependence depens in this)
            {
                if ((depens.ColumnId == columnId || columnId == 0) && (depens.TableId == tableId) && (depens.Constraint.Type == Constraint.ConstraintType.Check))
                    cons.Add(depens.Constraint);
            }
            //Luego las FK
            foreach (Dependence depens in this)
            {
                if ((depens.ColumnId == columnId || columnId == 0) && (depens.TableId == tableId) && (depens.Constraint.Type == Constraint.ConstraintType.ForeignKey))
                    cons.Add(depens.Constraint);
            }
            //Luego las Unique
            foreach (Dependence depens in this)
            {
                if ((depens.ColumnId == columnId || columnId == 0) && (depens.TableId == tableId) && (depens.Constraint.Type == Constraint.ConstraintType.Unique))
                    cons.Add(depens.Constraint);
            }
            //Por ultimo los PK
            foreach (Dependence depens in this)
            {
                if ((depens.ColumnId == columnId || columnId == 0) && (depens.TableId == tableId) && (depens.Constraint.Type == Constraint.ConstraintType.PrimaryKey))
                    cons.Add(depens.Constraint);
            }
            return cons;
        }
    }
}
