using System.Collections.Generic;
using System.Linq;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// The one-time inline tips (section 19, item R4).
    /// </summary>
    /// <remarks>
    /// Section 7.1 rule 4 stops Deletes being selected by default, which is safer and surprising: a user who
    /// expects every difference to be ticked will read an empty Actions count as "nothing to do". The tip
    /// exists so that surprise is explained once, in place, rather than becoming a support question - and it
    /// is worth being strict about when it appears, because a tip shown on a comparison with no deletes in
    /// it explains nothing and a tip repeated on every comparison is an annoyance rather than an
    /// explanation.
    /// </remarks>
    public static class TipRules
    {
        /// <summary>The tip's identity in the settings file, so it is remembered across runs.</summary>
        public const string DropPreselectionId = "drop-not-selected-by-default";

        /// <summary>Section 19's wording, verbatim.</summary>
        public const string DropPreselectionText = "Drop items start unselected for safety";

        /// <summary>The line under it, which says what to do about it.</summary>
        public const string DropPreselectionDetail =
            "Tick them here, or turn on Select Drop items by default in Options.";

        /// <summary>
        /// Whether the tip is worth showing: not seen before, something to explain, and something true to
        /// say. If the user has already turned deletes on by default, the tip would be describing behaviour
        /// they have changed, so it stays quiet.
        /// </summary>
        public static bool ShowsUnselectedDropTip(
            IEnumerable<string> seenTips,
            int unselectedDrops,
            bool dropsArePreselected)
        {
            if (dropsArePreselected || unselectedDrops <= 0)
            {
                return false;
            }

            return seenTips == null || !seenTips.Contains(DropPreselectionId);
        }
    }
}
