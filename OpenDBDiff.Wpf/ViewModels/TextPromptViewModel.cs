using System;
using System.Windows;
using OpenDBDiff.Wpf.Infrastructure;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// A one-line text prompt, used by "Save as…" for connection and project names (sections 6.1.1 and
    /// 12). A real dialog rather than a MessageBox, so section 6.10's "no MessageBox-style modals" holds.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class TextPromptViewModel : DialogViewModelBase
    {
        public TextPromptViewModel(string title, string prompt, string initialValue)
        {
            Title = title;
            Prompt = prompt;
            Value = initialValue ?? string.Empty;
            AcceptCommand = new RelayCommand(Accept, () => !string.IsNullOrWhiteSpace(Value));
            CancelCommand = new RelayCommand(Cancel);
        }

        public string Title { get; }

        public string Prompt { get; }

        public string Value { get; set; }

        public RelayCommand AcceptCommand { get; }

        public RelayCommand CancelCommand { get; }

        /// <summary>The value the user accepted, or null when they cancelled.</summary>
        public string? Result { get; private set; }

        public void Accept()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                return;
            }

            Result = Value.Trim();
            RequestClose();
        }

        public void Cancel()
        {
            Result = null;
            RequestClose();
        }

        private void OnValueChanged()
        {
            AcceptCommand.RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    /// A non-blocking toast, shown above the Action Bar (section 6.10). Errors stay up longer and offer
    /// a Details link.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class ToastViewModel : Infrastructure.ObservableObject
    {
        public ToastViewModel(string message, bool isError)
        {
            Message = message;
            IsError = isError;
        }

        public string Message { get; }

        public bool IsError { get; }

        /// <summary>Section 6.10: four seconds, eight for errors.</summary>
        public TimeSpan Lifetime
        {
            get { return IsError ? TimeSpan.FromSeconds(8) : TimeSpan.FromSeconds(4); }
        }
    }
}
