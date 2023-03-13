using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
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
            chkCompCLRAggregates.Checked = SQLOption.Ignore.FilterCLRAggregate;
            chkCompCLRFunctions.Checked = SQLOption.Ignore.FilterCLRFunction;
            chkCompCLRStore.Checked = SQLOption.Ignore.FilterCLRStoredProcedure;
            chkCompCLRTrigger.Checked = SQLOption.Ignore.FilterCLRTrigger;
            chkCompCLRUDT.Checked = SQLOption.Ignore.FilterCLRUDT;

            chkConstraints.Checked = SQLOption.Ignore.FilterConstraint;
            chkConstraintsPK.Checked = SQLOption.Ignore.FilterConstraintPK;
            chkConstraintsFK.Checked = SQLOption.Ignore.FilterConstraintFK;
            chkConstraintsUK.Checked = SQLOption.Ignore.FilterConstraintUK;
            chkConstraintsCheck.Checked = SQLOption.Ignore.FilterConstraintCheck;

            chkIndex.Checked = SQLOption.Ignore.FilterIndex;
            chkIndexRowLock.Checked = SQLOption.Ignore.FilterIndexRowLock;
            chkIndexFillFactor.Checked = SQLOption.Ignore.FilterIndexFillFactor;
            chkIndexIncludeColumns.Checked = SQLOption.Ignore.FilterIndexIncludeColumns;
            chkIndexFilter.Checked = SQLOption.Ignore.FilterIndexFilter;

            chkCompExtendedProperties.Checked = SQLOption.Ignore.FilterExtendedProperties;
            chkCompFunciones.Checked = SQLOption.Ignore.FilterFunction;
            chkFullText.Checked = SQLOption.Ignore.FilterFullText;
            chkFullTextPath.Checked = SQLOption.Ignore.FilterFullTextPath;

            chkCompSchemas.Checked = SQLOption.Ignore.FilterSchema;
            chkCompPermissions.Checked = SQLOption.Ignore.FilterPermission;
            chkCompStoredProcedure.Checked = SQLOption.Ignore.FilterStoredProcedure;

            chkTables.Checked = SQLOption.Ignore.FilterTable;
            chkTableOption.Checked = SQLOption.Ignore.FilterTableOption;
            chkTablesColumnIdentity.Checked = SQLOption.Ignore.FilterColumnIdentity;
            chkTablesColumnCollation.Checked = SQLOption.Ignore.FilterColumnCollation;
            chkTableLockEscalation.Checked = SQLOption.Ignore.FilterTableLockEscalation;
            chkTableChangeTracking.Checked = SQLOption.Ignore.FilterTableChangeTracking;

            chkTablesColumnOrder.Checked = SQLOption.Ignore.FilterColumnOrder;
            chkIgnoreNotForReplication.Checked = SQLOption.Ignore.FilterNotForReplication;

            chkCompTriggersDDL.Checked = SQLOption.Ignore.FilterDDLTriggers;
            chkCompTriggers.Checked = SQLOption.Ignore.FilterTrigger;
            chkCompUDT.Checked = SQLOption.Ignore.FilterUserDataType;
            chkCompVistas.Checked = SQLOption.Ignore.FilterView;
            chkCompXMLSchemas.Checked = SQLOption.Ignore.FilterXMLSchema;
            chkFileGroups.Checked = SQLOption.Ignore.FilterTableFileGroup;
            chkCompUsers.Checked = SQLOption.Ignore.FilterUsers;
            chkCompRoles.Checked = SQLOption.Ignore.FilterRoles;
            chkCompRules.Checked = SQLOption.Ignore.FilterRules;
            checkPartitionFunction.Checked = SQLOption.Ignore.FilterPartitionFunction;
            checkPartitionSchemas.Checked = SQLOption.Ignore.FilterPartitionScheme;

            IncludeSynonymsCheckBox.Checked = SQLOption.Ignore.FilterSynonyms;

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
            SQLOption.Ignore.FilterCLRAggregate = chkCompCLRAggregates.Checked && chkCompAssemblys.Checked;
            SQLOption.Ignore.FilterCLRFunction = chkCompCLRFunctions.Checked && chkCompAssemblys.Checked;
            SQLOption.Ignore.FilterCLRStoredProcedure = chkCompCLRStore.Checked && chkCompAssemblys.Checked;
            SQLOption.Ignore.FilterCLRTrigger = chkCompCLRTrigger.Checked && chkCompAssemblys.Checked;
            SQLOption.Ignore.FilterCLRUDT = chkCompCLRUDT.Checked && chkCompAssemblys.Checked;

            SQLOption.Ignore.FilterConstraint = chkConstraints.Checked;
            SQLOption.Ignore.FilterConstraintPK = chkConstraintsPK.Checked && chkConstraints.Checked;
            SQLOption.Ignore.FilterConstraintFK = chkConstraintsFK.Checked && chkConstraints.Checked;
            SQLOption.Ignore.FilterConstraintUK = chkConstraintsUK.Checked && chkConstraints.Checked;
            SQLOption.Ignore.FilterConstraintCheck = chkConstraintsCheck.Checked && chkConstraints.Checked;

            SQLOption.Ignore.FilterFunction = chkCompFunciones.Checked;

            SQLOption.Ignore.FilterIndex = chkIndex.Checked;
            SQLOption.Ignore.FilterIndexRowLock = chkIndexRowLock.Checked && chkIndex.Checked;
            SQLOption.Ignore.FilterIndexFillFactor = chkIndexFillFactor.Checked && chkIndex.Checked;
            SQLOption.Ignore.FilterIndexIncludeColumns = chkIndexIncludeColumns.Checked && chkIndex.Checked;
            SQLOption.Ignore.FilterIndexFilter = chkIndexFilter.Checked && chkIndex.Checked;

            SQLOption.Ignore.FilterSchema = chkCompSchemas.Checked;
            SQLOption.Ignore.FilterPermission = chkCompPermissions.Checked;
            SQLOption.Ignore.FilterStoredProcedure = chkCompStoredProcedure.Checked;

            SQLOption.Ignore.FilterTable = chkTables.Checked;
            SQLOption.Ignore.FilterColumnIdentity = chkTablesColumnIdentity.Checked && chkTables.Checked;
            SQLOption.Ignore.FilterColumnCollation = chkTablesColumnCollation.Checked && chkTables.Checked;
            SQLOption.Ignore.FilterColumnOrder = chkTablesColumnOrder.Checked && chkTables.Checked;
            SQLOption.Ignore.FilterTableOption = chkTableOption.Checked && chkTables.Checked;
            SQLOption.Ignore.FilterTableLockEscalation = chkTableLockEscalation.Checked && chkTables.Checked;
            SQLOption.Ignore.FilterTableChangeTracking = chkTableChangeTracking.Checked && chkTables.Checked;

            SQLOption.Ignore.FilterTableFileGroup = chkFileGroups.Checked;
            SQLOption.Ignore.FilterTrigger = chkCompTriggers.Checked;
            SQLOption.Ignore.FilterDDLTriggers = chkCompTriggersDDL.Checked;
            SQLOption.Ignore.FilterUserDataType = chkCompUDT.Checked;
            SQLOption.Ignore.FilterView = chkCompVistas.Checked;
            SQLOption.Ignore.FilterXMLSchema = chkCompXMLSchemas.Checked;
            SQLOption.Ignore.FilterExtendedProperties = chkCompExtendedProperties.Checked;
            SQLOption.Ignore.FilterUsers = chkCompUsers.Checked;
            SQLOption.Ignore.FilterRoles = chkCompRoles.Checked;
            SQLOption.Ignore.FilterRules = chkCompRules.Checked;
            SQLOption.Ignore.FilterFullText = chkFullText.Checked;
            SQLOption.Ignore.FilterFullTextPath = chkFullTextPath.Checked;
            SQLOption.Ignore.FilterSynonyms = IncludeSynonymsCheckBox.Checked;
            SQLOption.Ignore.FilterPartitionFunction = checkPartitionFunction.Checked;
            SQLOption.Ignore.FilterPartitionScheme = checkPartitionSchemas.Checked;

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
            bool ValueParent = chkIndex.Checked;
            chkIndexRowLock.Enabled = ValueParent;
            chkIndexRowLock.Checked = ValueParent;
            chkIndexFillFactor.Enabled = ValueParent;
            chkIndexFillFactor.Checked = ValueParent;
            chkIndexIncludeColumns.Enabled = ValueParent;
            chkIndexIncludeColumns.Checked = ValueParent;
            chkIndexFilter.Enabled = ValueParent;
            chkIndexFilter.Checked = ValueParent;
        }

        private void chkCompTablas_CheckedChanged(object sender, EventArgs e)
        {
            bool ValueParent = chkTables.Checked;
            chkTablesColumnCollation.Enabled = ValueParent;
            chkTablesColumnCollation.Checked = ValueParent;
            chkTablesColumnOrder.Enabled = ValueParent;
            chkTablesColumnOrder.Checked = ValueParent;
            chkTablesColumnIdentity.Enabled = ValueParent;
            chkTablesColumnIdentity.Checked = ValueParent;
            chkTableOption.Enabled = ValueParent;
            chkTableOption.Checked = ValueParent;
            chkTableLockEscalation.Enabled = ValueParent;
            chkTableLockEscalation.Checked = ValueParent;
            chkTableChangeTracking.Enabled = ValueParent;
            chkTableChangeTracking.Checked = ValueParent;
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
            bool ValueParent = chkConstraints.Checked;
            chkConstraintsPK.Enabled = ValueParent;
            chkConstraintsPK.Checked = ValueParent;
            chkConstraintsFK.Enabled = ValueParent;
            chkConstraintsFK.Checked = ValueParent;
            chkConstraintsUK.Enabled = ValueParent;
            chkConstraintsUK.Checked = ValueParent;
            chkConstraintsCheck.Enabled = ValueParent;
            chkConstraintsCheck.Checked = ValueParent;
        }

        private void chkFullText_CheckedChanged(object sender, EventArgs e)
        {
            bool ValueParent = chkFullText.Checked;
            chkFullTextPath.Enabled = ValueParent;
            chkFullTextPath.Checked = ValueParent;
        }

        private void chkCompAssemblys_CheckedChanged(object sender, EventArgs e)
        {
            bool ValueParent = chkCompAssemblys.Checked;
            chkCompCLRAggregates.Enabled = ValueParent;
            chkCompCLRAggregates.Checked = ValueParent;
            chkCompCLRStore.Enabled = ValueParent;
            chkCompCLRStore.Checked = ValueParent;
            chkCompCLRTrigger.Enabled = ValueParent;
            chkCompCLRTrigger.Checked = ValueParent;
            chkCompCLRFunctions.Enabled = ValueParent;
            chkCompCLRFunctions.Checked = ValueParent;
            chkCompCLRUDT.Enabled = ValueParent;
            chkCompCLRUDT.Checked = ValueParent;
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
    }
}
