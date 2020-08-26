using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenDBDiff.SqlServer.Schema.Options
{
    public class SqlOptionFilterItem
    {
        public SqlOptionFilterItem() { }

        public SqlOptionFilterItem(ObjectType objectType, string filterPattern)
        {
            this.ObjectType = objectType;
            this.FilterPattern = filterPattern;
        }

        public ObjectType ObjectType { get; set; }

        public string FilterPattern { get; set; }

        public bool IsMatch(ISchemaBase item)
        {
            if (item.ObjectType.Equals(this.ObjectType) && ValueSatisfiesCriteria(item.Name, this.FilterPattern))
                return true;
            else if (this.IsSchemaMatch(item))
                return true;
            else
                return false;
        }

        private bool IsSchemaMatch(ISchemaBase item)
        {
            if (item.Owner == null) return false;
            return this.ObjectType.Equals(ObjectType.Schema) && ValueSatisfiesCriteria(item.Owner, this.FilterPattern);
        }

        private static Lazy<Dictionary<string, Tuple<string, string>>> patternReplacements =
            new Lazy<Dictionary<string, Tuple<string, string>>>(() =>
            {
                return new Dictionary<string, Tuple<string, string>>
                {
                    // key: the literal string to match
                    // value: a tuple: first item: the search pattern, second item: the replacement
                    { @"~~", new Tuple<string, string>(@"~~", "~") },
                    { @"~*", new Tuple<string, string>(@"~\*", @"\*") },
                    { @"~?", new Tuple<string, string>(@"~\?", @"\?") },
                    { @"?", new Tuple<string, string>(@"\?", ".?") },
                    { @"*", new Tuple<string, string>(@"\*", ".*") }
                };
            });

        private static bool ValueSatisfiesCriteria(string value, string pattern)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(pattern)) return false;

            // if criteria is a regular expression, use regex
            if (pattern.IndexOfAny(new[] { '*', '?' }) > -1)
            {
                var regex = Regex.Replace(
                    pattern,
                    "(" + string.Join(
                            "|",
                            patternReplacements.Value.Values.Select(t => t.Item1))
                    + ")",
                    m => patternReplacements.Value[m.Value].Item2);
                regex = $"^{regex}$";

                return Regex.IsMatch(value, regex, RegexOptions.IgnoreCase);
            }

            // straight string comparison
            return string.Equals(value, pattern, StringComparison.OrdinalIgnoreCase);
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

        #endregion Overrides
    }
}
