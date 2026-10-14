using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Events;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Driver;
using OpenDBDiff.SqlServer.Schema.Options;

namespace OpenDBDiff.Wpf.Services
{
    /// <summary>What to compare.</summary>
    public sealed record ComparisonRequest(string OriginConnectionString, string DestinationConnectionString, IOption Options);

    /// <summary>One database loader's progress, as the progress window shows it.</summary>
    public sealed record DatabaseProgress(int Value, int Maximum, string Message);

    /// <summary>The two schema snapshots a comparison produced.</summary>
    public sealed record ComparisonResult(IDatabase Origin, IDatabase Destination);

    /// <summary>
    /// A failure during loading or comparing, with the step that was running.
    /// </summary>
    public sealed class ComparisonException : Exception
    {
        public ComparisonException(string location, Exception inner)
            : base("Error " + location, inner)
        {
            Location = location;
        }

        public string Location { get; }
    }

    public interface IComparisonService
    {
        Task<ComparisonResult> CompareAsync(
            ComparisonRequest request,
            IProgress<DatabaseProgress> originProgress,
            IProgress<DatabaseProgress> destinationProgress,
            CancellationToken cancellationToken);
    }

    /// <summary>
    /// Runs the comparison off the UI thread.
    /// </summary>
    /// <remarks>
    /// The order of operations is the one the WinForms ProgressForm used, and it is deliberately
    /// unchanged: the destination is loaded first, the origin second, the origin is cloned, and the
    /// comparer marks the destination up so that migrating the origin to the destination matches the
    /// source database. The clone is what keeps the origin's own statuses available afterwards, which
    /// is how the tree can show the old and new text of the same object.
    /// </remarks>
    public sealed class ComparisonService : IComparisonService
    {
        public Task<ComparisonResult> CompareAsync(
            ComparisonRequest request,
            IProgress<DatabaseProgress> originProgress,
            IProgress<DatabaseProgress> destinationProgress,
            CancellationToken cancellationToken)
        {
            return Task.Run(
                () => Compare(request, originProgress, destinationProgress, cancellationToken),
                cancellationToken);
        }

        private static ComparisonResult Compare(
            ComparisonRequest request,
            IProgress<DatabaseProgress> originProgress,
            IProgress<DatabaseProgress> destinationProgress,
            CancellationToken cancellationToken)
        {
            var origin = new SqlServerGenerator(request.OriginConnectionString, request.Options);
            var destination = new SqlServerGenerator(request.DestinationConnectionString, request.Options);

            origin.OnProgress += args => originProgress.Report(ToProgress(args, origin));
            destination.OnProgress += args => destinationProgress.Report(ToProgress(args, destination));

            var location = "initializing";

            try
            {
                location = "Loading " + request.OriginConnectionString;
                var originDatabase = origin.Process();

                location = "Loading " + request.DestinationConnectionString;
                var destinationDatabase = destination.Process();

                var originClone = (IDatabase)originDatabase.Clone(null);

                location = "Comparing Databases";
                var compared = new SqlServerComparer().Compare(originDatabase, destinationDatabase);

                cancellationToken.ThrowIfCancellationRequested();

                return new ComparisonResult(originClone, compared);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ComparisonException(location, ex);
            }
        }

        private static DatabaseProgress ToProgress(ProgressEventArgs args, SqlServerGenerator generator)
        {
            return new DatabaseProgress(args.Progress, generator.GetMaxValue(), args.Message);
        }

        /// <summary>
        /// The options a comparison starts from, matching the WinForms project handler's defaults.
        /// </summary>
        public static IOption CreateDefaultOptions()
        {
            return new SqlOption(true);
        }

        /// <summary>
        /// The lexer the SQL panes use, matching the WinForms front end's GetScriptLanguage.
        /// </summary>
        public static string ScriptLanguage => "sql";

        /// <summary>
        /// The object types whose children are only read when the node is opened. Keeps a large
        /// database responsive, as the WinForms tree did.
        /// </summary>
        public static IReadOnlyCollection<ObjectType> ExpandableTypes { get; } =
            new[] { ObjectType.Table, ObjectType.View };
    }
}
