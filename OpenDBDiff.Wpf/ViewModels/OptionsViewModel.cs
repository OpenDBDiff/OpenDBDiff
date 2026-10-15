using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.SqlServer.Schema.Options;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// One object-type switch from the ignore list, e.g. "Ignore tables".
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class IgnoreFilterViewModel
    {
        private readonly PropertyInfo property;
        private readonly object target;
        private readonly Func<bool> read;
        private readonly Action? changed;

        public IgnoreFilterViewModel(string caption, PropertyInfo property, object target, Action? changed = null)
        {
            Caption = caption;
            this.property = property;
            this.target = target;
            this.changed = changed;
            read = () => (bool)(property.GetValue(target, null) ?? false);
        }

        public string Caption { get; }

        /// <summary>
        /// Reads and writes the option object directly, so the WPF window and the WinForms options
        /// dialog edit the same settings rather than two copies of them.
        /// </summary>
        public bool IsIgnored
        {
            get { return read(); }
            set
            {
                property.SetValue(target, value, null);

                if (changed != null)
                {
                    changed();
                }
            }
        }
    }

    /// <summary>
    /// One exclusion pattern, as the options dialog lists it.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class ExclusionPatternViewModel
    {
        private readonly SqlOptionFilterItem item;

        public ExclusionPatternViewModel(SqlOptionFilterItem item)
        {
            this.item = item;
        }

        public SqlOptionFilterItem Item
        {
            get { return item; }
        }

        public string Pattern
        {
            get { return item.FilterPattern; }
            set { item.FilterPattern = value; }
        }

        public ObjectType ObjectType
        {
            get { return item.ObjectType; }
            set { item.ObjectType = value; }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", item.ObjectType, item.FilterPattern);
        }
    }

    /// <summary>
    /// The comparison options dialog: what counts as a difference, which object types to leave alone,
    /// and which objects to exclude by name.
    /// </summary>
    /// <remarks>
    /// The ignore switches are discovered by reflection rather than hard-coded, so this window lists
    /// whatever the option object offers and a new filter appears without changing this class. The
    /// caption is derived from the property name.
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public sealed class OptionsViewModel : DialogViewModelBase
    {
        private readonly SqlOption option;

        public OptionsViewModel(SqlOption option)
        {
            this.option = option;

            IgnoreFilters = typeof(SqlOptionIgnore)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.PropertyType == typeof(bool))
                .Select(property => new IgnoreFilterViewModel(CaptionOf(property.Name), property, option.Ignore, OnChanged))
                .ToList();

            ExclusionPatterns = option.Filters.Items
                .Select(item => new ExclusionPatternViewModel(item))
                .ToList();
        }

        /// <summary>
        /// Raised when a setting that affects the comparison changed, so the shell can mark the visible
        /// results stale (section 9.6). Options that only affect presentation are not wired here.
        /// </summary>
        public event EventHandler? Changed;

        private void OnChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private void OnIgnoreWhiteSpacesInCodeChanged()
        {
            OnChanged();
        }

        private void OnCaseSensityInCodeChanged()
        {
            OnChanged();
        }

        private void OnCaseSensitiveInCodeChanged()
        {
            OnChanged();
        }

        public SqlOption Option
        {
            get { return option; }
        }

        public IReadOnlyList<IgnoreFilterViewModel> IgnoreFilters { get; }

        public IReadOnlyList<ExclusionPatternViewModel> ExclusionPatterns { get; }

        public bool IgnoreWhiteSpacesInCode
        {
            get { return option.Comparison.IgnoreWhiteSpacesInCode; }
            set { option.Comparison.IgnoreWhiteSpacesInCode = value; }
        }

        public bool ReloadComparisonOnUpdate
        {
            get { return option.Comparison.ReloadComparisonOnUpdate; }
            set { option.Comparison.ReloadComparisonOnUpdate = value; }
        }

        public bool CaseSensitiveInCode
        {
            get { return option.Comparison.CaseSensityInCode == SqlOptionComparison.CaseSensityOptions.CaseSensity; }
            set
            {
                option.Comparison.CaseSensityInCode = value
                    ? SqlOptionComparison.CaseSensityOptions.CaseSensity
                    : SqlOptionComparison.CaseSensityOptions.CaseInsensity;
            }
        }

        public int CaseSensityInCode
        {
            get { return (int)option.Comparison.CaseSensityInCode; }
            set { option.Comparison.CaseSensityInCode = (SqlOptionComparison.CaseSensityOptions)value; }
        }

        public void Close()
        {
            RequestClose();
        }

        /// <summary>"FilterUserDataType" and "IgnoreWhiteSpacesInCode" become readable captions.</summary>
        private static string CaptionOf(string propertyName)
        {
            var name = propertyName.StartsWith("Filter", StringComparison.Ordinal)
                ? propertyName.Substring("Filter".Length)
                : propertyName;

            return System.Text.RegularExpressions.Regex.Replace(name, "(?<=[a-z0-9])(?=[A-Z])", " ");
        }
    }
}
