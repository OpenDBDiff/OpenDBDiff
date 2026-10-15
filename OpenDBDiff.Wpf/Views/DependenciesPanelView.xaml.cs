using System.Windows.Controls;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// Section 7.3's dependency listing, laid over the detail pane. Its data context is the main view
    /// model, because closing it is the shell's business.
    /// </summary>
    public partial class DependenciesPanelView : UserControl
    {
        public DependenciesPanelView()
        {
            InitializeComponent();
        }
    }
}
