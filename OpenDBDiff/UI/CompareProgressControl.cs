using System.Windows.Forms;

namespace OpenDBDiff.UI
{
    public partial class CompareProgressControl : UserControl
    {
        public CompareProgressControl()
        {
            InitializeComponent();
        }

        public string DatabaseName
        {
            get { return lblCompare.Text; }
            set { lblCompare.Text = value; }
        }

        public string Message
        {
            get
            {
                return lblMessage.Text;
            }
            set
            {
                lblMessage.Text = value;
                lblMessage.Refresh();
            }
        }

        public int Maximum
        {
            get { return progressBar1.Maximum; }
            set { progressBar1.Maximum = value; }
        }

        public int Value
        {
            get { return progressBar1.Value; }
            set
            {
                progressBar1.Value = value;
                progressBar1.Refresh();
            }
        }
    }
}
