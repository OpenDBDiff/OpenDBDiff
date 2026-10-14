namespace OpenDBDiff.Abstractions.Schema.Model
{
    /// <summary>
    /// Compares two schema snapshots and marks up the destination with the changes needed to match
    /// the origin.
    /// </summary>
    /// <remarks>
    /// Moved out of OpenDBDiff.Abstractions.Ui (net48) alongside <see cref="IGenerator"/> so that
    /// front ends other than WinForms can run a comparison.
    /// </remarks>
    public interface IDatabaseComparer
    {
        IDatabase Compare(IDatabase origin, IDatabase destination);
    }
}
