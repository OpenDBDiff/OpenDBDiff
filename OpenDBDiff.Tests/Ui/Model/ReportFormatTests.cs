using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// The reports folder of section 11.2: the folder name, the two files, the retention rule and the
    /// two report formats of section 6.9.
    /// </summary>
    [TestClass]
    public class ReportFormatTests
    {
        [TestMethod]
        public void TheFolderIsTheTimestampAndTheDatabase()
        {
            Assert.AreEqual("20260315-142530-VivaLDI",
                ReportPaths.FolderName(new DateTime(2026, 3, 15, 14, 25, 30), "VivaLDI"));
        }

        [TestMethod]
        public void ADatabaseNameThatIsNotAPathSegmentIsMadeIntoOne()
        {
            var name = ReportPaths.FolderName(new DateTime(2026, 3, 15, 14, 25, 30), "Viva/LDI:2");

            Assert.AreEqual("20260315-142530-Viva_LDI_2", name);
        }

        [TestMethod]
        public void AnEmptyDatabaseNameLeavesJustTheTimestamp()
        {
            Assert.AreEqual("20260315-142530", ReportPaths.FolderName(new DateTime(2026, 3, 15, 14, 25, 30), null));
            Assert.AreEqual(string.Empty, ReportPaths.SafeName("   "));
        }

        [TestMethod]
        public void ALongNameIsCutToSomethingAFileSystemAccepts()
        {
            var name = ReportPaths.SafeName(new string('d', 200));

            Assert.AreEqual(60, name.Length);
        }

        [TestMethod]
        public void TheRootIsUnderApplicationData()
        {
            var root = ReportPaths.Root(@"C:\Users\f\AppData\Roaming");

            Assert.IsTrue(root.EndsWith(System.IO.Path.Combine("OpenDBDiff", "Reports")), root);
        }

        [TestMethod]
        public void OnlyFoldersThatAreBothOldAndBeyondTheCountAreExpired()
        {
            // Section 11.2: "90 days or 200 entries, whichever is greater".
            var now = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc);
            var folders = new List<ReportFolderEntry>();

            // 250 folders, each a day apart, so 200 are inside the count and the rest are the oldest.
            for (var i = 0; i < 250; i++)
            {
                folders.Add(new ReportFolderEntry("f" + i, @"C:\r\f" + i, now.AddDays(-i)));
            }

            var expired = ReportRetentionRules.Expired(folders, now);
            var kept = ReportRetentionRules.Kept(folders, now);

            Assert.AreEqual(250, expired.Count + kept.Count);
            Assert.AreEqual(250 - ReportRetentionRules.Entries, expired.Count);
            Assert.IsTrue(expired.All(folder => folder.CreatedUtc < now.AddDays(-ReportRetentionRules.Days)));
            Assert.IsTrue(kept.Any(folder => folder.CreatedUtc < now.AddDays(-ReportRetentionRules.Days)),
                "an old folder inside the newest two hundred is still kept");
        }

        [TestMethod]
        public void RecentFoldersSurviveTheCount()
        {
            var now = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc);
            var folders = Enumerable.Range(0, 5)
                .Select(i => new ReportFolderEntry("f" + i, "p" + i, now.AddDays(-i)))
                .ToList();

            Assert.AreEqual(0, ReportRetentionRules.Expired(folders, now).Count);
            Assert.AreEqual(5, ReportRetentionRules.Kept(folders, now).Count);
        }

        [TestMethod]
        public void TheCsvCarriesTheTargetAndOneRowPerItem()
        {
            var csv = ReportFormat.ToCsv(Report());
            var lines = csv.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Assert.AreEqual(4, lines.Length, "header, target, and two items");
            Assert.AreEqual("Object,Action,Result,Duration,Message", lines[0]);
            Assert.IsTrue(lines[1].Contains("VivaLDI"), lines[1]);
            Assert.IsTrue(lines[2].Contains("dbo.A"), lines[2]);
            Assert.IsTrue(lines[3].Contains("dbo.B"), lines[3]);
        }

        [TestMethod]
        public void ACsvFieldWithACommaOrAQuoteIsEscaped()
        {
            Assert.AreEqual("plain", ReportFormat.Escape("plain"));
            Assert.AreEqual("\"a,b\"", ReportFormat.Escape("a,b"));
            Assert.AreEqual("\"say \"\"hi\"\"\"", ReportFormat.Escape("say \"hi\""));
            Assert.AreEqual(string.Empty, ReportFormat.Escape(null));
        }

        [TestMethod]
        public void TheHtmlNamesTheTargetTheOutcomeAndTheRows()
        {
            var html = ReportFormat.ToHtml(Report());

            Assert.IsTrue(html.StartsWith("<!DOCTYPE html>"), "a browser should read it as HTML");
            Assert.IsTrue(html.Contains("srv012023"));
            Assert.IsTrue(html.Contains("VivaLDI"));
            Assert.IsTrue(html.Contains("PROD"));
            Assert.IsTrue(html.Contains("Applied 1 of 2"));
            Assert.IsTrue(html.Contains("dbo.A"));
            Assert.IsTrue(html.Contains("dbo.B"));
            Assert.IsTrue(html.Contains("<code>script.sql</code>"));
        }

        [TestMethod]
        public void TheHtmlEscapesWhatItDidNotWrite()
        {
            var report = Report();
            report.Items.Add(new ApplyItemResult(
                new DiffItemKey(ObjectType.Table, "dbo", "a<b>&c"), DiffStatus.Create, ApplyItemOutcome.Failed, 1)
            {
                ErrorMessage = "<script>alert(1)</script>"
            });

            var html = ReportFormat.ToHtml(report);

            Assert.IsFalse(html.Contains("<script>alert"), "an object name must not be able to inject markup");
            Assert.IsTrue(html.Contains("&lt;script&gt;"));
        }

        private static ApplyReport Report()
        {
            var report = new ApplyReport(
                new ApplyTarget("srv012023", "VivaLDI", EnvironmentTag.For(EnvironmentKind.Prod)),
                usedTransaction: false,
                startedAt: new DateTime(2026, 3, 15, 14, 25, 30, DateTimeKind.Utc),
                endedAt: new DateTime(2026, 3, 15, 14, 25, 31, DateTimeKind.Utc));

            report.Items.Add(new ApplyItemResult(Key("dbo.A"), DiffStatus.Alter, ApplyItemOutcome.Succeeded, 12));
            report.Items.Add(new ApplyItemResult(Key("dbo.B"), DiffStatus.Create, ApplyItemOutcome.Failed, 30));
            report.Script = "print 'x'\r\n";

            return report;
        }

        private static DiffItemKey Key(string name)
        {
            var parts = name.Split('.');

            return new DiffItemKey(ObjectType.Table, parts[0], parts[1]);
        }
    }
}
