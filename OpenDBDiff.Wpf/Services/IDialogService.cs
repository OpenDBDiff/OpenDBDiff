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

        /// <summary>Section 11's "Last used folder for scripts/reports", Documents by default.</summary>
        string? LastSaveFolder { get; set; }

        string? AskSaveScriptPath(string suggestedFileName);

        string? AskOpenProjectPath();

        /// <summary>Where File → Save project as… should write (section 12).</summary>
        string? AskSaveProjectPath(string suggestedFileName);

        /// <summary>Where section 6.9's "Save report" should write (an .html or .csv file).</summary>
        string? AskSaveReportPath(string suggestedFileName);

        /// <summary>
        /// Asks for a short line of text (a connection name, a project name). Returns null when the
        /// user cancelled. Section 6.1.1's "Save as…" needs this and a MessageBox would break section
        /// 6.10's rule against message-box modals.
        /// </summary>
        string? AskText(string title, string prompt, string initialValue);
    }
}
