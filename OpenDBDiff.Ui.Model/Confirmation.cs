using System;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// The gating rules of the confirmation dialog (section 6.8), kept apart from the dialog so they can
    /// be exercised without one.
    /// </summary>
    public static class ConfirmationRules
    {
        /// <summary>Section 6.8's copy, which the typed-confirmation field is labelled with.</summary>
        public const string TypeToConfirmText = "Type the database name to confirm";

        /// <summary>
        /// Section 6.8: the typed confirmation is asked for only when the destination is tagged
        /// production or when at least one included item is destructive.
        /// </summary>
        public static bool RequiresTypedConfirmation(EnvironmentTag environment, int destructiveCount)
        {
            return (environment != null && environment.IsProduction) || destructiveCount > 0;
        }

        /// <summary>
        /// Section 6.8: the primary button stays disabled until the text equals the database name
        /// exactly, case-sensitive, after trimming.
        /// </summary>
        public static bool AcceptsTypedConfirmation(string database, string typed)
        {
            if (string.IsNullOrEmpty(database))
            {
                return false;
            }

            return string.Equals(database, (typed ?? string.Empty).Trim(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Whether the Apply button in the dialog is live. Without the typed gate, an included change is
        /// enough; section 6.8 leaves nothing else to check because the enable rules were already
        /// applied on the Action Bar before the dialog opened.
        /// </summary>
        public static bool CanApply(bool requiresTypedConfirmation, bool typedConfirmationAccepted, int selectedCount)
        {
            if (selectedCount <= 0)
            {
                return false;
            }

            return !requiresTypedConfirmation || typedConfirmationAccepted;
        }

        /// <summary>Section 6.8: the destructive callout's copy (Appendix A, confirm.destructive).</summary>
        public static string DestructiveCallout(int destructiveCount)
        {
            return string.Format(
                "{0} destructive statements. Dropped objects and columns cannot be recovered by this tool.",
                destructiveCount);
        }

        /// <summary>Section 6.8's title template, with the count and the database.</summary>
        public static string Title(int selectedCount, string database)
        {
            return string.Format("Apply {0} changes to {1}", selectedCount, database);
        }

        /// <summary>The primary button's label.</summary>
        public static string ApplyLabel(int selectedCount)
        {
            return string.Format("Apply {0} changes", selectedCount);
        }
    }
}
