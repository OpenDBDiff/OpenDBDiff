using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Data.SqlClient;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.SqlServer.Driver
{
    /// <summary>What an apply is asked to do (section 14's <c>ApplyRequest</c>, with the option set of section 6.8).</summary>
    public sealed class ApplyRequest
    {
        public ApplyRequest(IEnumerable<DiffItemKey> included, bool useTransaction, bool stopOnFirstError)
        {
            var keys = new List<DiffItemKey>();

            if (included != null)
            {
                foreach (var key in included)
                {
                    keys.Add(key);
                }
            }

            Included = keys;
            UseTransaction = useTransaction;
            StopOnFirstError = stopOnFirstError;
        }

        public IReadOnlyList<DiffItemKey> Included { get; }

        /// <summary>Section 6.8's "Run in a single transaction", off when a statement forbids it.</summary>
        public bool UseTransaction { get; }

        /// <summary>Section 6.8's "Stop on first error".</summary>
        public bool StopOnFirstError { get; }
    }

    /// <summary>How far an apply has got, for the progress bar of section 6.9.</summary>
    public sealed class ApplyProgress
    {
        public ApplyProgress(string objectFullName, int done, int total)
        {
            ObjectFullName = objectFullName;
            Done = done;
            Total = total;
        }

        public string ObjectFullName { get; }

        public int Done { get; }

        public int Total { get; }
    }

    /// <summary>
    /// Runs a reviewed script against the destination and reports what happened, item by item
    /// (sections 6.8, 6.9 and 8.4).
    /// </summary>
    /// <remarks>
    /// It runs the script the confirmation dialog showed rather than regenerating it from the model:
    /// what the user read is what executes, which is the point of reviewing it. For the same reason it
    /// does not ask <see cref="IUpdateConfirmation.ConfirmAlter"/> per object any more - the dialog is
    /// the one confirmation, and a second prompt per table would defeat it.
    ///
    /// Items are executed whole and in the order the script put them, so a failure can be attributed to
    /// an object; cancellation is only observed between items, because a statement already sent cannot
    /// be taken back. In a single transaction that means Cancel rolls back, which is what section 6.9
    /// says the user is told.
    ///
    /// This cannot be exercised against a live server from a Linux build machine. The batching rule it
    /// depends on is <see cref="Updater.splitBatches"/>, which the WinForms front end has used for years.
    /// </remarks>
    public static class SqlServerApplySession
    {
        public static ApplyReport Apply(
            GeneratedScript script,
            IReadOnlyList<DiffItem> items,
            ApplyRequest request,
            ApplyTarget target,
            string connectionString,
            IProgress<ApplyProgress> progress,
            CancellationToken cancellationToken)
        {
            var startedAt = DateTime.UtcNow;
            var report = new ApplyReport(target, request.UseTransaction, startedAt, startedAt)
            {
                Script = script == null ? string.Empty : script.Text
            };

            if (script == null)
            {
                return report;
            }

            var order = OrderOf(script, request.Included);
            var stopping = false;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlTransaction transaction = null;

                try
                {
                    if (request.UseTransaction)
                    {
                        transaction = connection.BeginTransaction();
                    }

                    for (var i = 0; i < order.Count; i++)
                    {
                        var key = order[i];

                        if (cancellationToken.IsCancellationRequested)
                        {
                            report.WasCancelled = true;
                            break;
                        }

                        if (stopping)
                        {
                            report.Items.Add(new ApplyItemResult(key, StatusOf(items, key), ApplyItemOutcome.Skipped, 0));
                            continue;
                        }

                        if (progress != null)
                        {
                            progress.Report(new ApplyProgress(key.FullName, i, order.Count));
                        }

                        string failingBatch = null;
                        var stopwatch = Stopwatch.StartNew();

                        try
                        {
                            foreach (var statement in script.For(key))
                            {
                                foreach (var batch in Updater.splitBatches(statement.Sql))
                                {
                                    failingBatch = batch;
                                    ExecuteBatch(connection, transaction, batch);
                                }
                            }

                            stopwatch.Stop();
                            report.Items.Add(new ApplyItemResult(key, StatusOf(items, key), ApplyItemOutcome.Succeeded, stopwatch.ElapsedMilliseconds));
                        }
                        catch (SqlException ex)
                        {
                            stopwatch.Stop();
                            report.Items.Add(Failure(key, StatusOf(items, key), stopwatch.ElapsedMilliseconds, ex.Number, ex.Message, ex.LineNumber, failingBatch));

                            if (request.StopOnFirstError)
                            {
                                stopping = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            stopwatch.Stop();
                            report.Items.Add(Failure(key, StatusOf(items, key), stopwatch.ElapsedMilliseconds, null, ex.Message, null, failingBatch));

                            if (request.StopOnFirstError)
                            {
                                stopping = true;
                            }
                        }
                    }

                    var rollBack = request.UseTransaction && (report.FailedCount > 0 || report.WasCancelled);

                    if (transaction != null)
                    {
                        if (rollBack)
                        {
                            transaction.Rollback();
                        }
                        else
                        {
                            transaction.Commit();
                        }
                    }
                }
                finally
                {
                    if (transaction != null)
                    {
                        transaction.Dispose();
                    }
                }
            }

            return report;
        }

        /// <summary>
        /// The items in the order the script put them: an item is executed where its first statement
        /// appears, so the dependency order the generator chose is the order that runs. An included item
        /// the script produced nothing for still appears, so the report shows it was considered.
        /// </summary>
        public static IReadOnlyList<DiffItemKey> OrderOf(GeneratedScript script, IReadOnlyList<DiffItemKey> included)
        {
            var order = new List<DiffItemKey>();

            if (included == null || included.Count == 0)
            {
                return order;
            }

            var seen = new HashSet<DiffItemKey>();

            if (script != null)
            {
                foreach (var statement in script.Statements)
                {
                    foreach (var key in included)
                    {
                        if (seen.Contains(key) || !string.Equals(statement.ObjectFullName, key.FullName, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        seen.Add(key);
                        order.Add(key);
                    }
                }
            }

            foreach (var key in included)
            {
                if (seen.Add(key))
                {
                    order.Add(key);
                }
            }

            return order;
        }

        private static DiffStatus StatusOf(IReadOnlyList<DiffItem> items, DiffItemKey key)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item.Key.Equals(key))
                    {
                        return item.Status;
                    }
                }
            }

            return DiffStatus.Alter;
        }

        private static ApplyItemResult Failure(DiffItemKey key, DiffStatus status, long durationMs, int? number, string message, int? line, string statement)
        {
            return new ApplyItemResult(key, status, ApplyItemOutcome.Failed, durationMs)
            {
                ErrorNumber = number,
                ErrorMessage = message,
                FailedStatement = statement,
                Line = line
            };
        }

        private static void ExecuteBatch(SqlConnection connection, SqlTransaction transaction, string batch)
        {
            using (var command = new SqlCommand(batch, connection))
            {
                if (transaction != null)
                {
                    command.Transaction = transaction;
                }

                command.ExecuteNonQuery();
            }
        }
    }
}
