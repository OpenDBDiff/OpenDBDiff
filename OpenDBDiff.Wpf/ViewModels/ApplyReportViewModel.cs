using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using OpenDBDiff.Ui.Model;
using OpenDBDiff.Wpf.Infrastructure;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>One row of the report table (section 6.9).</summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class ApplyReportRowViewModel
    {
        public ApplyReportRowViewModel(ApplyItemResult result)
        {
            Result = result;
        }

        public ApplyItemResult Result { get; }

        public string Object
        {
            get { return Result.Key.FullName; }
        }

        public string Action
        {
            get { return Result.Action; }
        }

        public string ResultText
        {
            get { return Result.ResultText; }
        }

        public string Duration
        {
            get { return Result.DurationText; }
        }

        public string Message
        {
            get { return Result.Message; }
        }

        public bool IsFailed
        {
            get { return Result.Outcome == ApplyItemOutcome.Failed; }
        }

        /// <summary>Section 6.9: failed rows expand to show the number, message, line and statement.</summary>
        public bool HasDetails
        {
            get { return Result.Outcome == ApplyItemOutcome.Failed; }
        }

        public bool IsExpanded { get; set; }

        /// <summary>The detail lines a failed row shows when it is expanded.</summary>
        public IReadOnlyList<string> Details
        {
            get
            {
                var lines = new List<string>();

                if (!HasDetails)
                {
                    return lines;
                }

                if (Result.ErrorNumber.HasValue)
                {
                    lines.Add("Error number: " + Result.ErrorNumber.Value.ToString(CultureInfo.InvariantCulture));
                }

                if (!string.IsNullOrEmpty(Result.ErrorMessage))
                {
                    lines.Add("Message: " + Result.ErrorMessage);
                }

                if (Result.Line.HasValue)
                {
                    lines.Add("Line: " + Result.Line.Value.ToString(CultureInfo.InvariantCulture));
                }

                if (!string.IsNullOrEmpty(Result.FailedStatement))
                {
                    lines.Add("Statement: " + Result.FailedStatement);
                }

                return lines;
            }
        }

        public bool HasDetailLines
        {
            get { return Details.Count > 0; }
        }
    }

    /// <summary>
    /// The REPORT state of section 6.9: what an apply did, with the rows a reader needs to act on.
    /// </summary>
    /// <remarks>
    /// Data only. The buttons that copy, save, retry and go back are the shell's, because they need the
    /// clipboard, the file dialogs and the list, and the shell already owns those.
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public sealed class ApplyReportViewModel : Infrastructure.ObservableObject
    {
        public ApplyReportViewModel(ApplyReport report, string reportFolder)
        {
            Report = report;
            ReportFolder = reportFolder;

            var rows = new List<ApplyReportRowViewModel>();

            foreach (var item in report.Items)
            {
                rows.Add(new ApplyReportRowViewModel(item));
            }

            Rows = rows;
        }

        public ApplyReport Report { get; }

        /// <summary>Where this run's report and script were written (section 11.2).</summary>
        public string ReportFolder { get; }

        public string Header
        {
            get { return Report.Header; }
        }

        public string OutcomeText
        {
            get { return Report.OutcomeText; }
        }

        /// <summary>Green, amber or red, as section 6.9's banner is.</summary>
        public ApplyOutcome Outcome
        {
            get { return Report.Outcome; }
        }

        public bool IsSuccess
        {
            get { return Report.Outcome == ApplyOutcome.Succeeded; }
        }

        public bool IsPartial
        {
            get { return Report.Outcome == ApplyOutcome.Partial; }
        }

        public bool IsBad
        {
            get { return Report.Outcome == ApplyOutcome.RolledBack || Report.Outcome == ApplyOutcome.Failed; }
        }

        public string TargetText
        {
            get
            {
                var builder = new StringBuilder();

                builder.Append(Report.Target.Server).Append(" / ").Append(Report.Target.Database);

                if (Report.Target.Environment.HasValue)
                {
                    builder.Append("  \u00b7  ").Append(Report.Target.Environment.Text);
                }

                if (Report.UsedTransaction)
                {
                    builder.Append("  \u00b7  one transaction");
                }

                builder.Append("  \u00b7  ").Append(Report.DurationText);

                return builder.ToString();
            }
        }

        public IReadOnlyList<ApplyReportRowViewModel> Rows { get; }

        public bool CanRetryFailed
        {
            get { return Report.CanRetryFailed; }
        }

        public string RetryFailedLabel
        {
            get { return "Retry failed (" + Report.FailedCount + ")"; }
        }

        public string FolderText
        {
            get { return string.IsNullOrEmpty(ReportFolder) ? string.Empty : "Saved to " + ReportFolder; }
        }
    }
}
