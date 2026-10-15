using System.Windows.Controls;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// The report of section 6.9. Its data context is the main view model, because the buttons that
    /// copy, save, retry and go back belong to the shell rather than to the report.
    /// </summary>
    public partial class ApplyReportView : UserControl
    {
        public ApplyReportView()
        {
            InitializeComponent();
        }
    }
}
