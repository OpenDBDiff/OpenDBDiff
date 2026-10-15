using System;
using System.Collections.Generic;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>One recorded inclusion state, with the change that produced it.</summary>
    public sealed class InclusionStep
    {
        public InclusionStep(string label, IReadOnlyList<DiffItemKey> included)
        {
            Label = label;
            Included = included ?? new List<DiffItemKey>();
        }

        /// <summary>What produced this state, e.g. "Select dbo.Users" or "Exclude all of type Table".</summary>
        public string Label { get; }

        public IReadOnlyList<DiffItemKey> Included { get; }
    }

    /// <summary>
    /// The undo stack behind section 9.8: selection changes are undoable with Ctrl+Z and Ctrl+Y, the
    /// stack holds fifty steps and is reset on each compare.
    /// </summary>
    /// <remarks>
    /// Only inclusion is recorded, because only inclusion is undoable - an apply is not, as the section
    /// says. The first step is the default inclusion a compare produces, so Ctrl+Z after a compare goes
    /// back to the state the compare itself created rather than past it.
    /// </remarks>
    public sealed class InclusionHistory
    {
        public const int MaxSteps = 50;

        private readonly List<InclusionStep> steps = new List<InclusionStep>();
        private int cursor = -1;

        public int Count
        {
            get { return steps.Count; }
        }

        public bool CanUndo
        {
            get { return cursor > 0; }
        }

        public bool CanRedo
        {
            get { return cursor >= 0 && cursor < steps.Count - 1; }
        }

        /// <summary>The change Ctrl+Z would take back, for the menu item and the tooltip.</summary>
        public string UndoLabel
        {
            get { return CanUndo ? steps[cursor].Label : null; }
        }

        public string RedoLabel
        {
            get { return CanRedo ? steps[cursor + 1].Label : null; }
        }

        public IReadOnlyList<string> Labels
        {
            get
            {
                var labels = new List<string>();

                foreach (var step in steps)
                {
                    labels.Add(step.Label);
                }

                return labels;
            }
        }

        /// <summary>Section 9.8: reset on each compare. The baseline is the inclusion that compare produced.</summary>
        public void Reset(string label, IReadOnlyList<DiffItemKey> included)
        {
            steps.Clear();
            cursor = -1;
            Record(label, included);
        }

        public void Clear()
        {
            steps.Clear();
            cursor = -1;
        }

        /// <summary>
        /// Records a new state. Recording after an undo drops the redo tail, which is what a user
        /// expects from a single undo stack rather than two.
        /// </summary>
        public void Record(string label, IReadOnlyList<DiffItemKey> included)
        {
            if (cursor < steps.Count - 1)
            {
                steps.RemoveRange(cursor + 1, steps.Count - cursor - 1);
            }

            steps.Add(new InclusionStep(label, included));
            cursor = steps.Count - 1;

            while (steps.Count > MaxSteps)
            {
                steps.RemoveAt(0);
                cursor--;
            }
        }

        /// <summary>The state to restore, or null when there is nothing to undo.</summary>
        public InclusionStep Undo()
        {
            if (!CanUndo)
            {
                return null;
            }

            cursor--;
            return steps[cursor];
        }

        public InclusionStep Redo()
        {
            if (!CanRedo)
            {
                return null;
            }

            cursor++;
            return steps[cursor];
        }
    }
}
