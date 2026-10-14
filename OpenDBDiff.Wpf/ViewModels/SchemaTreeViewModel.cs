using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.Wpf.Services;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// The schema tree: what a comparison produced, filtered by what the user wants to see, with the
    /// objects to script ticked off.
    /// </summary>
    /// <remarks>
    /// Replaces OpenDBDiff.UI.SchemaTreeView. The four filter check boxes, the counting in the folder
    /// captions, the check-box propagation and the lazy reading of table and view columns are all
    /// carried over; the difference is that the tree is data rather than a control, so the object
    /// model is read once per filter change and the nodes are plain view models.
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public sealed class SchemaTreeViewModel
    {
        /// <summary>
        /// Which objects were ticked, so a re-compare can tick the same ones again even though the
        /// schema objects themselves are new instances.
        /// </summary>
        private readonly HashSet<int> checkedObjects = new HashSet<int>();

        private IDatabase? database;
        private SchemaNodeViewModel? selectedNode;

        public SchemaTreeViewModel()
        {
            ShowNewItems = true;
            ShowMissingItems = true;
            ShowChangedItems = true;
            ShowUnchangedItems = false;
        }

        public ObservableCollection<SchemaNodeViewModel> Nodes { get; } = new ObservableCollection<SchemaNodeViewModel>();

        public bool ShowNewItems { get; set; }

        public bool ShowMissingItems { get; set; }

        public bool ShowChangedItems { get; set; }

        public bool ShowUnchangedItems { get; set; }

        public IDatabase? Database
        {
            get { return database; }
            set
            {
                database = value;
                Rebuild();
            }
        }

        /// <summary>
        /// Changes when the user picks another object, which is what the diff panes follow.
        /// </summary>
        public string? SelectedObjectName { get; private set; }

        public SchemaNodeViewModel? SelectedNode
        {
            get { return selectedNode; }
            set
            {
                selectedNode = value;
                OnSelectedNodeSet();
            }
        }

        public event EventHandler<string?>? SelectedObjectChanged;

        public void OnNodeChecked()
        {
            RememberCheckedObjects();
        }

        public List<ISchemaBase> GetCheckedSchemas()
        {
            RememberCheckedObjects();

            return CheckedNodes()
                .Select(node => node.Item!)
                .ToList();
        }

        /// <summary>
        /// Ticks the objects that were ticked before the last comparison ran.
        /// </summary>
        public void SetCheckedSchemas(IEnumerable<ISchemaBase> schemas)
        {
            checkedObjects.Clear();

            foreach (var schema in schemas)
            {
                checkedObjects.Add(schema.Id);
            }

            foreach (var node in AllNodes())
            {
                if (node.Item != null)
                {
                    node.IsChecked = checkedObjects.Contains(node.Item.Id);
                }
            }
        }

        /// <summary>
        /// Visibility flags, as the four check boxes express them.
        /// </summary>
        public SchemaVisibility Visibility
        {
            get
            {
                var visibility = SchemaVisibility.None;

                if (ShowNewItems)
                {
                    visibility |= SchemaVisibility.New;
                }

                if (ShowMissingItems)
                {
                    visibility |= SchemaVisibility.Missing;
                }

                if (ShowChangedItems)
                {
                    visibility |= SchemaVisibility.Changed;
                }

                if (ShowUnchangedItems)
                {
                    visibility |= SchemaVisibility.Unchanged;
                }

                return visibility;
            }
        }

        private void OnShowNewItemsChanged()
        {
            Rebuild();
        }

        private void OnShowMissingItemsChanged()
        {
            Rebuild();
        }

        private void OnShowChangedItemsChanged()
        {
            Rebuild();
        }

        private void OnShowUnchangedItemsChanged()
        {
            Rebuild();
        }

        private void Rebuild()
        {
            Nodes.Clear();
            SelectedObjectName = null;

            if (database is null)
            {
                return;
            }

            Nodes.Add(SchemaTreeBuilder.Build(this, database, Visibility));

            foreach (var node in AllNodes().Where(node => node.Item != null))
            {
                node.IsChecked = checkedObjects.Contains(node.Item!.Id);
            }
        }

        private void OnSelectedNodeSet()
        {
            if (selectedNode is null)
            {
                return;
            }

            // Table and view columns are only read when the node is opened, which is what keeps a
            // large database usable. Same behaviour as the WinForms tree's AfterSelect handler.
            if (selectedNode.Item != null
                && ComparisonService.ExpandableTypes.Contains(selectedNode.Item.ObjectType)
                && selectedNode.Children.Count == 0)
            {
                foreach (var child in SchemaTreeBuilder.BuildChildren(this, selectedNode.Item, Visibility))
                {
                    selectedNode.Children.Add(child);
                }
            }

            SelectedObjectName = selectedNode.Item?.FullName;
            SelectedObjectChanged?.Invoke(this, SelectedObjectName);
        }

        private IEnumerable<SchemaNodeViewModel> AllNodes()
        {
            foreach (var root in Nodes)
            {
                foreach (var node in Walk(root))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<SchemaNodeViewModel> CheckedNodes()
        {
            return AllNodes().Where(node => node.Item != null && node.IsChecked);
        }

        private static IEnumerable<SchemaNodeViewModel> Walk(SchemaNodeViewModel node)
        {
            yield return node;

            foreach (var child in node.Children)
            {
                foreach (var descendant in Walk(child))
                {
                    yield return descendant;
                }
            }
        }

        private void RememberCheckedObjects()
        {
            checkedObjects.Clear();

            foreach (var node in CheckedNodes())
            {
                checkedObjects.Add(node.Item!.Id);
            }
        }
    }
}
