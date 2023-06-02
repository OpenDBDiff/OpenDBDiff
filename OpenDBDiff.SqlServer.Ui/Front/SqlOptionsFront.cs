using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Options;
using System;
using System.Windows.Forms;

namespace OpenDBDiff.SqlServer.Ui
{
    public partial class SqlOptionsFront : OpenDBDiff.Abstractions.Ui.OptionControl
    {
        private SqlOption SQLOption;

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
            foreach (SqlOptionFilterItem item in SQLOption.Filters.Items)
            {
                var lview = new ListViewItem(item.FilterPattern);
                lview.SubItems.Add(item.ObjectType.ToString());
                lstFilters.Items.Add(lview);
            };
        }

        public override void Load(IOption option)
        {
            this.SQLOption = new SqlOption(option);
            txtBlob.Text = SQLOption.Defaults.DefaultBlobValue;
            txtDate.Text = SQLOption.Defaults.DefaultDateValue;
            txtDefaultInteger.Text = SQLOption.Defaults.DefaultIntegerValue;
            txtDefaultReal.Text = SQLOption.Defaults.DefaultRealValue;
            txtNText.Text = SQLOption.Defaults.DefaultNTextValue;
            txtText.Text = SQLOption.Defaults.DefaultTextValue;
            txtVariant.Text = SQLOption.Defaults.DefaultVariantValue;
            txtTime.Text = SQLOption.Defaults.DefaultTime;
            txtXML.Text = SQLOption.Defaults.DefaultXml;

            chkCompAssemblys.Checked = SQLOption.Ignore.FilterAssemblies;
            chkCompCLRFunctions.Checked = SQLOption.Ignore.FilterCLRFunction;
            chkCompCLRStore.Checked = SQLOption.Ignore.FilterCLRStoredProcedure;
            chkCompCLRTrigger.Checked = SQLOption.Ignore.FilterCLRTrigger;
            chkCompCLRUDT.Checked = SQLOption.Ignore.FilterCLRUDT;
            chkCompCLRAggregates.Checked = SQLOption.Ignore.FilterCLRAggregates;

            chkConstraints.Checked = SQLOption.Ignore.FilterConstraint;
            chkConstraintsPK.Checked = SQLOption.Ignore.FilterConstraintPK;
            chkConstraintsFK.Checked = SQLOption.Ignore.FilterConstraintFK;
            chkConstraintsUK.Checked = SQLOption.Ignore.FilterConstraintUK;
            chkConstraintsCheck.Checked = SQLOption.Ignore.FilterConstraintCheck;

            chkIndex.Checked = SQLOption.Ignore.FilterIndex;
            chkIndexFillFactor.Checked = SQLOption.Ignore.FilterIndexFillFactor;
            chkIndexIncludeColumns.Checked = SQLOption.Ignore.FilterIndexIncludeColumns;
            chkIndexFilter.Checked = SQLOption.Ignore.FilterIndexFilter;
            chkIndexRowLock.Checked = SQLOption.Ignore.FilterIndexRowLock;

            chkCompProgrammability.Checked = SQLOption.Ignore.FilterProgrammability;
            chkPartitionFunction.Checked = SQLOption.Ignore.FilterPartitionFunction;
            chkPartitionSchemas.Checked = SQLOption.Ignore.FilterPartitionScheme;
            chkCompTriggersDDL.Checked = SQLOption.Ignore.FilterDDLTriggers;
            chkCompFunctions.Checked = SQLOption.Ignore.FilterFunction;
            chkCompRules.Checked = SQLOption.Ignore.FilterRules;
            chkCompStoredProcedure.Checked = SQLOption.Ignore.FilterStoredProcedure;
            chkCompTriggers.Checked = SQLOption.Ignore.FilterTrigger;
            chkCompUDT.Checked = SQLOption.Ignore.FilterUserDataType;
            chkCompViews.Checked = SQLOption.Ignore.FilterView;
            chkCompXMLSchemas.Checked = SQLOption.Ignore.FilterXMLSchema;
            chkCompSynonyms.Checked = SQLOption.Ignore.FilterSynonyms;

            chkTables.Checked = SQLOption.Ignore.FilterTable;
            chkTablesColumnIdentity.Checked = SQLOption.Ignore.FilterColumnIdentity;
            chkTablesColumnCollation.Checked = SQLOption.Ignore.FilterColumnCollation;
            chkTableLockEscalation.Checked = SQLOption.Ignore.FilterTableLockEscalation;
            chkTableOption.Checked = SQLOption.Ignore.FilterTableOption;
            chkTableChangeTracking.Checked = SQLOption.Ignore.FilterTableChangeTracking;
            chkTablesColumnOrder.Checked = SQLOption.Ignore.FilterColumnOrder;

            chkIgnoreNotForReplication.Checked = SQLOption.Ignore.FilterNotForReplication;
            chkCompExtendedProperties.Checked = SQLOption.Ignore.FilterExtendedProperties;

            chkCompSecurity.Checked = SQLOption.Ignore.FilterSecurity;
            chkFileGroups.Checked = SQLOption.Ignore.FilterTableFileGroup;
            chkFullText.Checked = SQLOption.Ignore.FilterFullText;
            chkFullTextPath.Checked = SQLOption.Ignore.FilterFullTextPath;
            chkCompUsers.Checked = SQLOption.Ignore.FilterUsers;
            chkCompRoles.Checked = SQLOption.Ignore.FilterRoles;
            chkCompSchemas.Checked = SQLOption.Ignore.FilterSchema;
            chkCompPermissions.Checked = SQLOption.Ignore.FilterPermissions;

            if (SQLOption.Script.AlterObjectOnSchemaBinding)
                optScriptSchemaBindingAlter.Checked = true;
            else
                optScriptSchemaDrop.Checked = true;

            if (SQLOption.Comparison.CaseSensityType == SqlOptionComparison.CaseSensityOptions.Automatic)
                rdoCaseAutomatic.Checked = true;
            if (SQLOption.Comparison.CaseSensityType == SqlOptionComparison.CaseSensityOptions.CaseInsensity)
                rdoCaseInsensitive.Checked = true;
            if (SQLOption.Comparison.CaseSensityType == SqlOptionComparison.CaseSensityOptions.CaseSensity)
                rdoCaseSensitive.Checked = true;

            if (SQLOption.Comparison.CaseSensityInCode == SqlOptionComparison.CaseSensityOptions.CaseInsensity)
                rdoCaseInsensityInCode.Checked = true;
            if (SQLOption.Comparison.CaseSensityInCode == SqlOptionComparison.CaseSensityOptions.CaseSensity)
                rdoCaseSensityInCode.Checked = true;

            chkIgnoreWhiteSpaceInCode.Checked = SQLOption.Comparison.IgnoreWhiteSpacesInCode;

            chkReloadDB.Checked = SQLOption.Comparison.ReloadComparisonOnUpdate;

            LoadFilters();
        }

        public override void Save()
        {
            SQLOption.Defaults.DefaultBlobValue = txtBlob.Text;
            SQLOption.Defaults.DefaultDateValue = txtDate.Text;
            SQLOption.Defaults.DefaultIntegerValue = txtDefaultInteger.Text;
            SQLOption.Defaults.DefaultNTextValue = txtNText.Text;
            SQLOption.Defaults.DefaultRealValue = txtDefaultReal.Text;
            SQLOption.Defaults.DefaultTextValue = txtText.Text;
            SQLOption.Defaults.DefaultVariantValue = txtVariant.Text;
            SQLOption.Defaults.DefaultTime = txtTime.Text;
            SQLOption.Defaults.DefaultXml = txtXML.Text;

            SQLOption.Ignore.FilterAssemblies = chkCompAssemblys.Checked;
            SQLOption.Ignore.FilterCLRFunction = chkCompCLRFunctions.Checked && chkCompAssemblys.Checked;
            SQLOption.Ignore.FilterCLRStoredProcedure = chkCompCLRStore.Checked && chkCompAssemblys.Checked;
            SQLOption.Ignore.FilterCLRTrigger = chkCompCLRTrigger.Checked && chkCompAssemblys.Checked;
            SQLOption.Ignore.FilterCLRUDT = chkCompCLRUDT.Checked && chkCompAssemblys.Checked;
            SQLOption.Ignore.FilterCLRAggregates = chkCompCLRAggregates.Checked && chkCompAssemblys.Checked;

            SQLOption.Ignore.FilterConstraint = chkConstraints.Checked;
            SQLOption.Ignore.FilterConstraintPK = chkConstraintsPK.Checked;
            SQLOption.Ignore.FilterConstraintFK = chkConstraintsFK.Checked;
            SQLOption.Ignore.FilterConstraintUK = chkConstraintsUK.Checked;
            SQLOption.Ignore.FilterConstraintCheck = chkConstraintsCheck.Checked;

            SQLOption.Ignore.FilterIndex = chkIndex.Checked;
            SQLOption.Ignore.FilterIndexFillFactor = chkIndexFillFactor.Checked && chkIndex.Checked;
            SQLOption.Ignore.FilterIndexIncludeColumns = chkIndexIncludeColumns.Checked && chkIndex.Checked;
            SQLOption.Ignore.FilterIndexFilter = chkIndexFilter.Checked && chkIndex.Checked;
            SQLOption.Ignore.FilterIndexRowLock = chkIndexRowLock.Checked && chkIndex.Checked;

            SQLOption.Ignore.FilterTable = chkTables.Checked;
            SQLOption.Ignore.FilterColumnIdentity = chkTablesColumnIdentity.Checked && chkTables.Checked;
            SQLOption.Ignore.FilterColumnCollation = chkTablesColumnCollation.Checked && chkTables.Checked;
            SQLOption.Ignore.FilterColumnOrder = chkTablesColumnOrder.Checked && chkTables.Checked;
            SQLOption.Ignore.FilterTableOption = chkTableOption.Checked && chkTables.Checked;
            SQLOption.Ignore.FilterTableLockEscalation = chkTableLockEscalation.Checked && chkTables.Checked;
            SQLOption.Ignore.FilterTableChangeTracking = chkTableChangeTracking.Checked && chkTables.Checked;

            SQLOption.Ignore.FilterProgrammability = chkCompProgrammability.Checked;
            SQLOption.Ignore.FilterTrigger = chkCompTriggers.Checked;
            SQLOption.Ignore.FilterDDLTriggers = chkCompTriggersDDL.Checked;
            SQLOption.Ignore.FilterUserDataType = chkCompUDT.Checked;
            SQLOption.Ignore.FilterView = chkCompViews.Checked;
            SQLOption.Ignore.FilterXMLSchema = chkCompXMLSchemas.Checked;
            SQLOption.Ignore.FilterExtendedProperties = chkCompExtendedProperties.Checked;
            SQLOption.Ignore.FilterSynonyms = chkCompSynonyms.Checked;
            SQLOption.Ignore.FilterStoredProcedure = chkCompStoredProcedure.Checked;
            SQLOption.Ignore.FilterFunction = chkCompFunctions.Checked;
            SQLOption.Ignore.FilterPartitionScheme = chkPartitionSchemas.Checked;
            SQLOption.Ignore.FilterPartitionFunction = chkPartitionFunction.Checked;

            SQLOption.Ignore.FilterSecurity = chkCompSecurity.Checked;
            SQLOption.Ignore.FilterTableFileGroup = chkFileGroups.Checked;
            SQLOption.Ignore.FilterUsers = chkCompUsers.Checked;
            SQLOption.Ignore.FilterRoles = chkCompRoles.Checked;
            SQLOption.Ignore.FilterRules = chkCompRules.Checked;
            SQLOption.Ignore.FilterFullText = chkFullText.Checked;
            SQLOption.Ignore.FilterFullTextPath = chkFullTextPath.Checked;
            SQLOption.Ignore.FilterPermissions = chkCompPermissions.Checked;
            SQLOption.Ignore.FilterSchema = chkCompSchemas.Checked;

            SQLOption.Ignore.FilterNotForReplication = chkIgnoreNotForReplication.Checked;
            SQLOption.Script.AlterObjectOnSchemaBinding = optScriptSchemaBindingAlter.Checked;

            if (rdoCaseAutomatic.Checked)
                SQLOption.Comparison.CaseSensityType = SqlOptionComparison.CaseSensityOptions.Automatic;
            if (rdoCaseInsensitive.Checked)
                SQLOption.Comparison.CaseSensityType = SqlOptionComparison.CaseSensityOptions.CaseInsensity;
            if (rdoCaseSensitive.Checked)
                SQLOption.Comparison.CaseSensityType = SqlOptionComparison.CaseSensityOptions.CaseSensity;

            if (rdoCaseInsensityInCode.Checked)
                SQLOption.Comparison.CaseSensityInCode = SqlOptionComparison.CaseSensityOptions.CaseInsensity;
            if (rdoCaseSensityInCode.Checked)
                SQLOption.Comparison.CaseSensityInCode = SqlOptionComparison.CaseSensityOptions.CaseSensity;

            SQLOption.Comparison.IgnoreWhiteSpacesInCode = chkIgnoreWhiteSpaceInCode.Checked;
            SQLOption.Comparison.ReloadComparisonOnUpdate = chkReloadDB.Checked;

            FireOptionChanged(SQLOption);
        }
        public Abstractions.Schema.Model.IOption GetOption()
        {
            return SQLOption;
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
                AddExclusionPatternForm itemForm = new AddExclusionPatternForm(SQLOption, lstFilters.SelectedItems[0].Index);
                itemForm.ShowDialog(this);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddExclusionPatternForm itemForm = new AddExclusionPatternForm(SQLOption);
            itemForm.ShowDialog(this);
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
            chkCompCLRAggregates.Enabled = chkCompAssemblys.Checked;
        }

        private void DeleteNameFilterButton_Click(object sender, EventArgs e)
        {
            if (lstFilters.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstFilters.Items)
                {
                    if (item.Selected)
                    {
                        var type = (ObjectType)Enum.Parse(typeof(ObjectType), item.SubItems[1].Text);
                        var fi = new SqlOptionFilterItem(type, item.Text);
                        if (SQLOption.Filters.Items.Contains(fi))
                            SQLOption.Filters.Items.Remove(fi);
                    }
                }
                LoadFilters();
            }
        }

        private void lstFilters_DoubleClick(object sender, EventArgs e)
        {
            if (lstFilters.SelectedItems.Count > 0)
            {
                AddExclusionPatternForm itemForm = new AddExclusionPatternForm(SQLOption, lstFilters.SelectedItems[0].Index);
                itemForm.ShowDialog(this);
            }
        }

        private void chkCompProgrammability_CheckedChanged(object sender, EventArgs e)
        {
            chkCompTriggersDDL.Checked = chkCompProgrammability.Checked;
            chkCompFunctions.Checked = chkCompProgrammability.Checked;
            chkPartitionFunction.Checked = chkCompProgrammability.Checked;
            chkPartitionSchemas.Checked = chkCompProgrammability.Checked;
            chkCompRules.Checked = chkCompProgrammability.Checked;
            chkCompStoredProcedure.Checked = chkCompProgrammability.Checked;
            chkCompSynonyms.Checked = chkCompProgrammability.Checked;
            chkCompTriggers.Checked = chkCompProgrammability.Checked;
            chkCompUDT.Checked = chkCompProgrammability.Checked;
            chkCompViews.Checked = chkCompProgrammability.Checked;
            chkCompXMLSchemas.Checked = chkCompProgrammability.Checked;
        }

        private void chkCompSecurity_CheckedChanged(object sender, EventArgs e)
        {
            chkFileGroups.Checked = chkCompSecurity.Checked;
            chkFullText.Checked = chkCompSecurity.Checked;
            chkFullTextPath.Checked = chkCompSecurity.Checked;
            chkCompUsers.Checked = chkCompSecurity.Checked;
            chkCompRoles.Checked = chkCompSecurity.Checked;
            chkCompSchemas.Checked = chkCompSecurity.Checked;
            chkCompPermissions.Checked = chkCompSecurity.Checked;
        }
    }
}
