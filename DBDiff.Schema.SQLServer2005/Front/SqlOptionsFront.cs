using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DBDiff.Schema.SQLServer.Options;

namespace DBDiff.Schema.SQLServer.Front
{
    public partial class SqlOptionsFront : UserControl
    {
        private SqlOption option;

        public SqlOptionsFront()
        {
            InitializeComponent();
            HandlerHelper.OnChange += new HandlerHelper.SaveFilterHandler(HandlerHelper_OnChange);
        }

        public void HandlerHelper_OnChange()
        {
            LoadFilters();
        }

        private void LoadFilters()
        {
            lstFilters.Items.Clear();
            foreach (SqlOptionFilterItem item in option.Filters.Items)
            {
                ListViewItem lview = new ListViewItem(item.Filter);
                lview.SubItems.Add(item.Type.ToString());
                lstFilters.Items.Add(lview);
            };
        }

        public void Load(SqlOption valueOption)
        {
            this.option = valueOption;
            txtBlob.Text = option.Defaults.DefaultBlobValue;
            txtDate.Text = option.Defaults.DefaultDateValue;
            txtDefaultInteger.Text = option.Defaults.DefaultIntegerValue;
            txtDefaultReal.Text = option.Defaults.DefaultRealValue;
            txtNText.Text = option.Defaults.DefaultNTextValue;
            txtText.Text = option.Defaults.DefaultTextValue;
            txtVariant.Text = option.Defaults.DefaultVariantValue;

            chkCompAssemblys.Checked = option.Ignore.FilterAssemblies;
            chkCompConstraints.Checked = option.Ignore.FilterConstraint;
            chkCompExtendedProperties.Checked = option.Ignore.FilterExtendedPropertys;
            chkCompFunciones.Checked = option.Ignore.FilterFunction;
            chkCompIndices.Checked = option.Ignore.FilterIndex;
            chkIgnoreFillFactor.Checked = option.Ignore.FilterIgnoreFillFactor;
            chkIgnoreInclude.Checked = option.Ignore.FilterIgnoreIncludeColumns;
            chkCompSchemas.Checked = option.Ignore.FilterSchema;
            chkCompStoreProcedure.Checked = option.Ignore.FilterStoreProcedure;
            chkCompTablaOpciones.Checked = option.Ignore.FilterTableOption;
            chkCompTablas.Checked = option.Ignore.FilterTable;
            chkColumnIdentity.Checked = option.Ignore.FilterIgnoreColumnIdentity;
            chkColumnCollation.Checked = option.Ignore.FilterIgnoreColumnCollation;
            chkColumnOrder.Checked = option.Ignore.FilterIgnoreColumnOrder;
            chkIgnoreNotForReplication.Checked = option.Ignore.FilterIgnoreNotForReplication;

            chkCompTriggersDDL.Checked = option.Ignore.FilterDDLTriggers;
            chkCompTriggers.Checked = option.Ignore.FilterTrigger;
            chkCompUDT.Checked = option.Ignore.FilterUserDataType;
            chkCompVistas.Checked = option.Ignore.FilterView;
            chkCompXMLSchemas.Checked = option.Ignore.FilterXMLSchema;
            chkFileGroups.Checked = option.Ignore.FilterTableFileGroup;
            chkCompUsers.Checked = option.Ignore.FilterUsers;
            chkCompRoles.Checked = option.Ignore.FilterRoles;
            chkCompRules.Checked = option.Ignore.FilterRules;
            if (option.Script.AlterObjectOnSchemaBinding)
                optScriptSchemaBindingAlter.Checked = true;
            else
                optScriptSchemaDrop.Checked = true;
            LoadFilters();
        }

        public void Save()
        {
            option.Defaults.DefaultBlobValue = txtBlob.Text;
            option.Defaults.DefaultDateValue = txtDate.Text;
            option.Defaults.DefaultIntegerValue = txtDefaultInteger.Text;
            option.Defaults.DefaultNTextValue = txtNText.Text;
            option.Defaults.DefaultRealValue = txtDefaultReal.Text;
            option.Defaults.DefaultTextValue = txtText.Text;
            option.Defaults.DefaultVariantValue = txtVariant.Text;

            option.Ignore.FilterAssemblies = chkCompAssemblys.Checked;
            option.Ignore.FilterConstraint = chkCompConstraints.Checked;
            option.Ignore.FilterFunction = chkCompFunciones.Checked;
            option.Ignore.FilterIndex = chkCompIndices.Checked;
            option.Ignore.FilterIgnoreFillFactor = chkIgnoreFillFactor.Checked && chkCompIndices.Checked;
            option.Ignore.FilterIgnoreIncludeColumns = chkIgnoreInclude.Checked && chkCompIndices.Checked;
            option.Ignore.FilterSchema = chkCompSchemas.Checked;
            option.Ignore.FilterStoreProcedure = chkCompStoreProcedure.Checked;
            option.Ignore.FilterTable = chkCompTablas.Checked;
            option.Ignore.FilterIgnoreColumnIdentity = chkColumnIdentity.Checked && chkCompTablas.Checked;
            option.Ignore.FilterIgnoreColumnCollation = chkColumnCollation.Checked && chkCompTablas.Checked;
            option.Ignore.FilterIgnoreColumnOrder = chkColumnOrder.Checked && chkCompTablas.Checked;
            option.Ignore.FilterTableFileGroup = chkFileGroups.Checked;
            option.Ignore.FilterTableOption = chkCompTablaOpciones.Checked;
            option.Ignore.FilterTrigger = chkCompTriggers.Checked;
            option.Ignore.FilterDDLTriggers = chkCompTriggersDDL.Checked;
            option.Ignore.FilterUserDataType = chkCompUDT.Checked;
            option.Ignore.FilterView = chkCompVistas.Checked;
            option.Ignore.FilterXMLSchema = chkCompXMLSchemas.Checked;
            option.Ignore.FilterExtendedPropertys = chkCompExtendedProperties.Checked;
            option.Ignore.FilterUsers = chkCompUsers.Checked;
            option.Ignore.FilterRoles = chkCompRoles.Checked;
            option.Ignore.FilterRules = chkCompRules.Checked;
            option.Ignore.FilterIgnoreNotForReplication = chkIgnoreNotForReplication.Checked;
            option.Script.AlterObjectOnSchemaBinding = optScriptSchemaBindingAlter.Checked;
        }

        private void chkCompIndices_CheckedChanged(object sender, EventArgs e)
        {
            chkIgnoreFillFactor.Enabled = chkCompIndices.Checked;
            chkIgnoreInclude.Enabled = chkCompIndices.Checked;
        }

        private void chkCompTablas_CheckedChanged(object sender, EventArgs e)
        {
            chkColumnCollation.Enabled = chkCompTablas.Checked;
            chkColumnIdentity.Enabled = chkCompTablas.Checked;
            chkColumnOrder.Enabled = chkCompTablas.Checked;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (lstFilters.SelectedItems.Count > 0)
            {
                AddItem itemForm = new AddItem(option, lstFilters.SelectedItems[0].Index);
                itemForm.Show(this);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddItem itemForm = new AddItem(option, -1);
            itemForm.Show(this);
        }
    }
}
