using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections.Generic;

namespace OpenDBDiff.SqlServer.Schema.Options
{
    public class SqlOptionComparison : IOptionComparison
    {

        public enum CaseSensityOptions
        {
            Automatic = 0,
            CaseInsensity = 1,
            CaseSensity = 2
        }

        public SqlOptionComparison()
        {
            CaseSensityInCode = CaseSensityOptions.CaseInsensity;
            IgnoreWhiteSpacesInCode = false;
        }

        public SqlOptionComparison(IOptionComparison comparison)
        {
            this.ReloadComparisonOnUpdate = comparison.ReloadComparisonOnUpdate;
            var options = comparison.GetOptions();
            IgnoreWhiteSpacesInCode = bool.Parse(options["IgnoreWhiteSpacesInCode"]);
            CaseSensityInCode = (CaseSensityOptions)Enum.Parse(typeof(CaseSensityOptions), options["CaseSensityInCode"], true);
            CaseSensityType = (CaseSensityOptions)Enum.Parse(typeof(CaseSensityOptions), options["CaseSensityType"], true);
        }

        public bool IgnoreWhiteSpacesInCode { get; set; }
        public bool ReloadComparisonOnUpdate { get; set; }


        public CaseSensityOptions CaseSensityInCode { get; set; }

        public CaseSensityOptions CaseSensityType { get; set; }

        public IDictionary<string, string> GetOptions()
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            options.Add("IgnoreWhiteSpacesInCode", IgnoreWhiteSpacesInCode.ToString());
            options.Add("ReloadComparisonOnUpdate", ReloadComparisonOnUpdate.ToString());
            options.Add("CaseSensityInCode", CaseSensityInCode.ToString());
            options.Add("CaseSensityType", CaseSensityType.ToString());
            return options;
        }
    }
}
