using System.ComponentModel;

namespace OpenDBDiff.Wpf.Infrastructure
{
    /// <summary>
    /// Owns the change notification event that PropertyChanged.Fody raises from the property setters,
    /// so a view model can also raise it for a computed property whose value changed without the
    /// property itself being set.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
