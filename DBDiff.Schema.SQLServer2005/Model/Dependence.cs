using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    internal class Dependence
    {
        private int objectId;
        private int subObjectId;
        private int ownerTableId;
        private ISchemaBase objectSchema;
        private int typeId;

        public ISchemaBase ObjectSchema
        {
            get { return objectSchema; }
            set { objectSchema = value; }
        }

        public int DataTypeId
        {
            get { return typeId; }
            set { typeId = value; }
        }

        public Enums.ObjectType Type
        {
            get { return objectSchema.ObjectType; }
        }

        public int SubObjectId
        {
            get { return subObjectId; }
            set { subObjectId = value; }
        }

        /// <summary>
        /// ID de la tabla a la que hace referencia la constraint.
        /// </summary>
        public int ObjectId
        {
            get { return objectId; }
            set { objectId = value; }
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
