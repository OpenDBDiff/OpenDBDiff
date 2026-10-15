using System;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// Base for the dialogs the main window opens. A dialog asks to be closed rather than closing
    /// itself, so the view model never touches a Window.
    /// </summary>
    public abstract class DialogViewModelBase
    {
        public event EventHandler? CloseRequested;

        protected void RequestClose()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
