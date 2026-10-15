using System.Windows.Controls;
using System.Windows.Input;
using OpenDBDiff.Ui.Model;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// The review list (section 6.4). Its only code is the three keys section 7.1 rule 6 and section 7.2
    /// add to what a ListBox already does: Space toggles the focused row, Left and Right collapse and
    /// expand the group the focused row belongs to.
    /// </summary>
    public partial class ReviewListView : UserControl
    {
        public ReviewListView()
        {
            InitializeComponent();
        }

        private void Rows_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var row = Rows.SelectedItem as ResultsRowViewModel;

            if (row == null)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Space:
                    // Section 7.1 rule 6: the keyboard's version of clicking the checkbox.
                    if (row.Kind == ResultsRowKind.Item && row.CanToggle)
                    {
                        row.CheckBoxState = row.CheckBoxState == true ? false : true;
                    }
                    else if (row.Kind == ResultsRowKind.Group)
                    {
                        row.CheckState = row.CheckState == GroupCheckState.Checked
                            ? GroupCheckState.Unchecked
                            : GroupCheckState.Checked;
                    }
                    else
                    {
                        return;
                    }

                    e.Handled = true;
                    break;

                case Key.Right:
                    // Section 7.2: expand the group. On an item row this moves to its group first, which
                    // is what a tree does and what a reader expects.
                    if (row.Kind == ResultsRowKind.Group)
                    {
                        row.IsExpanded = true;
                    }
                    else
                    {
                        return;
                    }

                    e.Handled = true;
                    break;

                case Key.Left:
                    if (row.Kind == ResultsRowKind.Group)
                    {
                        row.IsExpanded = false;
                    }
                    else
                    {
                        return;
                    }

                    e.Handled = true;
                    break;
            }
        }
    }
}
