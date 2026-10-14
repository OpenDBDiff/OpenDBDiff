using System;
using System.Collections.Generic;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    public enum DiffLineKind
    {
        Unchanged,
        Inserted,
        Deleted,
        ModifiedOld,
        ModifiedNew
    }

    /// <summary>One rendered line of the diff.</summary>
    public sealed class DiffLineViewModel
    {
        public DiffLineViewModel(DiffLineKind kind, string prefix, string? text)
        {
            Kind = kind;
            Prefix = prefix;
            Text = text ?? string.Empty;
        }

        public DiffLineKind Kind { get; }

        /// <summary>"  ", "- ", "* " or "+ ", as the WinForms front end prefixed its lines.</summary>
        public string Prefix { get; }

        public string Text { get; }

        public string Display
        {
            get { return Prefix + Text; }
        }
    }

    /// <summary>
    /// The New object / Old object / Diff panes for the object selected in the tree.
    /// </summary>
    /// <remarks>
    /// The new text comes from the compared database (the left-hand panel, which the comparer marked
    /// up), the old text from the clone of the origin, and the interleaved diff is produced by the
    /// same DiffPlex call the WinForms front end used, with the same prefixes and the same
    /// line-background meanings. The one difference is how it is drawn: lines are rendered by WPF
    /// rather than painted as Scintilla markers, which is why this pane is a list and not an editor.
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public sealed class DiffViewModel
    {
        private readonly IDiffer differ = new Differ();

        public string? ObjectFullName { get; private set; }

        public string NewText { get; private set; } = string.Empty;

        public string OldText { get; private set; } = string.Empty;

        public IReadOnlyList<DiffLineViewModel> Lines { get; private set; } = new List<DiffLineViewModel>();

        /// <summary>True when the selected object differs, which is what enables "Update selected".</summary>
        public bool CanUpdate { get; private set; }

        /// <summary>True when the selected object is a table, which is what enables "Compare data".</summary>
        public bool CanCompareData { get; private set; }

        public bool HasObject
        {
            get { return ObjectFullName != null; }
        }

        public void Clear()
        {
            ObjectFullName = null;
            NewText = string.Empty;
            OldText = string.Empty;
            Lines = new List<DiffLineViewModel>();
            CanUpdate = false;
            CanCompareData = false;
        }

        /// <summary>
        /// Shows one object. <paramref name="compared"/> is the database the comparison produced (the
        /// new state) and <paramref name="origin"/> the clone of the source (the old state).
        /// </summary>
        public void Show(IDatabase compared, IDatabase origin, string? objectFullName)
        {
            Clear();

            if (string.IsNullOrEmpty(objectFullName))
            {
                return;
            }

            ObjectFullName = objectFullName;

            var newItem = compared.Find(objectFullName);

            if (newItem != null && newItem.Status != ObjectStatus.Drop)
            {
                NewText = newItem.ToSql();
                CanUpdate = newItem.Status != ObjectStatus.Original;
                CanCompareData = newItem.ObjectType == ObjectType.Table;
            }

            var oldItem = origin.Find(objectFullName);

            if (oldItem != null && oldItem.Status != ObjectStatus.Create)
            {
                OldText = oldItem.ToSql();
            }

            Lines = BuildLines(OldText, NewText);
        }

        private IReadOnlyList<DiffLineViewModel> BuildLines(string oldText, string newText)
        {
            var model = new SideBySideDiffBuilder(differ).BuildDiffModel(oldText, newText);
            var lines = new List<DiffLineViewModel>();
            var count = Math.Max(model.NewText.Lines.Count, model.OldText.Lines.Count);

            for (var i = 0; i < count; i++)
            {
                var newLine = i < model.NewText.Lines.Count ? model.NewText.Lines[i] : null;
                var oldLine = i < model.OldText.Lines.Count ? model.OldText.Lines[i] : null;

                // The WinForms front end assumed the old side was never shorter than the new one and
                // dereferenced it unconditionally; a shorter old side is guarded here instead of
                // throwing.
                var kind = oldLine?.Type ?? newLine?.Type ?? ChangeType.Unchanged;

                switch (kind)
                {
                    case ChangeType.Inserted:
                        lines.Add(new DiffLineViewModel(DiffLineKind.Unchanged, "  ", oldLine?.Text));
                        break;
                    case ChangeType.Deleted:
                        lines.Add(new DiffLineViewModel(DiffLineKind.Deleted, "- ", oldLine?.Text));
                        break;
                    case ChangeType.Modified:
                        lines.Add(new DiffLineViewModel(DiffLineKind.ModifiedNew, "* ", newLine?.Text));
                        lines.Add(new DiffLineViewModel(DiffLineKind.ModifiedOld, "* ", oldLine?.Text));
                        break;
                    case ChangeType.Imaginary:
                        lines.Add(new DiffLineViewModel(DiffLineKind.Inserted, "+ ", newLine?.Text));
                        break;
                    default:
                        lines.Add(new DiffLineViewModel(DiffLineKind.Unchanged, "  ", oldLine?.Text ?? newLine?.Text));
                        break;
                }
            }

            return lines;
        }
    }
}
