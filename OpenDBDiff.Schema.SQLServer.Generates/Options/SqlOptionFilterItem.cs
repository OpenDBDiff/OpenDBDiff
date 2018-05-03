using OpenDBDiff.Schema.Model;
using System;

namespace OpenDBDiff.Schema.SQLServer.Generates.Options
{
    public class SqlOptionFilterItem
    {
        public SqlOptionFilterItem(ObjectType objectType, string filterPattern)
        {
            this.ObjectType = objectType;
            this.FilterPattern = filterPattern;
        }

        public ObjectType ObjectType { get; set; }

        public string FilterPattern { get; set; }

        public bool IsMatch(ISchemaBase item)
        {
            return item.ObjectType.Equals(this.ObjectType) && item.Name.Equals(this.FilterPattern, StringComparison.OrdinalIgnoreCase) || this.IsSchemaMatch(item);
        }

        public bool IsSchemaMatch(ISchemaBase item)
        {
            if (item.Owner == null) return false;
            return this.ObjectType.Equals(ObjectType.Schema) && item.Owner.Equals(this.FilterPattern, StringComparison.OrdinalIgnoreCase);
        }

        #region Overrides
        public static bool operator ==(SqlOptionFilterItem x, SqlOptionFilterItem y)
        {
            return Object.Equals(x, y);
        }

        public static bool operator !=(SqlOptionFilterItem x, SqlOptionFilterItem y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            SqlOptionFilterItem fi = obj as SqlOptionFilterItem;
            if (fi == null)
            {
                return false;
            }
            return this.ObjectType.Equals(fi.ObjectType) && this.FilterPattern.Equals(fi.FilterPattern, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            long hash = 13;
            hash = hash + this.ObjectType.GetHashCode() + this.FilterPattern.ToLowerInvariant().GetHashCode();
            return Convert.ToInt32(hash & 0x7fffffff);
        }
        #endregion
    }
}
