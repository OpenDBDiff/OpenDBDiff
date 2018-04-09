using System;
using System.Windows.Forms;

namespace OpenDBDiff.Front
{
    public partial class OptionForm : Form
    {
        private IProjectHandler projectSelectorHandler;
        private Schema.Model.IOption SqlFilter;
        public event OptionControl.OptionEventHandler OptionSaved;


        public OptionForm(IProjectHandler projectSelectorHandler)
        {
            this.projectSelectorHandler = projectSelectorHandler;
            sqlOptionsFront1 = projectSelectorHandler.CreateOptionControl();
            sqlOptionsFront1.OptionSaved += SqlOptionsFront1_OptionSaved;
            InitializeComponent();
        }

        private void SqlOptionsFront1_OptionSaved(Schema.Model.IOption option)
        {
            OptionSaved?.Invoke(option);
        }

        public void Show(IWin32Window owner, Schema.Model.IOption filter)
        {
            SqlFilter = filter;
            sqlOptionsFront1.Load(filter);
            this.Show(owner);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            sqlOptionsFront1.Save();
            this.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
