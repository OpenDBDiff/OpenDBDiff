using System;
using System.Windows.Forms;

namespace OpenDBDiff.Front
{
    public partial class OptionForm : Form
    {
        private IProjectHandler projectSelectorHandler;
        private Schema.Model.IOption SqlFilter;

        public event OptionControl.OptionEventHandler OptionSaved;

        public OptionForm(IProjectHandler projectSelectorHandler, Schema.Model.IOption filter)
        {
            this.projectSelectorHandler = projectSelectorHandler;
            sqlOptionsFront1 = projectSelectorHandler.CreateOptionControl();
            sqlOptionsFront1.OptionSaved += SqlOptionsFront1_OptionSaved;

            InitializeComponent();

            SqlFilter = filter;
            sqlOptionsFront1.Load(filter);
        }

        private void SqlOptionsFront1_OptionSaved(Schema.Model.IOption option)
        {
            OptionSaved?.Invoke(option);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            sqlOptionsFront1.Save();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
