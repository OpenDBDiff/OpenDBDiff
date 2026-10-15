using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// The confirmation dialog (section 6.8). Cancel carries IsDefault so that pressing Enter does not
    /// apply anything, and the statement list scrolls the script rather than folding it away.
    /// </summary>
    public partial class ConfirmationView : UserControl
    {
        private ConfirmationViewModel subscribed;

        public ConfirmationView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (subscribed != null)
            {
                subscribed.StatementSelected -= OnStatementSelected;
                subscribed = null;
            }

            var viewModel = DataContext as ConfirmationViewModel;

            if (viewModel != null)
            {
                viewModel.StatementSelected += OnStatementSelected;
                subscribed = viewModel;
            }
        }

        /// <summary>
        /// Brings the clicked statement into view. A TextBox cannot scroll itself to a character offset,
        /// so the offset is turned into a line and the box scrolls to that line.
        /// </summary>
        private void OnStatementSelected(object sender, StatementSelectedEventArgs e)
        {
            var offset = e.Offset < 0 ? 0 : e.Offset;

            if (offset > ScriptBox.Text.Length)
            {
                offset = ScriptBox.Text.Length;
            }

            var length = e.Length;

            if (offset + length > ScriptBox.Text.Length)
            {
                length = ScriptBox.Text.Length - offset;
            }

            ScriptBox.Select(offset, length);

            var line = ScriptBox.GetLineIndexFromCharacterIndex(offset);

            if (line >= 0)
            {
                ScriptBox.ScrollToLine(line);
            }
        }

        private void StatementList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as ConfirmationViewModel;
            var statement = StatementList.SelectedItem as ConfirmationStatementViewModel;

            if (viewModel != null && statement != null)
            {
                viewModel.Select(statement);
            }
        }

        /// <summary>
        /// Section 6.8: the typed confirmation is a field to fill in, not a form to submit. Enter while
        /// it has focus must not reach the Apply button.
        /// </summary>
        private void TypeToConfirm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ConfirmationViewModel)?.Cancel();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ConfirmationViewModel)?.Apply();
        }
    }
}
