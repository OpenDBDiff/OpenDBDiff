using System;
using System.Threading;
using System.Threading.Tasks;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Wpf.Services
{
    /// <summary>
    /// Section 9.6's optional background check: while results are on screen, watch the destination for a
    /// change made by something other than this application.
    /// </summary>
    public interface IDestinationWatch
    {
        bool IsRunning { get; }

        /// <summary>
        /// Takes the stamp that the comparison describes and starts watching. Not starting is an answer too:
        /// a destination that cannot be read cannot be watched.
        /// </summary>
        Task StartAsync(string destinationConnectionString);

        void Stop();

        /// <summary>Raised once, with the change in words, when the destination moves under the results.</summary>
        event EventHandler<string> Changed;
    }
}
