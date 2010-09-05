using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.SQLServer.Options
{
    public class SqlOption
    {
        private SqlOptionDefault defaults;
        private SqlOptionIgnore ignore;
        private SqlOptionScript script;
        private SqlOptionFilter filters;

        public SqlOption()
        {
            defaults = new SqlOptionDefault();
            ignore = new SqlOptionIgnore(true);
            script = new SqlOptionScript();
            filters = new SqlOptionFilter();
        }

        public SqlOption(Boolean defaultFilter)
        {
            defaults = new SqlOptionDefault();
            ignore = new SqlOptionIgnore(defaultFilter);
            script = new SqlOptionScript();
        }

        public SqlOptionFilter Filters
        {
            get { return filters; }
            set { filters = value; }
        }

        /// <summary>
        /// Gets or sets the option filter.
        /// </summary>
        /// <value>The option filter.</value>
        public SqlOptionIgnore Ignore
        {
            get { return ignore; }
            set { ignore = value; }
        }

        /// <summary>
        /// Gets or sets the option default.
        /// </summary>
        /// <value>The option default.</value>
        public SqlOptionDefault Defaults
        {
            get { return defaults; }
            set { defaults = value; }
        }

        public SqlOptionScript Script
        {
            get { return script; }
            set { script = value; }
        }
    }
}
