using System;
using System.Windows.Input;

namespace OpenDBDiff.Wpf.Infrastructure
{
    /// <summary>
    /// The parameterised RelayCommand, used where one command serves several callers: the Recent
    /// comparisons menu, a chip that opens the panel on its own side, and the theme choices.
    /// </summary>
    public sealed class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool>? canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return canExecute is null || canExecute(Cast(parameter));
        }

        public void Execute(object? parameter)
        {
            execute(Cast(parameter));
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>A command parameter arrives as object; a mismatch is a binding bug, not a value to coerce.</summary>
        private static T Cast(object? parameter)
        {
            if (parameter == null)
            {
                return default!;
            }

            return parameter is T typed ? typed : default!;
        }
    }
}
