using System;
using OpenDBDiff.Wpf.Infrastructure;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// Base for the dialogs the main window opens. A dialog asks to be closed rather than closing
    /// itself, so the view model never touches a Window.
    /// </summary>
    /// <remarks>
    /// It owns the change notification of <see cref="ObservableObject"/> so a dialog can raise a
    /// dependent property by hand - the typed confirmation of section 6.8 has to re-evaluate whether
    /// Apply is allowed, and PropertyChanged.Fody only knows about the property that was assigned.
    /// </remarks>
    public abstract class DialogViewModelBase : ObservableObject
    {
        public event EventHandler? CloseRequested;

        protected void RequestClose()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
