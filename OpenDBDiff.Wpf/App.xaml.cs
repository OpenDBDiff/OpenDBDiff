using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using OpenDBDiff.Ui.Model;
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
    /// <remarks>
    /// The theme is applied before the first window exists, and section 13.4's switch re-creates the
    /// window rather than re-resolving resources in place: a StaticResource is resolved when the view
    /// that uses it is loaded, so a new token dictionary only reaches views loaded after it. The view
    /// model is not re-created, which is what keeps the connections, the results and the selection intact
    /// across a theme change.
    /// </remarks>
    public partial class App : Application
    {
        private DialogService? dialogs;
        private MainViewModel? viewModel;
        private IThemeService? themes;
        private EventHandler? closeRequested;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            var settings = new JsonSettingsStore();
            var comparisons = new ComparisonService();

            dialogs = new DialogService();
            themes = new ThemeService(RebuildWindow);

            viewModel = new MainViewModel(comparisons, settings, dialogs, themes);

            // Section 13.4: the theme follows the operating system until the View menu says otherwise, and
            // it is applied before the window is built, so the first window is already the right colours.
            themes.Apply(Theme.Parse(viewModel.Theme));

            MainWindow = CreateWindow();
            MainWindow.Show();
        }

        private Window CreateWindow()
        {
            var window = new MainWindow { DataContext = viewModel };

            // The view model does not own Window, but it needs to raise dialogs for the owner window.
            if (dialogs != null)
            {
                dialogs.Owner = window;
            }

            // File → Exit is a command on the view model; only the window can actually close.
            if (viewModel != null)
            {
                if (closeRequested != null)
                {
                    viewModel.WindowClosingRequested -= closeRequested;
                }

                closeRequested = (_, _) => window.Close();
                viewModel.WindowClosingRequested += closeRequested;
            }

            return window;
        }

        /// <summary>
        /// Shows a window built against the theme that was just applied, then closes the old one. The new
        /// window is shown first, so the application never has zero windows and never exits by accident.
        /// </summary>
        private void RebuildWindow()
        {
            var previous = MainWindow;
            var window = CreateWindow();

            MainWindow = window;
            window.Show();

            if (previous != null && !ReferenceEquals(previous, window))
            {
                previous.Close();
            }
        }
    }
}
