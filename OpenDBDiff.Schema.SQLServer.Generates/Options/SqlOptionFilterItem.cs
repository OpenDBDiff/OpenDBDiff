﻿using OpenDBDiff.Schema.Model;
using System;

namespace OpenDBDiff.Schema.SQLServer.Generates.Options
{
    public class SqlOptionFilterItem
    {
        public SqlOptionFilterItem(ObjectType type, string filter)
        {
            this.Filter = filter;
            this.Type = type;
        }

        public ObjectType Type { get; set; }

        public string Filter { get; set; }

        public bool IsMatch(ISchemaBase item)
        {
            return item.ObjectType == this.Type && item.Name.ToLower() == this.Filter.ToLower() || this.IsSchemaMatch(item);
        }

        public bool IsSchemaMatch(ISchemaBase item)
        {
            if (item.Owner == null) return false;
            return this.Type == ObjectType.Schema && item.Owner.ToLower() == this.Filter.ToLower();
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
            return this.Type.Equals(fi.Type) && this.Filter.Equals(fi.Filter);
        }

        public override int GetHashCode()
        {
            long hash = 13;
            hash = hash + this.Type.GetHashCode() + this.Filter.GetHashCode();
            return Convert.ToInt32(hash & 0x7fffffff);
        }
        #endregion
    }
}
