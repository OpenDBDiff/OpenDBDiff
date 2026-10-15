using System;
using System.Windows;
using Microsoft.Win32;
using OpenDBDiff.Wpf.ViewModels;
using OpenDBDiff.Wpf.Views;

namespace OpenDBDiff.Wpf.Services
{
    /// <summary>
    /// Windows-based implementation of <see cref="IDialogService"/>. Dialog content is chosen by data
    /// template (see App.xaml), so this class never names a view.
    /// </summary>
    public sealed class DialogService : IDialogService
    {
        public Window? Owner { get; set; }

        public void ShowDialog(DialogViewModelBase viewModel)
        {
            var window = Prepare(viewModel);
            window.ShowDialog();
        }

        public void ShowModeless(DialogViewModelBase viewModel)
        {
            var window = Prepare(viewModel);
            window.Show();
        }

        public bool Confirm(string message, string caption)
        {
            return MessageBox.Show(Owner ?? Application.Current.MainWindow, message, caption,
                MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
        }

        public void Inform(string caption, string message)
        {
            MessageBox.Show(Owner ?? Application.Current.MainWindow, message, caption,
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string location, Exception exception)
        {
            ShowDialog(new ErrorViewModel(location, exception));
        }

        /// <summary>
        /// Section 11's "Last used folder for scripts/reports". It is not a property of a dialog: the
        /// folder a person keeps their scripts in is a fact about them, so it survives the dialog that
        /// discovered it and the session that used it.
        /// </summary>
        public string? LastSaveFolder { get; set; }

        private string? FolderOf(string? fileName)
        {
            return string.IsNullOrEmpty(fileName) ? null : System.IO.Path.GetDirectoryName(fileName);
        }

        private string? StartIn(string suggestedFileName)
        {
            return string.IsNullOrEmpty(LastSaveFolder)
                ? null
                : System.IO.Path.Combine(LastSaveFolder!, suggestedFileName);
        }

        public string? AskSaveScriptPath(string suggestedFileName)
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".sql",
                Filter = "SQL File|*.sql",
                FileName = string.IsNullOrEmpty(LastSaveFolder) ? suggestedFileName : StartIn(suggestedFileName),
                OverwritePrompt = true
            };

            if (dialog.ShowDialog(Owner) != true)
            {
                return null;
            }

            LastSaveFolder = FolderOf(dialog.FileName);

            return dialog.FileName;
        }

        public string? AskOpenProjectPath()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "OpenDBDiff project|*.json;*.db|All files|*.*"
            };

            return dialog.ShowDialog(Owner) == true ? dialog.FileName : null;
        }

        public string? AskText(string title, string prompt, string initialValue)
        {
            var model = new TextPromptViewModel(title, prompt, initialValue);

            ShowDialog(model);

            return model.Result;
        }

        public string? AskSaveProjectPath(string suggestedFileName)
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".odbdproj",
                Filter = "OpenDBDiff project|*.odbdproj|All files|*.*",
                FileName = suggestedFileName,
                OverwritePrompt = true
            };

            return dialog.ShowDialog(Owner) == true ? dialog.FileName : null;
        }

        /// <summary>Section 6.9's "Save report": HTML by default, CSV when the user picks it.</summary>
        public string? AskSaveReportPath(string suggestedFileName)
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".html",
                Filter = "Report|*.html|Comma separated|*.csv|All files|*.*",
                FileName = string.IsNullOrEmpty(LastSaveFolder) ? suggestedFileName : StartIn(suggestedFileName),
                OverwritePrompt = true
            };

            if (dialog.ShowDialog(Owner) != true)
            {
                return null;
            }

            LastSaveFolder = FolderOf(dialog.FileName);

            return dialog.FileName;
        }

        private DialogWindow Prepare(DialogViewModelBase viewModel)
        {
            var window = new DialogWindow { DataContext = viewModel };

            if (Owner != null)
            {
                window.Owner = Owner;
            }

            void OnCloseRequested(object? sender, EventArgs e)
            {
                window.Close();
            }

            viewModel.CloseRequested += OnCloseRequested;
            window.Closed += (_, _) => viewModel.CloseRequested -= OnCloseRequested;

            return window;
        }
    }
}
