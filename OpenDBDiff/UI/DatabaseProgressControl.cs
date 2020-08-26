using System.Windows.Forms;

namespace OpenDBDiff.UI
{
    public partial class DatabaseProgressControl : UserControl
    {
        public DatabaseProgressControl()
        {
            InitializeComponent();
        }

        public string DatabaseName
        {
            get { return lblDatabase.Text; }
            set { lblDatabase.Text = value; }
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
