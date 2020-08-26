using OpenDBDiff.Front;
using System;
using System.Windows.Forms;

namespace OpenDBDiff.UI
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
            SqlFilter = filter;
            sqlOptionsFront1.Load(filter);

            InitializeComponent();

            this.SuspendLayout();

            this.sqlOptionsFront1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlOptionsFront1.Location = new System.Drawing.Point(3, 3);
            this.sqlOptionsFront1.Name = "sqlOptionsFront1";
            this.sqlOptionsFront1.Size = new System.Drawing.Size(586, 440);
            this.sqlOptionsFront1.TabIndex = 0;
            this.Controls.Add(this.sqlOptionsFront1);

            this.ResumeLayout();
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
