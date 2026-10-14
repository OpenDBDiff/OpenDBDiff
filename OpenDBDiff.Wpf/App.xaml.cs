using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using OpenDBDiff.Wpf.Services;
using OpenDBDiff.Wpf.ViewModels;
using OpenDBDiff.Wpf.Views;

namespace OpenDBDiff.Wpf
{
    /// <summary>
    /// Composition root: this front end has no DI container, so the object graph for the main window
    /// is built here. Only the SQL Server project handler exists today, which is why the database
    /// selector is created directly instead of through a project-handler factory.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            var settings = new JsonSettingsStore();
            var dialogs = new DialogService();
            var comparisons = new ComparisonService();

            var viewModel = new MainViewModel(comparisons, settings, dialogs);
            var window = new MainWindow { DataContext = viewModel };

            // The view model does not own Window, but it needs to raise dialogs for the owner window.
            dialogs.Owner = window;

            MainWindow = window;
            window.Show();
        }
    }
}
