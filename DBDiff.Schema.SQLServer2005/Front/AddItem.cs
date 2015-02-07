using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using DBDiff.Schema.SQLServer.Generates.Options;

namespace DBDiff.Schema.SQLServer.Generates.Front
{
    public partial class AddItem : Form
    {
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
                cboObjects.SelectedIndex = (int)sqlOption.Filters.Items[indexFilter].Type;
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

        public static IEnumerable<T> EnumToList<T>()
        {
            Type enumType = typeof(T);
            Array enumValArray = Enum.GetValues(enumType);
            List<T> enumValList = new List<T>(enumValArray.Length);

            foreach (int val in enumValArray)
            {
                enumValList.Add((T)Enum.Parse(enumType, val.ToString()));
            }

            return enumValList;
        }

        private void FillCombo()
        {
            foreach (Enums.ObjectType state in EnumToList<Enums.ObjectType>())
            {
                cboObjects.Items.Add(GetEnumDescription(state));
            }
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
                    sqlOption.Filters.Items.Add(new SqlOptionFilterItem((Enums.ObjectType)Enum.Parse(typeof(Enums.ObjectType), cboObjects.SelectedItem.ToString(), true), txtFilter.Text));
                else
                {
                    sqlOption.Filters.Items[indexFilter].Filter = txtFilter.Text;
                    sqlOption.Filters.Items[indexFilter].Type = (Enums.ObjectType)Enum.Parse(typeof(Enums.ObjectType), cboObjects.SelectedItem.ToString(), true);
                }
                HandlerHelper.RaiseOnChange();
                this.Dispose();
            }
            else
                MessageBox.Show(this,"Must complete all fields","ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void AddItem_Load(object sender, EventArgs e)
        {

        }
    }
}
