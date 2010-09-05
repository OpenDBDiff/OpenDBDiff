using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    internal class Dependencies: List<Dependence>
    {
        public void Add(int tableId, int columnId, int ownerTableId, int typeId, ISchemaBase constraint)
        {
            Dependence depends = new Dependence();
            depends.SubObjectId = columnId;
            depends.ObjectId = tableId;
            depends.OwnerTableId = ownerTableId;

            depends.ObjectSchema = constraint;
            depends.DataTypeId = typeId;
            base.Add(depends);
        }

        public void Add(int objectId, ISchemaBase objectSchema)
        {
            Dependence depends = new Dependence();
            depends.ObjectId = objectId;
            depends.ObjectSchema = objectSchema;
            base.Add(depends);
        }

        /// <summary>
        /// Devuelve todos las constraints dependientes de una tabla.
        /// </summary>
        public List<ISchemaBase> FindNotOwner(int tableId, Enums.ObjectType type)
        {
            List<ISchemaBase> cons = new List<ISchemaBase>();
            this.ForEach(depens =>
            {
                if (depens.Type == type)
                {
                    if (depens.Type == Enums.ObjectType.Constraint)
                    {
                        if ((depens.ObjectId == tableId) && (((Constraint)depens.ObjectSchema).Type == Constraint.ConstraintType.ForeignKey))
                            cons.Add(depens.ObjectSchema);
                    }
                    else
                        if (depens.ObjectId == tableId)
                            cons.Add(depens.ObjectSchema);
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
                if (item.Type == Enums.ObjectType.Constraint)
                    if ((item.ObjectId == tableId) && (item.ObjectSchema.Name.Equals(constraint.Name)))
                        item.ObjectSchema = constraint;
            });           
        }

        /// <summary>
        /// Devuelve todos las constraints dependientes de una tabla.
        /// </summary>
        public List<ISchemaBase> Find(int tableId)
        {
            return Find(tableId, 0, 0);
        }

        public int DependenciesCount(int objectId, Enums.ObjectType type)
        {
            Dictionary<int, bool> depencyTracker = new Dictionary<int, bool>();
            return DependenciesCount(objectId, type, depencyTracker);
        }

        private int DependenciesCount(int tableId, Enums.ObjectType type, Dictionary<int, bool> depencyTracker)
        {
            int count = 0;
            bool putItem = false;
            int relationalTableId;
            List<ISchemaBase> constraints = this.FindNotOwner(tableId, type);
            for (int index = 0; index < constraints.Count; index++)
            {
                ISchemaBase cons = constraints[index];
                if (cons.ObjectType == type)
                {
                    if (type == Enums.ObjectType.Constraint)
                    {
                        relationalTableId = ((Constraint)cons).RelationalTableId;
                        putItem = (relationalTableId == tableId);
                    }
                }
                if (putItem)
                {
                    if (!depencyTracker.ContainsKey(tableId))
                    {
                        depencyTracker.Add(tableId, true);
                        count += 1 + DependenciesCount(cons.Parent.Id, type, depencyTracker);
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Devuelve todos las constraints dependientes de una tabla y una columna.
        /// </summary>
        public List<ISchemaBase> Find(int tableId, int columnId, int dataTypeId)
        {
            List<ISchemaBase> cons = new List<ISchemaBase>();
            cons = (from depends in this
                    where (depends.Type == Enums.ObjectType.Constraint || depends.Type == Enums.ObjectType.Index) &&
                    ((depends.DataTypeId == dataTypeId || dataTypeId == 0) && (depends.SubObjectId == columnId || columnId == 0) && (depends.ObjectId == tableId))
                    select depends.ObjectSchema)
                        .Concat(from depends in this
                                where (depends.Type == Enums.ObjectType.View || depends.Type == Enums.ObjectType.Function) &&
                                (depends.ObjectId == tableId)
                                select depends.ObjectSchema).ToList();
            return cons;
        }
    }
}
