using System;
using System.Windows;
using System.Windows.Controls;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// The detail pane (section 6.5). Its only code is the scroll that section 6.5.3's F7 and Shift+F7
    /// need: the position of a change belongs to the model, the scrolling belongs to the view.
    /// </summary>
    public partial class DetailPaneView : UserControl
    {
        /// <summary>Every diff row is 24 px, which is what turns a row index into an offset.</summary>
        private const double RowHeight = 24;

        private ResultsViewModel? subscribed;

        public DetailPaneView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            Unloaded += OnUnloaded;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Subscribe();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            Unsubscribe();

            var viewModel = DataContext as MainViewModel;

            if (viewModel == null || viewModel.Results == null)
            {
                return;
            }

            subscribed = viewModel.Results;
            subscribed.HunkFocused += OnHunkFocused;
        }

        private void Unsubscribe()
        {
            if (subscribed != null)
            {
                subscribed.HunkFocused -= OnHunkFocused;
                subscribed = null;
            }
        }

        private void OnHunkFocused(object? sender, int index)
        {
            if (index < 0)
            {
                return;
            }

            var viewModel = DataContext as MainViewModel;

            if (viewModel == null)
            {
                return;
            }

            // The change's index is the row it starts on, so the offset is that row's top edge. A row is
            // held a little below the top so the change that follows it stays in view.
            var offset = Math.Max(0, (index * RowHeight) - RowHeight);

            var scroller = viewModel.Results != null && viewModel.Results.IsInline
                ? InlineScroller
                : SideBySideScroller;

            scroller.ScrollToVerticalOffset(offset);
        }
    }
}
