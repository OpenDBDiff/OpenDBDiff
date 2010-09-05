using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.Sybase.Options
{
    public class AseOption
    {
        private AseOptionDefault optionDefault;
        private AseOptionFilter optionFilter;

        public AseOption()
        {
            optionDefault = new AseOptionDefault();
            optionFilter = new AseOptionFilter();
        }

        public AseOption(Boolean defaultFilter)
        {
            optionDefault = new AseOptionDefault();
            optionFilter = new AseOptionFilter(defaultFilter);
        }

        /// <summary>
        /// Gets or sets the option filter.
        /// </summary>
        /// <value>The option filter.</value>
        public AseOptionFilter OptionFilter
        {
            get { return optionFilter; }
            set { optionFilter = value; }
        }

        /// <summary>
        /// Gets or sets the option default.
        /// </summary>
        /// <value>The option default.</value>
        public AseOptionDefault OptionDefault
        {
            get { return optionDefault; }
            set { optionDefault = value; }
        }
    }
}
