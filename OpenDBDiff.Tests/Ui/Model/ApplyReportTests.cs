using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// The apply report of sections 6.9 and 8.4: the outcome, the header, the retry set and the
    /// per-row detail a failure has to carry.
    /// </summary>
    [TestClass]
    public class ApplyReportTests
    {
        [TestMethod]
        public void EveryChangeAppliedIsASuccess()
        {
            var report = Report(false, Succeeded("dbo.A"), Succeeded("dbo.B"));

            Assert.AreEqual(ApplyOutcome.Succeeded, report.Outcome);
            Assert.AreEqual("Applied 2 of 2", report.Header);
            Assert.IsFalse(report.CanRetryFailed);
        }

        [TestMethod]
        public void SomeAppliedAndSomeFailedIsPartial()
        {
            var report = Report(false, Succeeded("dbo.A"), Failed("dbo.B"));

            Assert.AreEqual(ApplyOutcome.Partial, report.Outcome);
            Assert.AreEqual("Applied 1 of 2 \u00b7 1 failed", report.Header);
            Assert.IsTrue(report.CanRetryFailed);
            Assert.AreEqual(1, report.FailedKeys.Count);
        }

        [TestMethod]
        public void NothingAppliedIsAFailure()
        {
            var report = Report(false, Failed("dbo.A"), Failed("dbo.B"));

            Assert.AreEqual(ApplyOutcome.Failed, report.Outcome);
            Assert.AreEqual("Applied 0 of 2 \u00b7 2 failed", report.Header);
        }

        [TestMethod]
        public void OneTransactionThatFailedRolledBack()
        {
            // Section 6.9: a single transaction rolls back, so nothing was left behind and there is
            // nothing to retry.
            var report = Report(true, Succeeded("dbo.A"), Failed("dbo.B"));

            Assert.AreEqual(ApplyOutcome.RolledBack, report.Outcome);
            Assert.AreEqual("Rolled back; no changes were made.", report.OutcomeText);
            Assert.IsFalse(report.CanRetryFailed);
        }

        [TestMethod]
        public void ACancelledSingleTransactionRunIsReportedAsRolledBack()
        {
            var report = Report(true, Succeeded("dbo.A"));
            report.WasCancelled = true;

            Assert.AreEqual(ApplyOutcome.RolledBack, report.Outcome);
        }

        [TestMethod]
        public void ACancelledRunWithoutATransactionSaysSo()
        {
            var report = Report(false, Succeeded("dbo.A"));
            report.WasCancelled = true;

            Assert.AreEqual(ApplyOutcome.Cancelled, report.Outcome);
            Assert.AreEqual("The run was cancelled.", report.OutcomeText);
        }

        [TestMethod]
        public void ASkippedItemIsCountedButNotAsAFailure()
        {
            var report = Report(false, Succeeded("dbo.A"), Skipped("dbo.B"));

            Assert.AreEqual(1, report.SkippedCount);
            Assert.AreEqual(ApplyOutcome.Succeeded, report.Outcome);
            Assert.AreEqual("Applied 1 of 2", report.Header);
        }

        [TestMethod]
        public void AFailureCarriesTheServerErrorAndTheStatement()
        {
            var failed = Failed("dbo.B");
            var item = failed;
            item.ErrorNumber = 2714;
            item.ErrorMessage = "There is already an object named 'B' in the database.";
            item.FailedStatement = "CREATE TABLE [dbo].[B] (Id int)";
            item.Line = 3;

            Assert.AreEqual("Error 2714: There is already an object named 'B' in the database.", item.Message);
            Assert.AreEqual("Failed", item.ResultText);
            Assert.AreEqual("CREATE", item.Action);
            Assert.AreEqual("3", item.Line.ToString());
        }

        [TestMethod]
        public void AResultWithNoErrorNumberStillReads()
        {
            var item = Failed("dbo.B");
            item.ErrorMessage = "The connection was closed.";

            Assert.AreEqual("The connection was closed.", item.Message);
        }

        [TestMethod]
        public void ASuccessfulRowShowsItsOutcome()
        {
            var item = Succeeded("dbo.A");

            Assert.AreEqual("Succeeded", item.Message);
            Assert.AreEqual("ALTER", item.Action);
        }

        [TestMethod]
        public void TheDurationIsReadableAtBothScales()
        {
            var quick = new ApplyReport(Target(), false, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 420, DateTimeKind.Utc));
            Assert.AreEqual("420 ms", quick.DurationText);

            var slow = new ApplyReport(Target(), false, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 2, 500, DateTimeKind.Utc));
            Assert.AreEqual("2.5 s", slow.DurationText);
        }

        private static ApplyTarget Target()
        {
            return new ApplyTarget("srv012023", "VivaLDI", EnvironmentTag.For(EnvironmentKind.Prod));
        }

        private static ApplyReport Report(bool usedTransaction, params ApplyItemResult[] items)
        {
            var report = new ApplyReport(Target(), usedTransaction, DateTime.UtcNow, DateTime.UtcNow);

            foreach (var item in items)
            {
                report.Items.Add(item);
            }

            return report;
        }

        private static ApplyItemResult Succeeded(string name)
        {
            return new ApplyItemResult(Key(name), DiffStatus.Alter, ApplyItemOutcome.Succeeded, 12);
        }

        private static ApplyItemResult Failed(string name)
        {
            return new ApplyItemResult(Key(name), DiffStatus.Create, ApplyItemOutcome.Failed, 30);
        }

        private static ApplyItemResult Skipped(string name)
        {
            return new ApplyItemResult(Key(name), DiffStatus.Create, ApplyItemOutcome.Skipped, 0);
        }

        private static DiffItemKey Key(string name)
        {
            var parts = name.Split('.');

            return new DiffItemKey(ObjectType.Table, parts[0], parts[1]);
        }
    }
}
