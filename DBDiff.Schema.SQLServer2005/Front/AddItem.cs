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
            if (cboObjects.SelectedItem != null)
            {
                if (indexFilter == -1)
                    sqlOption.Filters.Items.Add(new SqlOptionFilterItem((Enums.ObjectType)Enum.Parse(typeof(Enums.ObjectType), cboObjects.SelectedValue.ToString(), true), txtFilter.Text));
                else
                {
                    sqlOption.Filters.Items[indexFilter].Filter = txtFilter.Text;
                    sqlOption.Filters.Items[indexFilter].Type = (Enums.ObjectType)Enum.Parse(typeof(Enums.ObjectType), cboObjects.SelectedValue.ToString(), true);
                }
                HandlerHelper.RaiseOnChange();
                this.Dispose();
            }
            else
                MessageBox.Show(this, "Must complete all fields", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
