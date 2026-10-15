using OpenDBDiff.Wpf.Services;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// One progress bar in the comparison window, for one database being read.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class DatabaseProgressViewModel
    {
        public DatabaseProgressViewModel(string caption)
        {
            Caption = caption;
            Maximum = 1;
            Message = "Waiting";
        }

        /// <summary>
        /// Which database this bar belongs to, as "Server: …, Database: …". Neither bar is labelled
        /// "source" or "destination": the comparison pipeline loads the right-hand panel first, and
        /// naming the database it is reading is clearer than inheriting that inversion.
        /// </summary>
        public string Caption { get; set; }

        public int Value { get; private set; }

        public int Maximum { get; private set; }

        public string Message { get; private set; }

        public void Report(DatabaseProgress progress)
        {
            Maximum = progress.Maximum > 0 ? progress.Maximum : 1;

            if (progress.Value > -1)
            {
                Value = progress.Value;
            }

            if (!string.IsNullOrEmpty(progress.Message))
            {
                Message = progress.Message;
            }
        }

        public void Complete()
        {
            Value = Maximum;
            Message = "Complete";
        }
    }

    /// <summary>
    /// The comparison window: how far each database read has got, and whether it finished or failed.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class ProgressViewModel : DialogViewModelBase
    {
        public ProgressViewModel(string originCaption, string destinationCaption)
        {
            Origin = new DatabaseProgressViewModel(originCaption);
            Destination = new DatabaseProgressViewModel(destinationCaption);
        }

        public DatabaseProgressViewModel Origin { get; }

        public DatabaseProgressViewModel Destination { get; }

        public bool IsComplete { get; private set; }

        public bool HasError { get; private set; }

        public string? ErrorMessage { get; private set; }

        public bool CanClose
        {
            get { return IsComplete || HasError; }
        }

        public void Complete()
        {
            IsComplete = true;
        }

        public void Fail(string message)
        {
            HasError = true;
            ErrorMessage = message;
        }

        /// <summary>Called by the window's Close button once the run has finished.</summary>
        public void Close()
        {
            RequestClose();
        }
    }
}
