using System;
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
        public ScriptPreviewViewModel(string script, int objectCount)
        {
            Script = script;
            Title = string.Format("Script for {0} selected changes", objectCount);
            CopyCommand = new RelayCommand(Copy);
        }

        public string Title { get; }

        public string Script { get; }

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
