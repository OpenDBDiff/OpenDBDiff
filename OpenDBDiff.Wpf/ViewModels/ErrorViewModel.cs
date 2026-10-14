using System;
using System.Text;
using System.Windows;
using OpenDBDiff.Wpf.Infrastructure;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// The error report, replacing ErrorForm: what was being done, what went wrong, and the detail.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class ErrorViewModel : DialogViewModelBase
    {
        public ErrorViewModel(string location, Exception exception)
        {
            Location = location;
            Exception = exception;
            Report = BuildReport(location, exception);
            CopyCommand = new RelayCommand(() => Clipboard.SetText(Report));
        }

        public string Location { get; }

        public Exception Exception { get; }

        public string Message
        {
            get { return Exception.Message; }
        }

        public string Report { get; }

        public RelayCommand CopyCommand { get; }

        public void Close()
        {
            RequestClose();
        }

        private static string BuildReport(string location, Exception exception)
        {
            var report = new StringBuilder();

            report.AppendLine("Error " + location);
            report.AppendLine();

            for (var current = exception; current != null; current = current.InnerException)
            {
                report.AppendLine(current.GetType().FullName + ": " + current.Message);
                report.AppendLine(current.StackTrace);
                report.AppendLine();
            }

            return report.ToString();
        }
    }
}
