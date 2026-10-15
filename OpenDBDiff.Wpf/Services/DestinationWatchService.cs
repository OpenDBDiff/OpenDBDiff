using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Wpf.Services
{
    /// <summary>
    /// Watches the destination on a slow timer, off the UI thread (section 9.6).
    /// </summary>
    /// <remarks>
    /// Three decisions are worth naming.
    ///
    /// The interval is slow and the query is one aggregate, because this runs while somebody is reading
    /// results: the point is to notice a colleague's deploy, and nothing about that needs a query every
    /// second. A probe never overlaps itself either - a destination that takes longer to answer than the
    /// interval gets one query in flight, not a queue of them.
    ///
    /// The baseline is taken when watching starts, which is just after the comparison finished. That is the
    /// state the results describe, so it is the right thing to compare against. A change made *during* the
    /// comparison is therefore part of the baseline and will not be reported; the comparison's own results
    /// are what the user is reading, and they are stale for a compare that raced with a write in a way this
    /// check was never going to catch.
    ///
    /// It fires once. After that the results are known to be out of date, which is the whole message, and
    /// continuing to poll a database nobody is going to write to helps no one. The event is raised on the
    /// thread this service was created on - the UI thread - because the view model that listens to it is
    /// bound to a window.
    /// </remarks>
    public sealed class DestinationWatchService : IDestinationWatch, IDisposable
    {
        /// <summary>Slow enough to be invisible, quick enough to catch a deploy while it is being read.</summary>
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(20);

        private readonly IComparisonService comparison;
        private readonly SynchronizationContext? context;
        private readonly System.Timers.Timer timer;

        private DestinationStamp? baseline;
        private string? connectionString;
        private bool probing;

        public DestinationWatchService(IComparisonService comparison)
        {
            this.comparison = comparison;

            // Captured here, on the UI thread, so the event can be delivered where the binding expects it.
            context = SynchronizationContext.Current;

            timer = new System.Timers.Timer(Interval.TotalMilliseconds) { AutoReset = true };
            timer.Elapsed += OnElapsed;
        }

        public bool IsRunning { get; private set; }

        public event EventHandler<string>? Changed;

        public async Task StartAsync(string destinationConnectionString)
        {
            Stop();

            if (string.IsNullOrEmpty(destinationConnectionString))
            {
                return;
            }

            DestinationStamp? stamp;

            try
            {
                stamp = await comparison.FingerprintAsync(destinationConnectionString);
            }
            catch (Exception)
            {
                stamp = null;
            }

            if (stamp == null)
            {
                // Nothing to compare against, so there is nothing to notice. The next comparison will try
                // again; a destination that cannot be read is not a change.
                return;
            }

            baseline = stamp;
            connectionString = destinationConnectionString;
            IsRunning = true;
            timer.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            timer.Stop();

            baseline = null;
            connectionString = null;
        }

        private async void OnElapsed(object? sender, ElapsedEventArgs e)
        {
            if (!IsRunning || probing || baseline == null || connectionString == null)
            {
                return;
            }

            probing = true;

            try
            {
                var current = await comparison.FingerprintAsync(connectionString);

                if (!DestinationStamp.Same(baseline, current))
                {
                    var detail = current?.DescribeChangeFrom(baseline) ?? "the destination could not be read";

                    Stop();
                    Raise(detail);
                }
            }
            catch (Exception)
            {
                // A check that fails is a check that learned nothing; it tries again on the next tick.
            }
            finally
            {
                probing = false;
            }
        }

        private void Raise(string detail)
        {
            var handler = Changed;

            if (handler == null)
            {
                return;
            }

            if (context != null)
            {
                context.Post(_ => handler(this, detail), null);
            }
            else
            {
                handler(this, detail);
            }
        }

        public void Dispose()
        {
            timer.Elapsed -= OnElapsed;
            timer.Dispose();
        }
    }
}
