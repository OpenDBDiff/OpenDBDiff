using System;
using System.Data;
using Microsoft.Data.SqlClient;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.SqlServer.Driver
{
    /// <summary>
    /// Reads the destination's stamp for section 9.6's background check.
    /// </summary>
    /// <remarks>
    /// One aggregate query, because this runs while somebody is reading results and must not cost more than
    /// the answer is worth: two columns over sys.objects, no joins, no per-object rows coming back. The
    /// stamp itself and what it means live in the model; this class only gets the numbers and turns
    /// DBNull into a missing date rather than a crash.
    /// </remarks>
    public static class SqlServerDestinationWatch
    {
        /// <summary>
        /// modify_date moves when an object is created or altered, and the count moves when one is added
        /// or dropped - which between them is every way an object can change. System objects are excluded,
        /// because they change on their own as the server is patched.
        /// </summary>
        public const string StampSql =
            "SELECT MAX(modify_date) AS latest_modify, COUNT_BIG(*) AS object_count " +
            "FROM sys.objects WHERE is_ms_shipped = 0";

        /// <summary>Reads the stamp, or null when the destination cannot be read at all.</summary>
        public static DestinationStamp Read(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(StampSql, connection))
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        return new DestinationStamp(LatestModify(reader), ObjectCount(reader));
                    }
                }
            }
            catch (SqlException)
            {
                // A destination that has gone away is not a change; the compare will say so when it is run.
                return null;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        /// <summary>
        /// The two column values, from a reader already positioned on its row. A database with no user
        /// objects has no modification date to report, which is a stamp rather than a failure.
        /// </summary>
        public static DestinationStamp StampFromReader(IDataRecord reader)
        {
            if (reader == null)
            {
                return null;
            }

            return new DestinationStamp(LatestModify(reader), ObjectCount(reader));
        }

        private static DateTime? LatestModify(IDataRecord reader)
        {
            var value = reader.GetValue(0);

            return value == null || value is DBNull ? (DateTime?)null : Convert.ToDateTime(value);
        }

        /// <summary>
        /// COUNT_BIG returns a bigint, which is wider than the stamp's count. A database cannot have more
        /// objects than an int holds, but this runs on a background thread where an overflow would be an
        /// unhandled exception rather than a wrong answer, so it is clamped instead of converted.
        /// </summary>
        private static int ObjectCount(IDataRecord reader)
        {
            var value = reader.GetValue(1);

            if (value == null || value is DBNull)
            {
                return 0;
            }

            var count = Convert.ToInt64(value);

            return count > int.MaxValue ? int.MaxValue : (int)count;
        }
    }
}
