using System.Windows.Controls;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// Summary bar (section 6.3). Bindings only, except Ctrl+F, which has to move focus into the search
    /// box and so cannot be expressed as a binding.
    /// </summary>
    public partial class SummaryBarView : UserControl
    {
        public SummaryBarView()
        {
            InitializeComponent();
        }

        public void FocusSearch()
        {
            SearchBox.Focus();
            SearchBox.SelectAll();
        }
    }
}
