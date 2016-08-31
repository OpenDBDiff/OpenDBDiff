using DBDiff.Schema.Model;
using System;

namespace DBDiff.Schema.SQLServer.Generates.Options
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

        public SqlOptionComparison Comparison { get; set; }

        public SqlOptionFilter Filters { get; set; }
        IOptionFilter IOption.Filters { get { return Filters; } }

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
    }
}
