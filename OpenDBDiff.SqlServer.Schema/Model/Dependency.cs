using OpenDBDiff.Abstractions.Schema;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    internal class Dependency
    {
        public string FullName { get; set; }

        public int DataTypeId { get; set; }

        public ObjectType Type { get; set; }

        public int SubObjectId { get; set; }

        /// <summary>
        /// ID de la tabla a la que hace referencia la constraint.
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// ID de la tabla a la que pertenece la constraint.
        /// </summary>
        public int OwnerTableId { get; set; }
    }
}
