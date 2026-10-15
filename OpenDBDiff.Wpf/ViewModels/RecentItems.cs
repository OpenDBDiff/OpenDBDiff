using System;
using OpenDBDiff.Ui.Model;
using OpenDBDiff.Wpf.Infrastructure;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>Which card of the connection panel is in focus (section 6.1.1).</summary>
    public enum ConnectionSide
    {
        Source,
        Destination
    }

    /// <summary>
    /// One entry of the Recent comparisons menu (section 6.1.6). It carries its own command so the menu
    /// can be a plain item list.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class RecentPairViewModel
    {
        public RecentPairViewModel(RecentPair pair, DateTime nowUtc, Action<RecentPair> open)
        {
            Pair = pair;
            Header = pair.Direction;
            Detail = pair.LastRunText(nowUtc);
            OpenCommand = new RelayCommand(() => open(pair));
        }

        public RecentPair Pair { get; }

        /// <summary>The direction, which is what the menu line says (section 6.1.6).</summary>
        public string Header { get; }

        public string Detail { get; }

        public RelayCommand OpenCommand { get; }
    }

    /// <summary>One entry of the Recent projects menu (section 12).</summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class RecentProjectViewModel
    {
        public RecentProjectViewModel(string path, Action<string> open)
        {
            Path = path;
            OpenCommand = new RelayCommand(() => open(path));
        }

        public string Path { get; }

        public RelayCommand OpenCommand { get; }
    }
}
