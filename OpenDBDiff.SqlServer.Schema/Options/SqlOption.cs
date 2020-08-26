using OpenDBDiff.Abstractions.Schema.Model;
using System;

namespace OpenDBDiff.SqlServer.Schema.Options
{
    public class SqlOption : IOption
    {
        public SqlOption()
        {
            Defaults = new SqlOptionDefault();
            Ignore = new SqlOptionIgnore(true);
            Script = new SqlOptionScript();
            Filters = new SqlOptionFilter();
            Comparison = new SqlOptionComparison();
        }

        public SqlOption(Boolean defaultFilter)
        {
            Defaults = new SqlOptionDefault();
            Ignore = new SqlOptionIgnore(defaultFilter);
            Script = new SqlOptionScript();
            Filters = new SqlOptionFilter();
            Comparison = new SqlOptionComparison();
        }

        public SqlOption(IOption option)
        {
            Defaults = new SqlOptionDefault(option.Defaults);
            Ignore = new SqlOptionIgnore(option.Ignore);
            Script = new SqlOptionScript(option.Script);
            Filters = new SqlOptionFilter(option.Filters);
            Comparison = new SqlOptionComparison(option.Comparison);
        }

        public SqlOptionComparison Comparison { get; set; }

        public SqlOptionFilter Filters { get; set; }

        /// <summary>
        /// Gets or sets the option filter.
        /// </summary>
        /// <value>The option filter.</value>
        public SqlOptionIgnore Ignore { get; set; }

        /// <summary>
        /// Gets or sets the option default.
        /// </summary>
        /// <value>The option default.</value>
        public SqlOptionDefault Defaults { get; set; }

        public SqlOptionScript Script { get; set; }

        IOptionFilter IOption.Filters { get { return Filters; } }
        IOptionsContainer<string> IOption.Defaults { get { return Defaults; } }
        IOptionsContainer<bool> IOption.Ignore { get { return Ignore; } }

        IOptionsContainer<bool> IOption.Script { get { return Script; } }

        IOptionComparison IOption.Comparison { get { return Comparison; } }

        public string Serialize()
        {
            return this.ToString();
        }
    }
}
