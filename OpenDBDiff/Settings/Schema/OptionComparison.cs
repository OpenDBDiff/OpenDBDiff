using OpenDBDiff.SqlServer.Schema.Options;

namespace OpenDBDiff.Settings.Schema
{
    public class OptionComparison
    {
        public bool IgnoreWhiteSpacesInCode { get; set; }
        public bool ReloadComparisonOnUpdate { get; set; }
        public SqlOptionComparison.CaseSensityOptions CaseSensityInCode { get; set; }
        public SqlOptionComparison.CaseSensityOptions CaseSensityType { get; set; }
    }
}
