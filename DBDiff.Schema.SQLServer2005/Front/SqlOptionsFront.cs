using System;
using System.Linq;
using System.Windows.Forms;
using DBDiff.Schema.SQLServer.Generates.Options;

namespace DBDiff.Schema.SQLServer.Generates.Front
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

        public new void Load(SqlOption valueOption)
        {
            this.option = valueOption;
            txtBlob.Text = option.Defaults.DefaultBlobValue;
            txtDate.Text = option.Defaults.DefaultDateValue;
            txtDefaultInteger.Text = option.Defaults.DefaultIntegerValue;
            txtDefaultReal.Text = option.Defaults.DefaultRealValue;
            txtNText.Text = option.Defaults.DefaultNTextValue;
            txtText.Text = option.Defaults.DefaultTextValue;
            txtVariant.Text = option.Defaults.DefaultVariantValue;
            txtTime.Text = option.Defaults.DefaultTime;
            txtXML.Text = option.Defaults.DefaultXml;

            chkCompAssemblys.Checked = option.Ignore.FilterAssemblies;
            chkCompCLRFunctions.Checked = option.Ignore.FilterCLRFunction;
            chkCompCLRStore.Checked = option.Ignore.FilterCLRStoredProcedure;
            chkCompCLRTrigger.Checked = option.Ignore.FilterCLRTrigger;
            chkCompCLRUDT.Checked = option.Ignore.FilterCLRUDT;

            chkConstraints.Checked = option.Ignore.FilterConstraint;
            chkConstraintsPK.Checked = option.Ignore.FilterConstraintPK;
            chkConstraintsFK.Checked = option.Ignore.FilterConstraintFK;
            chkConstraintsUK.Checked = option.Ignore.FilterConstraintUK;
            chkConstraintsCheck.Checked = option.Ignore.FilterConstraintCheck;

            chkCompExtendedProperties.Checked = option.Ignore.FilterExtendedProperties;
            chkCompFunciones.Checked = option.Ignore.FilterFunction;
            chkIndex.Checked = option.Ignore.FilterIndex;
            chkIndexFillFactor.Checked = option.Ignore.FilterIndexFillFactor;
            chkIndexIncludeColumns.Checked = option.Ignore.FilterIndexIncludeColumns;
            chkIndexFilter.Checked = option.Ignore.FilterIndexFilter;
            chkFullText.Checked = option.Ignore.FilterFullText;
            chkFullTextPath.Checked = option.Ignore.FilterFullTextPath;

            chkCompSchemas.Checked = option.Ignore.FilterSchema;
            chkCompStoredProcedure.Checked = option.Ignore.FilterStoredProcedure;
            chkTableOption.Checked = option.Ignore.FilterTableOption;
            chkTables.Checked = option.Ignore.FilterTable;
            chkTablesColumnIdentity.Checked = option.Ignore.FilterColumnIdentity;
            chkTablesColumnCollation.Checked = option.Ignore.FilterColumnCollation;
            chkTableLockEscalation.Checked = option.Ignore.FilterTableLockEscalation;
            chkTableChangeTracking.Checked = option.Ignore.FilterTableChangeTracking;

            chkTablesColumnOrder.Checked = option.Ignore.FilterColumnOrder;
            chkIgnoreNotForReplication.Checked = option.Ignore.FilterNotForReplication;

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

            if (option.Comparison.CaseSensityType == SqlOptionComparison.CaseSensityOptions.Automatic)
                rdoCaseAutomatic.Checked = true;
            if (option.Comparison.CaseSensityType == SqlOptionComparison.CaseSensityOptions.CaseInsensity)
                rdoCaseInsensitive.Checked = true;
            if (option.Comparison.CaseSensityType == SqlOptionComparison.CaseSensityOptions.CaseSensity)
                rdoCaseSensitive.Checked = true;

            if (option.Comparison.CaseSensityInCode == SqlOptionComparison.CaseSensityOptions.CaseInsensity)
                rdoCaseInsensityInCode.Checked = true;
            if (option.Comparison.CaseSensityInCode == SqlOptionComparison.CaseSensityOptions.CaseSensity)
                rdoCaseSensityInCode.Checked = true;

            chkIgnoreWhiteSpaceInCode.Checked = option.Comparison.IgnoreWhiteSpacesInCode;

            chkReloadDB.Checked = option.Comparison.ReloadComparisonOnUpdate;

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
            option.Defaults.DefaultTime = txtTime.Text;
            option.Defaults.DefaultXml = txtXML.Text;

            option.Ignore.FilterAssemblies = chkCompAssemblys.Checked;
            option.Ignore.FilterCLRFunction = chkCompCLRFunctions.Checked && chkCompAssemblys.Checked;
            option.Ignore.FilterCLRStoredProcedure = chkCompCLRStore.Checked && chkCompAssemblys.Checked;
            option.Ignore.FilterCLRTrigger = chkCompCLRTrigger.Checked && chkCompAssemblys.Checked;
            option.Ignore.FilterCLRUDT = chkCompCLRUDT.Checked && chkCompAssemblys.Checked;

            option.Ignore.FilterConstraint = chkConstraints.Checked;
            option.Ignore.FilterConstraintPK = chkConstraintsPK.Checked;
            option.Ignore.FilterConstraintFK = chkConstraintsFK.Checked;
            option.Ignore.FilterConstraintUK = chkConstraintsUK.Checked;
            option.Ignore.FilterConstraintCheck = chkConstraintsCheck.Checked;

            option.Ignore.FilterFunction = chkCompFunciones.Checked;

            option.Ignore.FilterIndex = chkIndex.Checked;
            option.Ignore.FilterIndexFillFactor = chkIndexFillFactor.Checked && chkIndex.Checked;
            option.Ignore.FilterIndexIncludeColumns = chkIndexIncludeColumns.Checked && chkIndex.Checked;
            option.Ignore.FilterIndexFilter = chkIndexFilter.Checked && chkIndex.Checked;

            option.Ignore.FilterSchema = chkCompSchemas.Checked;
            option.Ignore.FilterStoredProcedure = chkCompStoredProcedure.Checked;

            option.Ignore.FilterTable = chkTables.Checked;
            option.Ignore.FilterColumnIdentity = chkTablesColumnIdentity.Checked && chkTables.Checked;
            option.Ignore.FilterColumnCollation = chkTablesColumnCollation.Checked && chkTables.Checked;
            option.Ignore.FilterColumnOrder = chkTablesColumnOrder.Checked && chkTables.Checked;
            option.Ignore.FilterTableOption = chkTableOption.Checked && chkTables.Checked;
            option.Ignore.FilterTableLockEscalation = chkTableLockEscalation.Checked && chkTables.Checked;
            option.Ignore.FilterTableChangeTracking = chkTableChangeTracking.Checked && chkTables.Checked;

            option.Ignore.FilterTableFileGroup = chkFileGroups.Checked;
            option.Ignore.FilterTrigger = chkCompTriggers.Checked;
            option.Ignore.FilterDDLTriggers = chkCompTriggersDDL.Checked;
            option.Ignore.FilterUserDataType = chkCompUDT.Checked;
            option.Ignore.FilterView = chkCompVistas.Checked;
            option.Ignore.FilterXMLSchema = chkCompXMLSchemas.Checked;
            option.Ignore.FilterExtendedProperties = chkCompExtendedProperties.Checked;
            option.Ignore.FilterUsers = chkCompUsers.Checked;
            option.Ignore.FilterRoles = chkCompRoles.Checked;
            option.Ignore.FilterRules = chkCompRules.Checked;
            option.Ignore.FilterFullText = chkFullText.Checked;
            option.Ignore.FilterFullTextPath = chkFullTextPath.Checked;

            option.Ignore.FilterNotForReplication = chkIgnoreNotForReplication.Checked;
            option.Script.AlterObjectOnSchemaBinding = optScriptSchemaBindingAlter.Checked;

            if (rdoCaseAutomatic.Checked)
                option.Comparison.CaseSensityType = SqlOptionComparison.CaseSensityOptions.Automatic;
            if (rdoCaseInsensitive.Checked)
                option.Comparison.CaseSensityType = SqlOptionComparison.CaseSensityOptions.CaseInsensity;
            if (rdoCaseSensitive.Checked)
                option.Comparison.CaseSensityType = SqlOptionComparison.CaseSensityOptions.CaseSensity;

            if (rdoCaseInsensityInCode.Checked)
                option.Comparison.CaseSensityInCode = SqlOptionComparison.CaseSensityOptions.CaseInsensity;
            if (rdoCaseSensityInCode.Checked)
                option.Comparison.CaseSensityInCode = SqlOptionComparison.CaseSensityOptions.CaseSensity;

            option.Comparison.IgnoreWhiteSpacesInCode = chkIgnoreWhiteSpaceInCode.Checked;
            option.Comparison.ReloadComparisonOnUpdate = chkReloadDB.Checked;
        }

        private void chkCompIndices_CheckedChanged(object sender, EventArgs e)
        {
            chkIndexFillFactor.Enabled = chkIndex.Checked;
            chkIndexIncludeColumns.Enabled = chkIndex.Checked;
            chkIndexFilter.Enabled = chkIndex.Checked;
            chkIndexRowLock.Enabled = chkIndex.Checked;
        }

        private void chkCompTablas_CheckedChanged(object sender, EventArgs e)
        {
            chkTablesColumnCollation.Enabled = chkTables.Checked;
            chkTablesColumnIdentity.Enabled = chkTables.Checked;
            chkTablesColumnOrder.Enabled = chkTables.Checked;
            chkTableChangeTracking.Enabled = chkTables.Checked;
            chkTableLockEscalation.Enabled = chkTables.Checked;
            chkTableOption.Enabled = chkTables.Checked;
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

        private void chkConstraints_CheckedChanged(object sender, EventArgs e)
        {
            chkConstraintsFK.Enabled = chkConstraints.Checked;
            chkConstraintsPK.Enabled = chkConstraints.Checked;
            chkConstraintsUK.Enabled = chkConstraints.Checked;
            chkConstraintsCheck.Enabled = chkConstraints.Checked;
        }

        private void chkFullText_CheckedChanged(object sender, EventArgs e)
        {
            chkFullTextPath.Enabled = chkFullText.Checked;
        }

        private void chkCompAssemblys_CheckedChanged(object sender, EventArgs e)
        {
            chkCompCLRStore.Enabled = chkCompAssemblys.Checked;
            chkCompCLRTrigger.Enabled = chkCompAssemblys.Checked;
            chkCompCLRFunctions.Enabled = chkCompAssemblys.Checked;
            chkCompCLRUDT.Enabled = chkCompAssemblys.Checked;
        }

        private void DeleteNameFilterButton_Click(object sender, EventArgs e)
        {
            if (lstFilters.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstFilters.Items)
                {
                    if (item.Selected)
                    {
                        var type = (Enums.ObjectType)Enum.Parse(typeof(Enums.ObjectType), item.SubItems[1].Text);
                        var fi = new SqlOptionFilterItem(type, item.Text);
                        if (option.Filters.Items.Contains(fi))
                            option.Filters.Items.Remove(fi);
                    }
                }
                LoadFilters();
            }
        }
    }
}
