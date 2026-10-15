using System;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// What a database looked like at a moment, for section 9.6's background check: the newest object
    /// modification and how many objects there are.
    /// </summary>
    /// <remarks>
    /// Section 9.6 sketches this as "comparing destination modify_date metadata to the compare timestamp".
    /// A timestamp cannot be compared that way and be right: sys.objects.modify_date is a datetime in the
    /// server's own local time, while the compare timestamp is this machine's clock, and there is no offset
    /// to reconcile them without asking the server - and on a server in another time zone the check would
    /// either fire every twenty seconds or never. Two stamps taken at the same moment by the same machine
    /// need no shared clock at all, and the pair of them answers the question the timestamp was there to
    /// answer: did anything change since we looked?
    ///
    /// Two values rather than one, because they catch different things. A create or an alter moves the
    /// newest modification date; a drop moves neither, so the count is what notices it. Between them the
    /// three ways an object can change are all visible for the cost of one aggregate query.
    /// </remarks>
    public sealed class DestinationStamp : IEquatable<DestinationStamp>
    {
        public DestinationStamp(DateTime? latestModify, int objectCount)
        {
            LatestModify = latestModify;
            ObjectCount = objectCount;
        }

        /// <summary>The newest object modification, or null when the database has no objects to report.</summary>
        public DateTime? LatestModify { get; }

        public int ObjectCount { get; }

        /// <summary>
        /// Whether two stamps describe the same database, either of which may be missing because the database
        /// could not be read.
        /// </summary>
        /// <remarks>
        /// Two missing stamps count as no change, which is the point: a check that cannot see the destination
        /// must not tell the user it changed, and a compare whose own stamp failed to arrive would otherwise
        /// report one the first time the network hiccupped. One missing stamp is a change, because the two
        /// reads disagree about whether there is anything there.
        /// </remarks>
        public static bool Same(DestinationStamp first, DestinationStamp second)
        {
            if (first == null && second == null)
            {
                return true;
            }

            return first != null && first.SameAs(second);
        }

        /// <summary>
        /// Whether this stamp and another describe the same database. A stamp compared with nothing is not a
        /// match, so a check that could not read the database never reports a change it did not see.
        /// </summary>
        public bool SameAs(DestinationStamp other)
        {
            return other != null && other.ObjectCount == ObjectCount && Nullable.Equals(other.LatestModify, LatestModify);
        }

        /// <summary>
        /// What changed, in words a person can act on - section 9.6's banner says the destination changed,
        /// and this says which way.
        /// </summary>
        public string DescribeChangeFrom(DestinationStamp baseline)
        {
            if (baseline == null)
            {
                return "the destination could not be read";
            }

            var difference = ObjectCount - baseline.ObjectCount;

            if (difference > 0)
            {
                return difference == 1 ? "1 object was added" : string.Format("{0} objects were added", difference);
            }

            if (difference < 0)
            {
                return difference == -1 ? "1 object was removed" : string.Format("{0} objects were removed", -difference);
            }

            return "an object was written to";
        }

        public bool Equals(DestinationStamp other)
        {
            return SameAs(other);
        }

        public override bool Equals(object obj)
        {
            return SameAs(obj as DestinationStamp);
        }

        public override int GetHashCode()
        {
            return (ObjectCount * 397) ^ (LatestModify.HasValue ? LatestModify.Value.Ticks.GetHashCode() : 0);
        }
    }
}
