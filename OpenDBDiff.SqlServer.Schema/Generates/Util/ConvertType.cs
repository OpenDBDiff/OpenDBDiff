using OpenDBDiff.Abstractions.Schema;

namespace OpenDBDiff.SqlServer.Schema.Generates.Util
{
    internal static class ConvertType
    {
        public static ObjectType GetObjectType(string type)
        {
            if (type.Trim().Equals("V")) return ObjectType.View;
            if (type.Trim().Equals("U")) return ObjectType.Table;
            if (type.Trim().Equals("FN")) return ObjectType.Function;
            if (type.Trim().Equals("TF")) return ObjectType.Function;
            if (type.Trim().Equals("IF")) return ObjectType.Function;
            if (type.Trim().Equals("P")) return ObjectType.StoredProcedure;
            if (type.Trim().Equals("TR")) return ObjectType.Trigger;
            return ObjectType.None;
        }
    }
}
