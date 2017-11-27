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

        public SqlOptionComparison()
        {
            CaseSensityInCode = CaseSensityOptions.CaseInsensity;
            IgnoreWhiteSpacesInCode = false;
        }

        public bool IgnoreWhiteSpacesInCode { get; set; }
        public bool ReloadComparisonOnUpdate { get; set; }


        public CaseSensityOptions CaseSensityInCode { get; set; }

        public CaseSensityOptions CaseSensityType { get; set; }
    }
}
