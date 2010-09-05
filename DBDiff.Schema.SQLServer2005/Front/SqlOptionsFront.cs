using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
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
            
        }

        public void Load(SqlOption valueOption)
        {
            this.option = valueOption;
            txtBlob.Text = option.OptionDefault.DefaultBlobValue;
            txtDate.Text = option.OptionDefault.DefaultDateValue;
            txtDefaultInteger.Text = option.OptionDefault.DefaultIntegerValue;
            txtDefaultReal.Text = option.OptionDefault.DefaultRealValue;
            txtNText.Text = option.OptionDefault.DefaultNTextValue;
            txtText.Text = option.OptionDefault.DefaultTextValue;
            txtVariant.Text = option.OptionDefault.DefaultVariantValue;

            chkCompAssemblys.Checked = option.OptionFilter.FilterAssemblies;
            chkCompConstraints.Checked = option.OptionFilter.FilterConstraint;
            chkCompExtendedProperties.Checked = option.OptionFilter.FilterExtendedPropertys;
            chkCompFunciones.Checked = option.OptionFilter.FilterFunction;
            chkCompIndices.Checked = option.OptionFilter.FilterIndex;
            chkCompSchemas.Checked = option.OptionFilter.FilterSchema;
            chkCompStoreProcedure.Checked = option.OptionFilter.FilterStoreProcedure;
            chkCompTablaOpciones.Checked = option.OptionFilter.FilterTableOption;
            chkCompTablas.Checked = option.OptionFilter.FilterTable;
            chkCompTriggers.Checked = option.OptionFilter.FilterTrigger;
            chkCompUDT.Checked = option.OptionFilter.FilterUserDataType;
            chkCompVistas.Checked = option.OptionFilter.FilterView;
            chkCompXMLSchemas.Checked = option.OptionFilter.FilterXMLSchema;
            chkFileGroups.Checked = option.OptionFilter.FilterTableFileGroup;
        }

        public void Save()
        {
            option.OptionDefault.DefaultBlobValue = txtBlob.Text;
            option.OptionDefault.DefaultDateValue = txtDate.Text;
            option.OptionDefault.DefaultIntegerValue = txtDefaultInteger.Text;
            option.OptionDefault.DefaultNTextValue = txtNText.Text;
            option.OptionDefault.DefaultRealValue = txtDefaultReal.Text;
            option.OptionDefault.DefaultTextValue = txtText.Text;
            option.OptionDefault.DefaultVariantValue = txtVariant.Text;

            option.OptionFilter.FilterAssemblies = chkCompAssemblys.Checked;
            option.OptionFilter.FilterConstraint = chkCompConstraints.Checked;
            option.OptionFilter.FilterFunction = chkCompFunciones.Checked;
            option.OptionFilter.FilterIndex = chkCompIndices.Checked;
            option.OptionFilter.FilterSchema = chkCompSchemas.Checked;
            option.OptionFilter.FilterStoreProcedure = chkCompStoreProcedure.Checked;
            option.OptionFilter.FilterTable = chkCompTablas.Checked;
            option.OptionFilter.FilterTableFileGroup = chkFileGroups.Checked;
            option.OptionFilter.FilterTableOption = chkCompTablaOpciones.Checked;
            option.OptionFilter.FilterTrigger = chkCompTriggers.Checked;
            option.OptionFilter.FilterUserDataType = chkCompUDT.Checked;
            option.OptionFilter.FilterView = chkCompVistas.Checked;
            option.OptionFilter.FilterXMLSchema = chkCompXMLSchemas.Checked;
            option.OptionFilter.FilterExtendedPropertys = chkCompExtendedProperties.Checked;
        }
    }
}
