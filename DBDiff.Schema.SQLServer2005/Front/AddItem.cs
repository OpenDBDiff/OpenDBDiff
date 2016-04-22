using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DBDiff.Schema.SQLServer.Generates.Options;

namespace DBDiff.Schema.SQLServer.Generates.Front
{
    public partial class AddItem : Form
    {
        private class ObjectTypeComboBoxItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        private IList<ObjectTypeComboBoxItem> items = new List<ObjectTypeComboBoxItem>();

        private SqlOption sqlOption;
        private int indexFilter;

        public AddItem(SqlOption sqlOption, int Index)
        {
            InitializeComponent();
            FillCombo();
            this.sqlOption = sqlOption;
            indexFilter = Index;
            if (indexFilter != -1)
            {
                txtFilter.Text = sqlOption.Filters.Items[indexFilter].Filter;
                cboObjects.SelectedValue = sqlOption.Filters.Items[indexFilter].Type.ToString();
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

        private void FillCombo()
        {
            foreach (Enums.ObjectType state in Enum.GetValues(typeof(Enums.ObjectType)).Cast<Enums.ObjectType>())
            {
                var item = new ObjectTypeComboBoxItem();
                item.Id = state.ToString();
                item.Name = GetEnumDescription(state);
                items.Add(item);
            }
            cboObjects.DataSource = items.OrderBy(i => i.Name).ToList();
            cboObjects.DisplayMember = "Name";
            cboObjects.ValueMember = "Id";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (cboObjects.SelectedItem == null)
            {
                MessageBox.Show(this, "All fields are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var fi = new SqlOptionFilterItem((Enums.ObjectType)Enum.Parse(typeof(Enums.ObjectType), cboObjects.SelectedValue.ToString(), true), txtFilter.Text);

            if (sqlOption.Filters.Items.Contains(fi))
            {
                MessageBox.Show(this, string.Format("The list of name filters already includes an entry for text '{0}' of type '{1}'", fi.Filter, fi.Type.ToString()), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }


            if (indexFilter == -1)
                sqlOption.Filters.Items.Add(fi);
            else
            {
                sqlOption.Filters.Items[indexFilter].Filter = fi.Filter;
                sqlOption.Filters.Items[indexFilter].Type = fi.Type;
            }
            HandlerHelper.RaiseOnChange();
            this.Dispose();
        }
    }
}
