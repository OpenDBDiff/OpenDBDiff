using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.SQLServer.Options
{
    public class SqlOption
    {
        private SqlOptionDefault optionDefault;
        private SqlOptionFilter optionFilter;

        public SqlOption()
        {
            optionDefault = new SqlOptionDefault();
            optionFilter = new SqlOptionFilter();
        }

        public SqlOption(Boolean defaultFilter)
        {
            optionDefault = new SqlOptionDefault();
            optionFilter = new SqlOptionFilter(defaultFilter);
        }

        /// <summary>
        /// Gets or sets the option filter.
        /// </summary>
        /// <value>The option filter.</value>
        public SqlOptionFilter OptionFilter
        {
            get { return optionFilter; }
            set { optionFilter = value; }
        }

        /// <summary>
        /// Gets or sets the option default.
        /// </summary>
        /// <value>The option default.</value>
        public SqlOptionDefault OptionDefault
        {
            get { return optionDefault; }
            set { optionDefault = value; }
        }
    }
}
