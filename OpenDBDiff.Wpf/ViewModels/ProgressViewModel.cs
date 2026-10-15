using System;
using System.Globalization;
using OpenDBDiff.Ui.Model;
using OpenDBDiff.Wpf.Infrastructure;
using OpenDBDiff.Wpf.Services;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>What the run is doing (section 8.3).</summary>
    public enum ProgressStage
    {
        Idle,
        ReadingSource,
        ReadingDestination,
        Diffing,
        GeneratingScript,
        Applying
    }

    /// <summary>
    /// One side's progress, as region B's per-side tooltip reports it (section 6.2): "Source: complete",
    /// "Destination: reading Stored Procedures (4 of 7)".
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class DatabaseProgressViewModel : Infrastructure.ObservableObject
    {
        public DatabaseProgressViewModel(string side, string caption)
        {
            Side = side;
            Caption = caption;
            Maximum = 0;
            Message = string.Empty;
        }

        /// <summary>"Source" or "Destination", the words the specification uses for the two sides.</summary>
        public string Side { get; }

        public string Caption { get; }

        public int Value { get; private set; }

        public int Maximum { get; private set; }

        public string Message { get; private set; }

        public bool IsStarted { get; private set; }

        public bool IsComplete { get; private set; }

        public double Percent
        {
            get { return Maximum > 0 ? (100.0 * Value) / Maximum : 0; }
        }

        /// <summary>The half of the progress bar's tooltip that belongs to this side.</summary>
        public string StatusText
        {
            get
            {
                if (!IsStarted)
                {
                    return "waiting";
                }

                if (IsComplete)
                {
                    return "complete";
                }

                if (string.IsNullOrEmpty(Message))
                {
                    return "reading\u2026";
                }

                return Maximum > 0
                    ? string.Format(CultureInfo.CurrentCulture, "reading {0} ({1} of {2})", Message, Value, Maximum)
                    : "reading " + Message;
            }
        }

        public void Report(DatabaseProgress progress)
        {
            IsStarted = true;
            IsComplete = false;

            if (progress.Maximum > 0)
            {
                Maximum = progress.Maximum;
            }

            if (progress.Value > -1)
            {
                Value = progress.Value;
            }

            if (!string.IsNullOrEmpty(progress.Message))
            {
                Message = progress.Message;
            }
        }

        public void Complete()
        {
            IsStarted = true;
            IsComplete = true;
            Value = Maximum;

            if (string.IsNullOrEmpty(Message))
            {
                Message = "objects";
            }
        }

        public void Reset()
        {
            Value = 0;
            Maximum = 0;
            Message = string.Empty;
            IsStarted = false;
            IsComplete = false;
        }
    }

    /// <summary>
    /// Region B: the progress bar and the status text beside it (section 6.2).
    /// </summary>
    /// <remarks>
    /// This replaces the modal progress window the previous layout opened (problem P7). It is not a
    /// dialog: the phases of section 4 all happen in the main window, so the bar can be bound to the
    /// same view model that owns the compare and the apply.
    ///
    /// Cancellation is cooperative. <see cref="Cancel"/> only changes what the UI says; the shell
    /// restores the previous stable state itself, because AC-05 allows the engine two seconds and the
    /// engine may not abort a query that is already running.
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public sealed class ProgressViewModel : Infrastructure.ObservableObject
    {
        public ProgressViewModel(string sourceCaption, string destinationCaption)
        {
            Source = new DatabaseProgressViewModel("Source", sourceCaption);
            Destination = new DatabaseProgressViewModel("Destination", destinationCaption);
        }

        public DatabaseProgressViewModel Source { get; }

        public DatabaseProgressViewModel Destination { get; }

        public ProgressStage Stage { get; private set; }

        /// <summary>Section 8.3: null percent means an indeterminate sweep.</summary>
        public double? Percent { get; private set; }

        public string? CurrentObject { get; private set; }

        public bool IsCancelling { get; private set; }

        /// <summary>Section 6.2: the fill is visible in COMPARING and APPLYING, hidden otherwise.</summary>
        public bool IsActive
        {
            get { return Stage != ProgressStage.Idle; }
        }

        /// <summary>No percentage to report: the engine gives a message but no ratio.</summary>
        public bool IsIndeterminate
        {
            get
            {
                if (!IsActive)
                {
                    return false;
                }

                switch (Stage)
                {
                    case ProgressStage.ReadingSource:
                        return Source.Maximum <= 0;
                    case ProgressStage.ReadingDestination:
                        return Destination.Maximum <= 0;
                    default:
                        return Percent == null;
                }
            }
        }

        public bool IsDeterminate
        {
            get { return IsActive && !IsIndeterminate; }
        }

        public double FillPercent
        {
            get
            {
                if (IsIndeterminate)
                {
                    return 0;
                }

                switch (Stage)
                {
                    case ProgressStage.ReadingSource:
                        return Source.Percent;
                    case ProgressStage.ReadingDestination:
                        return Destination.Percent;
                    default:
                        return Percent ?? 0;
                }
            }
        }

        /// <summary>Appendix A: "Reading {side} objects… {percent}%".</summary>
        public string StatusText
        {
            get
            {
                if (IsCancelling)
                {
                    return "Cancelling\u2026";
                }

                switch (Stage)
                {
                    case ProgressStage.ReadingSource:
                        return ReadingText("source", Source);
                    case ProgressStage.ReadingDestination:
                        return ReadingText("destination", Destination);
                    case ProgressStage.Diffing:
                        return "Comparing objects\u2026";
                    case ProgressStage.GeneratingScript:
                        return "Generating the script\u2026";
                    case ProgressStage.Applying:
                        return string.IsNullOrEmpty(CurrentObject)
                            ? "Applying changes\u2026"
                            : "Applying " + CurrentObject;
                    default:
                        return string.Empty;
                }
            }
        }

        /// <summary>Section 6.2: hover the bar or a chip and get both sides at once.</summary>
        public string PerSideTooltip
        {
            get
            {
                return Source.Side + ": " + Source.StatusText + Environment.NewLine
                    + Destination.Side + ": " + Destination.StatusText;
            }
        }

        public void StartReading()
        {
            Reset();
            Stage = ProgressStage.ReadingSource;
            Percent = null;
            Refresh();
        }

        public void ReportSource(DatabaseProgress progress)
        {
            Source.Report(progress);
            Stage = ProgressStage.ReadingSource;
            Refresh();
        }

        public void ReportDestination(DatabaseProgress progress)
        {
            Destination.Report(progress);
            Stage = ProgressStage.ReadingDestination;
            Refresh();
        }

        public void BeginDiffing()
        {
            Stage = ProgressStage.Diffing;
            Percent = null;
            Refresh();
        }

        public void ReportApplying(string currentObject, int done, int total)
        {
            Stage = ProgressStage.Applying;
            CurrentObject = currentObject;
            Percent = total > 0 ? (100.0 * done) / total : (double?)null;
            Refresh();
        }

        /// <summary>The run finished; the fill hides and the 3 px track stays (section 6.2).</summary>
        public void Complete()
        {
            Source.Complete();
            Destination.Complete();
            Stage = ProgressStage.Idle;
            IsCancelling = false;
            CurrentObject = null;
            Percent = null;
            Refresh();
        }

        /// <summary>The user asked to stop: the UI says so immediately (section 6.2).</summary>
        public void Cancel()
        {
            IsCancelling = true;
            Refresh();
        }

        public void Reset()
        {
            Source.Reset();
            Destination.Reset();
            Stage = ProgressStage.Idle;
            IsCancelling = false;
            CurrentObject = null;
            Percent = null;
            Refresh();
        }

        private static string ReadingText(string side, DatabaseProgressViewModel progress)
        {
            var percent = progress.Maximum > 0
                ? string.Format(CultureInfo.CurrentCulture, " {0}%", (int)Math.Round(progress.Percent))
                : string.Empty;

            return "Reading " + side + " objects\u2026" + percent;
        }

        private void Refresh()
        {
            OnPropertyChanged(nameof(Stage));
            OnPropertyChanged(nameof(Percent));
            OnPropertyChanged(nameof(CurrentObject));
            OnPropertyChanged(nameof(IsCancelling));
            OnPropertyChanged(nameof(IsActive));
            OnPropertyChanged(nameof(IsIndeterminate));
            OnPropertyChanged(nameof(IsDeterminate));
            OnPropertyChanged(nameof(FillPercent));
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(PerSideTooltip));
        }
    }
}
