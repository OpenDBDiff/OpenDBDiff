using OpenDBDiff.Schema.SQLServer.Generates.Options;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace OpenDBDiff.Schema.SQLServer.Generates.Front
{
    public partial class AddExclusionPatternForm : Form
    {
        private SqlOption sqlOption;
        private int indexFilter;

        public AddExclusionPatternForm(SqlOption sqlOption)
            : this(sqlOption, -1)
        { }

        public AddExclusionPatternForm(SqlOption sqlOption, int Index)
        {
            InitializeComponent();

            PopulateObjectTypeDropDownList();

            this.sqlOption = sqlOption;
            indexFilter = Index;
            if (indexFilter != -1)
            {
                txtFilter.Text = sqlOption.Filters.Items[indexFilter].FilterPattern;
                cboObjects.SelectedValue = sqlOption.Filters.Items[indexFilter].ObjectType;
            }
        }

        private string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        private void PopulateObjectTypeDropDownList()
        {
            var data = Enum.GetValues(typeof(ObjectType)).Cast<ObjectType>()
                .Select(ot => new { ObjectType = ot, Description = GetEnumDescription(ot) })
                .OrderBy(a => a.Description)
                .ToList();

            cboObjects.DataSource = data;
            cboObjects.DisplayMember = "Description";
            cboObjects.ValueMember = "ObjectType";
        }

        private void CancelFormButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (cboObjects.SelectedItem == null)
            {
                MessageBox.Show(this, "All fields are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var fi = new SqlOptionFilterItem((ObjectType)Enum.Parse(typeof(ObjectType), cboObjects.SelectedValue.ToString(), true), txtFilter.Text);

            if (sqlOption.Filters.Items.Contains(fi))
            {
                MessageBox.Show(this, string.Format("The list of name filters already includes an entry for text '{0}' of type '{1}'", fi.FilterPattern, fi.ObjectType.ToString()), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (indexFilter == -1)
                sqlOption.Filters.Items.Add(fi);
            else
            {
                sqlOption.Filters.Items[indexFilter].FilterPattern = fi.FilterPattern;
                sqlOption.Filters.Items[indexFilter].ObjectType = fi.ObjectType;
            }
            HandlerHelper.RaiseOnChange();

            this.Close();
        }
    }
}
