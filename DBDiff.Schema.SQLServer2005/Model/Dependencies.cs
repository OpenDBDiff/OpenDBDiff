using System;
using System.Collections.Generic;
using System.Linq;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    internal class Dependencies : List<Dependency>
    {
        public Database Database { get; private set; }

        public void Add(Database database, int tableId, int columnId, int ownerTableId, int typeId, ISchemaBase constraint)
        {
            Dependency dependency = new Dependency();
            dependency.SubObjectId = columnId;
            dependency.ObjectId = tableId;
            dependency.OwnerTableId = ownerTableId;

            dependency.FullName = constraint.FullName;
            dependency.Type = constraint.ObjectType;
            dependency.DataTypeId = typeId;
            this.Database = database;
            base.Add(dependency);
        }

        public void Add(Database database, int objectId, ISchemaBase objectSchema)
        {
            Dependency dependency = new Dependency();
            dependency.ObjectId = objectId;
            dependency.FullName = objectSchema.FullName;
            dependency.Type = objectSchema.ObjectType;
            this.Database = database;
            base.Add(dependency);
        }

        /// <summary>
        /// Devuelve todos las constraints dependientes de una tabla.
        /// </summary>
        public List<ISchemaBase> FindNotOwner(int tableId, Enums.ObjectType type)
        {
            try
            {
                List<ISchemaBase> cons = new List<ISchemaBase>();
                this.ForEach(dependency =>
                {
                    if (dependency.Type == type)
                    {
                        ISchemaBase item = (ISchemaBase)Database.Find(dependency.FullName);
                        if (dependency.Type == Enums.ObjectType.Constraint)
                        {
                            if ((dependency.ObjectId == tableId) && (((Constraint)item).Type == Constraint.ConstraintType.ForeignKey))
                                cons.Add(item);
                        }
                        else
                            if (dependency.ObjectId == tableId)
                            cons.Add(item);
                    }

                });
                return cons;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Devuelve todos las constraints dependientes de una tabla.
        /// </summary>
        /*public void Set(int tableId, Constraint constraint)
        {
            this.ForEach(item =>
            {
                if (item.Type == Enums.ObjectType.Constraint)
                    if ((item.ObjectId == tableId) && (item.ObjectSchema.Name.Equals(constraint.Name)))
                        item.ObjectSchema = constraint;
            });
        }*/

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
            List<string> cons = new List<string>();
            List<ISchemaBase> real = new List<ISchemaBase>();

            cons = (from depends in this
                    where (depends.Type == Enums.ObjectType.Constraint || depends.Type == Enums.ObjectType.Index) &&
                    ((depends.DataTypeId == dataTypeId || dataTypeId == 0) && (depends.SubObjectId == columnId || columnId == 0) && (depends.ObjectId == tableId))
                    select depends.FullName)
                        .Concat(from depends in this
                                where (depends.Type == Enums.ObjectType.View || depends.Type == Enums.ObjectType.Function) &&
                                (depends.ObjectId == tableId)
                                select depends.FullName).ToList();

            cons.ForEach(item =>
                {
                    ISchemaBase schema = Database.Find(item);
                    if (schema != null) real.Add(schema);
                }
            );
            return real;
        }
    }
}
