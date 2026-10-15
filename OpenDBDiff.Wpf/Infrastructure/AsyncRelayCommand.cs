using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OpenDBDiff.Wpf.Infrastructure
{
    /// <summary>
    /// ICommand for work that leaves the UI thread (loading two schemas and comparing them).
    /// Guards against re-entry: the button stays disabled for the duration of the operation.
    /// </summary>
    public sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> execute;
        private readonly Func<bool>? canExecute;
        private bool isRunning;

        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return !isRunning && (canExecute is null || canExecute());
        }

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            isRunning = true;
            RaiseCanExecuteChanged();

            try
            {
                await execute().ConfigureAwait(true);
            }
            finally
            {
                isRunning = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
