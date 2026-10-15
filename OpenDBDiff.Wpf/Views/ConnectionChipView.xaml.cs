using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// One connection chip (section 6.1.2). The command is a dependency property because the chip's
    /// data context is the connection, not the shell that owns the panel it opens.
    /// </summary>
    public partial class ConnectionChipView : UserControl
    {
        public static readonly DependencyProperty OpenCommandProperty = DependencyProperty.Register(
            nameof(OpenCommand),
            typeof(ICommand),
            typeof(ConnectionChipView),
            new PropertyMetadata(null));

        public ConnectionChipView()
        {
            InitializeComponent();
        }

        /// <summary>Opens the Connection Panel focused on this chip's side (section 6.1 item 5).</summary>
        public ICommand? OpenCommand
        {
            get { return (ICommand?)GetValue(OpenCommandProperty); }
            set { SetValue(OpenCommandProperty, value); }
        }
    }
}
