using System;
using System.Windows.Input;

namespace OpenDBDiff.Wpf.Infrastructure
{
    /// <summary>
    /// Minimal ICommand for the view models. Fody handles change notification; commands stay
    /// explicit so that CanExecute state is visible where it is defined.
    /// </summary>
    public sealed class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool>? canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return canExecute is null || canExecute();
        }

        public void Execute(object? parameter)
        {
            execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
