namespace DBDiff.Schema.SQLServer.Generates.Model
{
    internal class Dependence
    {
        private int objectId;
        private int subObjectId;
        private int ownerTableId;
        private string fullName;
        private int typeId;
        private Enums.ObjectType type;

        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }

        public int DataTypeId
        {
            get { return typeId; }
            set { typeId = value; }
        }

        public Enums.ObjectType Type
        {
            get { return type; }
            set { type = value; }
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
