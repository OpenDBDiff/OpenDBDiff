using System;
using OpenDBDiff.Ui.Model;
using OpenDBDiff.Wpf.Infrastructure;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// The script preview of section 6.7.1. It is shown as a dialog here rather than as a panel that
    /// slides over the detail pane; the content and the actions are the same.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class ScriptPreviewViewModel : DialogViewModelBase
    {
        public ScriptPreviewViewModel(string script, int objectCount, bool isStale = false)
        {
            Script = script;
            Title = string.Format("Script for {0} selected changes", objectCount);
            IsStale = isStale;
            CopyCommand = new RelayCommand(Copy);
        }

        public string Title { get; }

        public string Script { get; }

        /// <summary>
        /// Section 9.6: while the results are stale the script actions still work, but the preview says
        /// which comparison the script came from.
        /// </summary>
        public bool IsStale { get; }

        public string RibbonText
        {
            get { return StaleRules.RibbonText; }
        }

        public RelayCommand CopyCommand { get; }

        public void Close()
        {
            RequestClose();
        }

        private void Copy()
        {
            System.Windows.Clipboard.SetText(Script);
        }
    }
}
