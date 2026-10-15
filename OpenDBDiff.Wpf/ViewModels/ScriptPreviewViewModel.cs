using OpenDBDiff.Ui.Model;
using OpenDBDiff.Wpf.Infrastructure;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// The script preview of section 6.7.1. It is a panel that slides over the detail pane rather than a
    /// window, and it re-generates while it is open so what it shows is the current selection's script.
    /// </summary>
    /// <remarks>
    /// The actions beside it - Copy, Save as…, Close - belong to the shell, because they need the
    /// clipboard and the file dialogs and the shell already owns those. This holds the words and the
    /// text, and it is mutable because the selection underneath it can change.
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public sealed class ScriptPreviewViewModel : ObservableObject
    {
        public ScriptPreviewViewModel(string script, int objectCount, ScriptBundle bundle, bool isStale)
        {
            Update(script, objectCount, bundle, isStale);
        }

        /// <summary>Section 6.7.1's title: "Script for {k} selected changes".</summary>
        public string Title { get; private set; }

        public string Script { get; private set; }

        /// <summary>Section 6.7.1's statement count and destructive summary, in the model's one wording.</summary>
        public string Summary { get; private set; }

        public bool HasDestructive { get; private set; }

        /// <summary>
        /// Section 9.6: while the results are stale the script actions still work, but the preview says
        /// which comparison the script came from.
        /// </summary>
        public bool IsStale { get; private set; }

        public string RibbonText
        {
            get { return StaleRules.RibbonText; }
        }

        /// <summary>
        /// Re-generates the panel in place, which is what section 6.7.1 asks for when the selection
        /// changes while it is open. Replacing the view model instead would lose the scroll position and
        /// make the panel flicker on every click in the list.
        /// </summary>
        public void Update(string script, int objectCount, ScriptBundle bundle, bool isStale)
        {
            var statements = bundle ?? ScriptBundle.Empty;

            Title = string.Format("Script for {0} selected changes", objectCount);
            Script = script ?? string.Empty;
            Summary = statements.Summary;
            HasDestructive = statements.DestructiveCount > 0;
            IsStale = isStale;
        }
    }
}
