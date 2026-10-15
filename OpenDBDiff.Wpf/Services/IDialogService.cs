using System;
using System.Windows;

namespace OpenDBDiff.Wpf.Services
{
    /// <summary>
    /// Everything the view models need from the windowing system. Keeping this behind an interface is
    /// what lets a view model be exercised without a window.
    /// </summary>
    public interface IDialogService
    {
        Window? Owner { get; set; }

        /// <summary>Shows a dialog and waits for it. The window closes when the view model asks.</summary>
        void ShowDialog(ViewModels.DialogViewModelBase viewModel);

        /// <summary>Shows a window that does not block the caller, e.g. the comparison progress.</summary>
        void ShowModeless(ViewModels.DialogViewModelBase viewModel);

        bool Confirm(string message, string caption);

        void Inform(string caption, string message);

        void ShowError(string location, Exception exception);

        string? AskSaveScriptPath(string suggestedFileName);

        string? AskOpenProjectPath();
    }
}
