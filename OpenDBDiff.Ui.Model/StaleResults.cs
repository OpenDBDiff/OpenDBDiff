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
        Applied,

        /// <summary>
        /// Section 9.6's optional background check: the destination was changed by something outside this
        /// application while the results were on screen.
        /// </summary>
        ChangedExternally
    }

    /// <summary>
    /// The stale-results rules of section 9.6: what makes results stale, and the two strings the
    /// banner and the script preview's ribbon use.
    /// </summary>
    public static class StaleRules
    {
        /// <summary>The banner under the Summary Bar.</summary>
        public const string BannerText = "Results are out of date. Compare again to refresh.";

        /// <summary>
        /// The banner when the destination was changed by something else. It says so rather than reusing the
        /// general wording, because the useful thing here is not that the results are old but that they are
        /// describing a database that is being edited by someone else.
        /// </summary>
        public const string ExternalChangeBannerText =
            "Results are out of date. The destination changed outside this application. Compare again to refresh.";

        /// <summary>The ribbon the script preview shows while the results are stale.</summary>
        public const string RibbonText = "From an earlier comparison";

        /// <summary>The banner a reason shows.</summary>
        public static string BannerFor(CompareStaleness reason)
        {
            return reason == CompareStaleness.ChangedExternally ? ExternalChangeBannerText : BannerText;
        }

        /// <summary>
        /// Whether section 9.6's check is worth running at all. There is nothing to watch before a
        /// comparison has produced results, nothing to learn while the results are already known to be out
        /// of date, no point watching during a comparison or an apply - both of which write or read the
        /// destination themselves - and nothing to watch without a destination.
        /// </summary>
        public static bool WorthWatching(AppState state, CompareStaleness staleness, bool hasDestination)
        {
            return hasDestination && staleness == CompareStaleness.Fresh && state == AppState.Results;
        }

        /// <summary>Any recorded reason other than Fresh invalidates the results.</summary>
        public static bool MakesResultsStale(CompareStaleness reason)
        {
            return reason != CompareStaleness.Fresh;
        }
    }
}
