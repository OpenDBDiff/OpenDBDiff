using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// Saved connections and recent pairs (section 6.1.6): the caps, the ordering and the rule that the
    /// opposite direction is a different pair.
    /// </summary>
    [TestClass]
    public class ConnectionLibraryTests
    {
        [TestMethod]
        public void AtMostFiftySavedConnectionsAreKept()
        {
            IReadOnlyList<SavedConnection> saved = new List<SavedConnection>();

            for (var i = 0; i < 60; i++)
            {
                saved = ConnectionLibraryRules.Save(saved, Saved("name" + i));
            }

            Assert.AreEqual(50, saved.Count);
            Assert.AreEqual("name59", saved[0].Name, "the newest entry is first");
            Assert.AreEqual("name10", saved[49].Name, "the oldest entries fall off the end");
        }

        [TestMethod]
        public void SavingTheSameNameAgainReplacesItRatherThanDuplicating()
        {
            var saved = ConnectionLibraryRules.Save(new List<SavedConnection>(), Saved("Viva", "srv-old", "VivaLDI"));
            saved = ConnectionLibraryRules.Save(saved, Saved("Viva", "srv-new", "VivaLDI"));

            Assert.AreEqual(1, saved.Count);
            Assert.AreEqual("srv-new", saved[0].Host);
        }

        [TestMethod]
        public void AtMostTenRecentPairsAreKeptMostRecentFirst()
        {
            IReadOnlyList<RecentPair> recent = new List<RecentPair>();

            for (var i = 0; i < 15; i++)
            {
                recent = ConnectionLibraryRules.Record(recent, Pair("srv" + i, "db" + i, "dst" + i, "db" + i, new DateTime(2026, 1, 1).AddMinutes(i)));
            }

            Assert.AreEqual(10, recent.Count);
            Assert.AreEqual("srv14", recent[0].SourceHost);
            Assert.AreEqual($"srv{15 - 10}", recent[9].SourceHost);
        }

        [TestMethod]
        public void RecordingTheSamePairAgainMovesItToTheFrontWithTheNewTime()
        {
            var first = Pair("srv-a", "db", "dst-a", "db", new DateTime(2026, 1, 1));
            var second = Pair("srv-b", "db", "dst-b", "db", new DateTime(2026, 1, 2));

            IReadOnlyList<RecentPair> recent = new List<RecentPair> { first };
            recent = ConnectionLibraryRules.Record(recent, second);
            recent = ConnectionLibraryRules.Record(recent, Pair("srv-a", "db", "dst-a", "db", new DateTime(2026, 1, 3)));

            Assert.AreEqual(2, recent.Count, "the pair is moved, not duplicated");
            Assert.AreEqual("srv-a", recent[0].SourceHost);
            Assert.AreEqual(new DateTime(2026, 1, 3), recent[0].LastRunUtc);
            Assert.AreEqual("srv-b", recent[1].SourceHost);
        }

        [TestMethod]
        public void TheOppositeDirectionIsADifferentPair()
        {
            var forward = Pair("srv-a", "db", "srv-b", "db", new DateTime(2026, 1, 1));
            var backward = Pair("srv-b", "db", "srv-a", "db", new DateTime(2026, 1, 1));

            Assert.IsFalse(ConnectionLibraryRules.IsSamePair(forward, backward));

            var recent = ConnectionLibraryRules.Record(new List<RecentPair> { forward }, backward);
            Assert.AreEqual(2, recent.Count);
        }

        [TestMethod]
        public void TheDirectionNamesBothEndsAndTheLastRunIsRelative()
        {
            var pair = Pair("srv012023", "VivaLDI", "srv012025", "VivaLDI_Prod", new DateTime(2026, 1, 1, 12, 0, 0));

            Assert.AreEqual("srv012023 / VivaLDI \u2192 srv012025 / VivaLDI_Prod", pair.Direction);

            var now = new DateTime(2026, 1, 1, 12, 0, 0);
            Assert.AreEqual("just now", pair.LastRunText(now));
            Assert.AreEqual("5 minutes ago", pair.LastRunText(now.AddMinutes(5)));
            Assert.AreEqual("1 hour ago", pair.LastRunText(now.AddHours(1)));
            Assert.AreEqual("3 days ago", pair.LastRunText(now.AddDays(3)));
        }

        [TestMethod]
        public void ASavedConnectionCarriesNoPasswordField()
        {
            // Section 11.3: the password is never stored in the settings or project file. The model
            // enforces that by not having a place to put one.
            var hasSecret = typeof(SavedConnection)
                .GetProperties()
                .Any(property => property.Name.IndexOf("password", StringComparison.OrdinalIgnoreCase) >= 0);

            Assert.IsFalse(hasSecret, "SavedConnection must not be able to hold a password");
        }

        private static SavedConnection Saved(string name, string host = "srv012023", string database = "VivaLDI")
        {
            return new SavedConnection(name, host, database, true, string.Empty, EnvironmentKind.None);
        }

        private static RecentPair Pair(string sourceHost, string sourceDatabase, string destinationHost, string destinationDatabase, DateTime when)
        {
            return new RecentPair(sourceHost, sourceDatabase, destinationHost, destinationDatabase, when);
        }
    }
}
