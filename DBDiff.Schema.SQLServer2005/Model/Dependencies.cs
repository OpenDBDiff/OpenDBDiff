using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    internal class Dependencies: List<Dependence>
    {
        public void Add(int tableId, int columnId, int ownerTableId, int typeId, Constraint constraint)
        {
            Dependence depends = new Dependence();
            depends.ColumnId = columnId;
            depends.TableId = tableId;
            depends.OwnerTableId = ownerTableId;
            depends.Constraint = constraint;
            depends.DataTypeId = typeId;
            depends.Type = Dependence.DependencieTypeEnum.Constraint;
            base.Add(depends);
        }

        public void Add(int tableId, int columnId, int ownerTableId, int typeId, Index index)
        {
            Dependence depends = new Dependence();
            depends.ColumnId = columnId;
            depends.TableId = tableId;
            depends.OwnerTableId = ownerTableId;
            depends.Index = index;
            depends.DataTypeId = typeId;
            depends.Type = Dependence.DependencieTypeEnum.Index;
            base.Add(depends);
        }

        public void Add(int tableId, int columnId, int ownerTableId, int typeId, View view)
        {
            Dependence depends = new Dependence();
            depends.ColumnId = columnId;
            depends.TableId = tableId;
            depends.OwnerTableId = ownerTableId;
            depends.View = view;
            depends.DataTypeId = typeId;
            depends.Type = Dependence.DependencieTypeEnum.Default;
            base.Add(depends);
        }

        public void Add(int tableId, int columnId, int ownerTableId, int typeId, ColumnConstraint constraint)
        {
            Dependence depends = new Dependence();
            depends.ColumnId = columnId;
            depends.TableId = tableId;
            depends.OwnerTableId = ownerTableId;
            depends.Default = constraint;
            depends.DataTypeId = typeId;
            depends.Type = Dependence.DependencieTypeEnum.View;
            base.Add(depends);
        }

        /// <summary>
        /// Devuelve todos las constraints dependientes de una tabla.
        /// </summary>
        public Constraints FindNotOwner(int tableId)
        {
            Constraints cons = new Constraints(null);
            this.ForEach(depens =>
            {
                if (depens.Constraint != null)
                {
                    if ((depens.TableId == tableId) && (depens.Constraint.Type == Constraint.ConstraintType.ForeignKey))
                        cons.Add(depens.Constraint);
                }
            });
            return cons;
        }


        /// <summary>
        /// Devuelve todos las constraints dependientes de una tabla.
        /// </summary>
        public void Set(int tableId, Constraint constraint)
        {
            this.ForEach(item =>
            {
                if (item.Constraint != null)
                    if ((item.TableId == tableId) && (item.Constraint.Name.Equals(constraint.Name)))
                        item.Constraint = constraint;
            });           
        }

        /// <summary>
        /// Devuelve todos las constraints dependientes de una tabla.
        /// </summary>
        public List<ISchemaBase> Find(int tableId)
        {
            return Find(tableId, 0, 0);
        }

        /// <summary>
        /// Devuelve todos las constraints dependientes de una tabla y una columna.
        /// </summary>
        public List<ISchemaBase> Find(int tableId, int columnId, int dataTypeId)
        {
            List<ISchemaBase> cons = new List<ISchemaBase>();
            cons = (from depends in this
                    where depends.Type == Dependence.DependencieTypeEnum.Constraint &&
                    ((depends.DataTypeId == dataTypeId || dataTypeId == 0) && (depends.ColumnId == columnId || columnId == 0) && (depends.TableId == tableId))
                    select (ISchemaBase)depends.Constraint)
                    .Concat(from depends in this
                            where depends.Type == Dependence.DependencieTypeEnum.Index &&
                            ((depends.DataTypeId == dataTypeId || dataTypeId == 0) && (depends.ColumnId == columnId || columnId == 0) && (depends.TableId == tableId))
                            select (ISchemaBase)depends.Index)
                            .Concat(from depends in this
                                    where depends.Type == Dependence.DependencieTypeEnum.View &&
                                    (depends.TableId == tableId)
                                    select (ISchemaBase)depends.View).ToList();            
            return cons;
        }
    }
}
