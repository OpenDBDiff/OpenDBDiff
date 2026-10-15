using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OpenDBDiff.Abstractions.Schema;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>What happened to one item (section 8.4).</summary>
    public enum ApplyItemOutcome
    {
        Succeeded,
        Failed,

        /// <summary>Not attempted, because an earlier failure stopped the run.</summary>
        Skipped
    }

    /// <summary>How the run as a whole ended (sections 4 and 8.4).</summary>
    public enum ApplyOutcome
    {
        Succeeded,

        /// <summary>Some items applied and some failed.</summary>
        Partial,

        /// <summary>A single transaction, so nothing was left behind.</summary>
        RolledBack,

        Failed,

        Cancelled
    }

    /// <summary>Where an apply was aimed, which the confirmation and the report both name (sections 6.8 and 6.9).</summary>
    public sealed class ApplyTarget
    {
        public ApplyTarget(string server, string database, EnvironmentTag environment)
        {
            Server = server ?? string.Empty;
            Database = database ?? string.Empty;
            Environment = environment ?? EnvironmentTag.None;
        }

        public string Server { get; }

        public string Database { get; }

        public EnvironmentTag Environment { get; }
    }

    /// <summary>One row of the report (section 6.9).</summary>
    public sealed class ApplyItemResult
    {
        public ApplyItemResult(DiffItemKey key, DiffStatus status, ApplyItemOutcome outcome, long durationMs)
        {
            Key = key;
            Status = status;
            Outcome = outcome;
            DurationMs = durationMs;
        }

        public DiffItemKey Key { get; }

        public DiffStatus Status { get; }

        public ApplyItemOutcome Outcome { get; }

        public long DurationMs { get; }

        public int? ErrorNumber { get; set; }

        public string ErrorMessage { get; set; }

        /// <summary>The statement the server rejected, which is what a reader needs to fix it.</summary>
        public string FailedStatement { get; set; }

        public int? Line { get; set; }

        /// <summary>The action in the words the list and the report use, e.g. "ALTER".</summary>
        public string Action
        {
            get { return StatusVocabulary.Badge(Status); }
        }

        public string ResultText
        {
            get
            {
                switch (Outcome)
                {
                    case ApplyItemOutcome.Succeeded: return "Succeeded";
                    case ApplyItemOutcome.Failed: return "Failed";
                    default: return "Skipped";
                }
            }
        }

        public string DurationText
        {
            get { return DurationMs + " ms"; }
        }

        /// <summary>What the result column shows: the error for a failure, the outcome otherwise.</summary>
        public string Message
        {
            get
            {
                if (Outcome != ApplyItemOutcome.Failed)
                {
                    return ResultText;
                }

                var text = string.IsNullOrEmpty(ErrorMessage) ? "The statement failed." : ErrorMessage;

                return ErrorNumber.HasValue
                    ? string.Format("Error {0}: {1}", ErrorNumber.Value, text)
                    : text;
            }
        }
    }

    /// <summary>
    /// The result of an apply (section 8.4), which is what the REPORT state shows and what gets written
    /// to the reports folder (section 11.2).
    /// </summary>
    public sealed class ApplyReport
    {
        public ApplyReport(ApplyTarget target, bool usedTransaction, DateTime startedAt, DateTime endedAt)
        {
            Target = target;
            UsedTransaction = usedTransaction;
            StartedAt = startedAt;
            EndedAt = endedAt;
            Items = new List<ApplyItemResult>();
        }

        public ApplyTarget Target { get; }

        public bool UsedTransaction { get; }

        public DateTime StartedAt { get; }

        public DateTime EndedAt { get; }

        public List<ApplyItemResult> Items { get; }

        public bool WasCancelled { get; set; }

        /// <summary>The script that was run, saved beside the report (section 11.2).</summary>
        public string Script { get; set; } = string.Empty;

        /// <summary>Where the copy of the script was written, when the user asked for one.</summary>
        public string ScriptCopyPath { get; set; }

        /// <summary>Where the report itself was written.</summary>
        public string ReportPath { get; set; }

        public int TotalCount
        {
            get { return Items.Count; }
        }

        public int SucceededCount
        {
            get { return Items.Count(item => item.Outcome == ApplyItemOutcome.Succeeded); }
        }

        public int FailedCount
        {
            get { return Items.Count(item => item.Outcome == ApplyItemOutcome.Failed); }
        }

        public int SkippedCount
        {
            get { return Items.Count(item => item.Outcome == ApplyItemOutcome.Skipped); }
        }

        public ApplyOutcome Outcome
        {
            get { return ApplyReportRules.Outcome(UsedTransaction, WasCancelled, Items); }
        }

        /// <summary>Section 6.9's header, e.g. "Applied 13 of 14 &#183; 1 failed".</summary>
        public string Header
        {
            get { return ApplyReportRules.Header(SucceededCount, TotalCount, FailedCount); }
        }

        public string OutcomeText
        {
            get { return ApplyReportRules.OutcomeText(Outcome); }
        }

        /// <summary>Section 6.9's Retry failed: only when nothing was rolled back and something failed.</summary>
        public bool CanRetryFailed
        {
            get { return FailedCount > 0 && Outcome != ApplyOutcome.RolledBack; }
        }

        public IReadOnlyList<DiffItemKey> FailedKeys
        {
            get { return Items.Where(item => item.Outcome == ApplyItemOutcome.Failed).Select(item => item.Key).ToList(); }
        }

        public string DurationText
        {
            get
            {
                var elapsed = EndedAt - StartedAt;

                return elapsed.TotalSeconds >= 1
                    ? elapsed.TotalSeconds.ToString("0.0", CultureInfo.InvariantCulture) + " s"
                    : elapsed.TotalMilliseconds.ToString("0", CultureInfo.InvariantCulture) + " ms";
            }
        }
    }

    /// <summary>The report's derived facts, so the view and the test agree on them.</summary>
    public static class ApplyReportRules
    {
        /// <summary>
        /// Section 6.9's banner: green when everything succeeded, amber when some did, red when the run
        /// was rolled back or failed outright. A cancelled single-transaction run counts as rolled back,
        /// because that is what the user was told.
        /// </summary>
        public static ApplyOutcome Outcome(bool usedTransaction, bool cancelled, IReadOnlyList<ApplyItemResult> items)
        {
            if (cancelled)
            {
                return usedTransaction ? ApplyOutcome.RolledBack : ApplyOutcome.Cancelled;
            }

            if (items == null || items.Count == 0)
            {
                return ApplyOutcome.Succeeded;
            }

            var failed = items.Count(item => item.Outcome == ApplyItemOutcome.Failed);
            var succeeded = items.Count(item => item.Outcome == ApplyItemOutcome.Succeeded);

            if (failed == 0)
            {
                return ApplyOutcome.Succeeded;
            }

            if (usedTransaction)
            {
                return ApplyOutcome.RolledBack;
            }

            return succeeded == 0 ? ApplyOutcome.Failed : ApplyOutcome.Partial;
        }

        /// <summary>Appendix A's report.partial, and the all-succeeded form of it.</summary>
        public static string Header(int succeeded, int total, int failed)
        {
            return failed == 0
                ? string.Format("Applied {0} of {1}", succeeded, total)
                : string.Format("Applied {0} of {1} \u00b7 {2} failed", succeeded, total, failed);
        }

        public static string OutcomeText(ApplyOutcome outcome)
        {
            switch (outcome)
            {
                case ApplyOutcome.Succeeded:
                    return "Every change was applied.";
                case ApplyOutcome.Partial:
                    return "Some changes could not be applied.";
                case ApplyOutcome.RolledBack:
                    return "Rolled back; no changes were made.";
                case ApplyOutcome.Failed:
                    return "No changes could be applied.";
                default:
                    return "The run was cancelled.";
            }
        }
    }
}
