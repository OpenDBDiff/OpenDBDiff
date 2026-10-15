using System.Windows.Controls;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// The script preview panel of section 6.7.1, laid over the detail pane. Its data context is the main
    /// view model: the script's own words come from the preview view model, the buttons from the shell.
    /// </summary>
    public partial class ScriptPreviewPanelView : UserControl
    {
        public ScriptPreviewPanelView()
        {
            InitializeComponent();
        }
    }
}
