using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.SqlServer.Driver;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Driver
{
    /// <summary>
    /// The destination stamp of section 9.6's background check: the two columns of one aggregate query,
    /// turned into a stamp that survives a database with nothing in it and a query that fails.
    /// </summary>
    /// <remarks>
    /// The SQL itself cannot be run here - there is no SQL Server on this machine and no chance of one in
    /// the test suite - so what is pinned is the reading of its two columns, which is where a null and a
    /// DBNull would otherwise become an exception in a background thread.
    /// </remarks>
    [TestClass]
    public class SqlServerDestinationWatchTests
    {
        [TestMethod]
        public void TheStampSqlIsOneAggregateQueryOverSysObjects()
        {
            var sql = SqlServerDestinationWatch.StampSql;

            StringAssert.Contains(sql, "modify_date");
            StringAssert.Contains(sql, "COUNT_BIG(*)");
            StringAssert.Contains(sql, "sys.objects");
            StringAssert.Contains(sql, "is_ms_shipped = 0");
            Assert.AreEqual(1, sql.Split(new[] { "SELECT" }, StringSplitOptions.None).Length - 1,
                "one statement, so a background check cannot run more than it was asked to");
        }

        [TestMethod]
        public void TheStampCarriesTheDateAndTheCount()
        {
            var date = new DateTime(2026, 10, 15, 9, 30, 0);

            var stamp = SqlServerDestinationWatch.StampFromReader(Reader(date, 128L));

            Assert.AreEqual(date, stamp.LatestModify);
            Assert.AreEqual(128, stamp.ObjectCount);
        }

        /// <summary>A database with no user objects has no modification date; that is a stamp, not a failure.</summary>
        [TestMethod]
        public void AnEmptyDatabaseHasNoDateAndNoObjects()
        {
            var stamp = SqlServerDestinationWatch.StampFromReader(Reader(DBNull.Value, 0L));

            Assert.IsNull(stamp.LatestModify);
            Assert.AreEqual(0, stamp.ObjectCount);
        }

        /// <summary>
        /// COUNT_BIG returns a bigint, which is wider than the stamp's count. Nothing real comes close, but
        /// this runs on a background thread where an overflow would be an unhandled exception rather than a
        /// wrong answer, so the read clamps.
        /// </summary>
        [TestMethod]
        public void ACountBeyondWhatAnIntHoldsIsClampedRatherThanThrowing()
        {
            var stamp = SqlServerDestinationWatch.StampFromReader(Reader(new DateTime(2026, 1, 1), 3_000_000_000L));

            Assert.AreEqual(int.MaxValue, stamp.ObjectCount);
        }

        [TestMethod]
        public void ReadingNothingGivesNothing()
        {
            Assert.IsNull(SqlServerDestinationWatch.StampFromReader(null));
        }

        [TestMethod]
        public void AnUnreadableDestinationIsNotAChange()
        {
            // Null means "could not read", and the comparison refuses to treat that as a change, so a failed
            // check never reports something it never saw.
            var stamp = new DestinationStamp(DateTime.Now, 10);

            Assert.IsFalse(DestinationStamp.Same(stamp, null));
            Assert.IsFalse(DestinationStamp.Same(null, stamp));
        }

        /// <summary>
        /// A reader positioned on one row, which is the state the stamp is read in: the query asks for one
        /// row and the caller has already read it.
        /// </summary>
        private static IDataRecord Reader(object latestModify, object count)
        {
            var table = new DataTable();

            table.Columns.Add("latest_modify", typeof(DateTime));
            table.Columns.Add("object_count", typeof(long));
            table.Rows.Add(latestModify, count);

            var reader = table.CreateDataReader();
            reader.Read();

            return reader;
        }
    }
}
