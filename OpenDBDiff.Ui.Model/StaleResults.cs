namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// Why results stopped being trustworthy. Section 9.6 lists the first four; a successful apply is
    /// the fifth, from the transition rules in section 4.
    /// </summary>
    public enum CompareStaleness
    {
        Fresh,

        /// <summary>A connection field changed after the compare.</summary>
        ConnectionChanged,

        /// <summary>Direction swapped.</summary>
        DirectionSwapped,

        /// <summary>Options changed that affect comparison.</summary>
        OptionsChanged,

        /// <summary>Project loaded with different connections.</summary>
        ProjectLoaded,

        /// <summary>The destination was written to, so the compare no longer describes it (section 4).</summary>
        Applied
    }

    /// <summary>
    /// The stale-results rules of section 9.6: what makes results stale, and the two strings the
    /// banner and the script preview's ribbon use.
    /// </summary>
    public static class StaleRules
    {
        /// <summary>The banner under the Summary Bar.</summary>
        public const string BannerText = "Results are out of date. Compare again to refresh.";

        /// <summary>The ribbon the script preview shows while the results are stale.</summary>
        public const string RibbonText = "From an earlier comparison";

        /// <summary>Any recorded reason other than Fresh invalidates the results.</summary>
        public static bool MakesResultsStale(CompareStaleness reason)
        {
            return reason != CompareStaleness.Fresh;
        }
    }
}
