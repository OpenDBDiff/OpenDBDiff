using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBDiff.Schema.SQLServer.Generates.Options
{
    public class SqlOptionComparison
    {
        public enum CaseSensityOptions
        {
            Automatic = 0,
            CaseInsensity = 1,
            CaseSensity = 2
        }

        private CaseSensityOptions caseSensityType;
        private CaseSensityOptions caseSensityInCode = CaseSensityOptions.CaseInsensity;
        private bool ignoreWhiteSpacesInCode = false;

        public bool IgnoreWhiteSpacesInCode
        {
            get { return ignoreWhiteSpacesInCode; }
            set { ignoreWhiteSpacesInCode = value; }
        }


        public CaseSensityOptions CaseSensityInCode
        {
            get { return caseSensityInCode; }
            set { caseSensityInCode = value; }
        }

        public CaseSensityOptions CaseSensityType
        {
            get { return caseSensityType; }
            set { caseSensityType = value; }
        }

    }
}
