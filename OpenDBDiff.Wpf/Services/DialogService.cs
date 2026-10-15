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

        public string? AskSaveScriptPath(string suggestedFileName)
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".sql",
                Filter = "SQL File|*.sql",
                FileName = suggestedFileName,
                OverwritePrompt = true
            };

            return dialog.ShowDialog(Owner) == true ? dialog.FileName : null;
        }

        public string? AskOpenProjectPath()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "OpenDBDiff project|*.json;*.db|All files|*.*"
            };

            return dialog.ShowDialog(Owner) == true ? dialog.FileName : null;
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
