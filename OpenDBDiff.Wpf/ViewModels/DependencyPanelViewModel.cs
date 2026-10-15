using System.Collections.Generic;
using System.Linq;
using OpenDBDiff.Ui.Model;
using OpenDBDiff.Wpf.Infrastructure;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// Section 7.3's "Show dependencies…", as a panel rather than a dialog.
    /// </summary>
    /// <remarks>
    /// Section 6.10 allows a message-box-style modal only for the confirmation dialog and for unsaved
    /// work, so a list of object names is shown the way section 6.7.1's script preview is: over the
    /// detail pane, non-modal, with the list still usable underneath.
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public sealed class DependencyPanelViewModel : ObservableObject
    {
        public DependencyPanelViewModel(string objectName, IReadOnlyList<DiffItemKey> needs, IReadOnlyList<DiffItemKey> neededBy)
        {
            ObjectName = objectName ?? string.Empty;
            Needs = Names(needs);
            NeededBy = Names(neededBy);
        }

        public string ObjectName { get; private set; }

        public string Title
        {
            get { return "Dependencies for " + ObjectName; }
        }

        /// <summary>What this object needs, which is what can stop it being created or altered.</summary>
        public IReadOnlyList<string> Needs { get; private set; }

        /// <summary>What needs this object, which is what makes dropping it dangerous.</summary>
        public IReadOnlyList<string> NeededBy { get; private set; }

        public bool HasNeeds
        {
            get { return Needs.Count > 0; }
        }

        public bool HasNeededBy
        {
            get { return NeededBy.Count > 0; }
        }

        /// <summary>Section 9.4 calls the graph best effort, and the panel says so rather than implying it is exhaustive.</summary>
        public string Caveat
        {
            get { return "Best effort: the engine reads declared dependencies and foreign keys, so dynamic SQL is not visible here."; }
        }

        public string Summary
        {
            get
            {
                if (!HasNeeds && !HasNeededBy)
                {
                    return "Nothing declared, in either direction.";
                }

                var needs = Needs.Count == 1 ? "needs 1 object" : "needs " + Needs.Count + " objects";
                var neededBy = NeededBy.Count == 1 ? "needed by 1 object" : "needed by " + NeededBy.Count + " objects";

                return HasNeeds && HasNeededBy
                    ? needs + ", " + neededBy
                    : (HasNeeds ? needs : neededBy);
            }
        }

        private static IReadOnlyList<string> Names(IReadOnlyList<DiffItemKey> keys)
        {
            return keys == null
                ? new List<string>()
                : keys.Select(DependencyRules.Name).OrderBy(name => name, System.StringComparer.OrdinalIgnoreCase).ToList();
        }
    }
}
