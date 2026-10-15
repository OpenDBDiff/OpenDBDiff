using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// The shell window. Everything in it is bound except four things a Window cannot bind: the save
    /// and restore of the window bounds, the keys that move focus or depend on what is on screen
    /// (Ctrl+F, Esc, F1), and the move of the single Menu between the bar and the narrow-width button
    /// (section 12).
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool menuIsInPopup;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        private MainViewModel? ViewModel
        {
            get { return DataContext as MainViewModel; }
        }

        /// <summary>Section 11: the bounds are restored only if they still fall on a monitor.</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = ViewModel;

            if (viewModel == null)
            {
                return;
            }

            if (viewModel.WindowWidth.HasValue && viewModel.WindowHeight.HasValue
                && viewModel.WindowLeft.HasValue && viewModel.WindowTop.HasValue
                && IsOnScreen(viewModel.WindowLeft.Value, viewModel.WindowTop.Value,
                    viewModel.WindowWidth.Value, viewModel.WindowHeight.Value))
            {
                Left = viewModel.WindowLeft.Value;
                Top = viewModel.WindowTop.Value;
                Width = viewModel.WindowWidth.Value;
                Height = viewModel.WindowHeight.Value;
            }

            if (string.Equals(viewModel.WindowStateText, WindowState.Maximized.ToString(), StringComparison.Ordinal))
            {
                WindowState = WindowState.Maximized;
            }

            MoveMenu(ActualWidth < 1180);
        }

        private static bool IsOnScreen(double left, double top, double width, double height)
        {
            var screen = new Rect(
                SystemParameters.VirtualScreenLeft,
                SystemParameters.VirtualScreenTop,
                SystemParameters.VirtualScreenWidth,
                SystemParameters.VirtualScreenHeight);

            return screen.IntersectsWith(new Rect(left, top, width, height));
        }

        /// <summary>
        /// Section 12: the menu bar collapses to a ≡ button at narrow widths. The one Menu instance is
        /// moved between the two hosts rather than the menu being written out twice, so the two can
        /// never drift apart.
        /// </summary>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel?.UpdateWidth(ActualWidth);
            MoveMenu(ActualWidth < 1180);
        }

        private void MoveMenu(bool intoPopup)
        {
            if (menuIsInPopup == intoPopup)
            {
                return;
            }

            menuIsInPopup = intoPopup;

            MenuHost.Child = null;
            MenuPopup.Child = null;

            if (intoPopup)
            {
                MenuPopup.Child = MainMenu;
            }
            else
            {
                MenuButton.IsChecked = false;
                MenuHost.Child = MainMenu;
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                SummaryBar.FocusSearch();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.F1)
            {
                ViewModel?.ShowShortcutsCommand.Execute(null);
                e.Handled = true;
                return;
            }

            // Section 7.2: Ctrl+Z, Ctrl+Y and Ctrl+A belong to the application unless the user is
            // typing in a field, where they mean the usual text editing. That is why they are handled
            // here rather than as window-wide input bindings.
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && !IsTextEditing())
            {
                if (e.Key == Key.Z)
                {
                    ViewModel?.UndoCommand.Execute(null);
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.Y)
                {
                    ViewModel?.RedoCommand.Execute(null);
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.A)
                {
                    ViewModel?.SelectAllVisibleCommand.Execute(null);
                    e.Handled = true;
                    return;
                }
            }

            // Section 7.2: Esc cancels the running operation, then closes the topmost overlay, then
            // clears the search - in that order.
            if (e.Key == Key.Escape)
            {
                var viewModel = ViewModel;

                if (viewModel == null)
                {
                    return;
                }

                if (viewModel.CancelCommand.CanExecute(null))
                {
                    viewModel.CancelCommand.Execute(null);
                }
                else if (viewModel.IsConnectionPanelOpen && !viewModel.IsConnectionPanelInline)
                {
                    viewModel.ClosePanelCommand.Execute(null);
                }
                else if (viewModel.IsScriptPreviewOpen)
                {
                    viewModel.CloseScriptPreviewCommand.Execute(null);
                }
                else if (viewModel.IsDependenciesOpen)
                {
                    viewModel.CloseDependenciesCommand.Execute(null);
                }
                else if (viewModel.Results != null && !string.IsNullOrEmpty(viewModel.Results.SearchText))
                {
                    viewModel.Results.SearchText = string.Empty;
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// True when the focused element is a text field, in which case the editing keys belong to it.
        /// </summary>
        private static bool IsTextEditing()
        {
            var focused = Keyboard.FocusedElement as DependencyObject;

            while (focused != null)
            {
                if (focused is TextBox || focused is PasswordBox || focused is RichTextBox)
                {
                    return true;
                }

                focused = System.Windows.Media.VisualTreeHelper.GetParent(focused);
            }

            return false;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var viewModel = ViewModel;

            if (viewModel == null)
            {
                return;
            }

            viewModel.WindowStateText = WindowState.ToString();

            if (WindowState == WindowState.Normal)
            {
                viewModel.WindowLeft = Left;
                viewModel.WindowTop = Top;
                viewModel.WindowWidth = Width;
                viewModel.WindowHeight = Height;
            }
            else
            {
                var bounds = RestoreBounds;

                viewModel.WindowLeft = bounds.Left;
                viewModel.WindowTop = bounds.Top;
                viewModel.WindowWidth = bounds.Width;
                viewModel.WindowHeight = bounds.Height;
            }

            viewModel.SaveWindowState();
        }
    }
}
