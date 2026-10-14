using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Attributes;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.Wpf.Services;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// How a tree node is painted. PropertyChanged.Fody weaves <see cref="SchemaNodeViewModel"/>
    /// into a change-notifying class; the converter turns this into a brush.
    /// </summary>
    public enum NodeColour
    {
        Default,
        Create,
        Drop,
        Alter,
        AlterWhitespace,
        Rebuild,
        Mixed
    }

    public enum SchemaNodeKind
    {
        Database,
        Folder,
        Object
    }

    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public sealed class SchemaNodeViewModel
    {
        private readonly SchemaTreeViewModel? owner;

        public SchemaNodeViewModel(SchemaTreeViewModel? owner, SchemaNodeKind kind, string text)
        {
            this.owner = owner;
            Kind = kind;
            Text = text;
            Children = new ObservableCollection<SchemaNodeViewModel>();
        }

        public SchemaNodeKind Kind { get; }

        public string Text { get; set; }

        public NodeColour Colour { get; set; }

        public ObservableCollection<SchemaNodeViewModel> Children { get; }

        /// <summary>The schema object this node stands for; null for the database and folder nodes.</summary>
        public ISchemaBase? Item { get; set; }

        public bool IsExpanded { get; set; }

        public bool IsSelected { get; set; }

        public bool IsChecked { get; set; }

        /// <summary>
        /// Only objects can be checked, and an object node has no children to propagate to.
        /// </summary>
        public bool IsCheckable
        {
            get { return Kind == SchemaNodeKind.Object; }
        }

        /// <summary>
        /// Folder nodes carry the summary of what they hold: one colour when everything under them
        /// agrees, plum when they do not. Matches the WinForms tree.
        /// </summary>
        public static NodeColour Combine(NodeColour current, NodeColour next)
        {
            if (next == NodeColour.Default)
            {
                return current;
            }

            if (current == NodeColour.Default)
            {
                return next;
            }

            return current == next ? current : NodeColour.Mixed;
        }

        /// <summary>
        /// Checking a folder checks the objects under it, as the WinForms tree did (one level, and
        /// only for nodes that are not themselves objects).
        /// </summary>
        private void OnIsCheckedChanged()
        {
            if (IsCheckable)
            {
                return;
            }

            foreach (var child in Children)
            {
                child.IsChecked = IsChecked;
            }

            owner?.OnNodeChecked();
        }
    }

    /// <summary>
    /// Builds the object tree for a comparison result.
    /// </summary>
    /// <remarks>
    /// The shape is taken from the schema model: the database's properties marked with
    /// <see cref="SchemaNodeAttribute"/> become the folders, and each folder lists the objects of that
    /// collection that pass the visibility rules. Nothing about the object model is hard-coded here,
    /// which is why adding a schema type shows up in the tree without touching this class.
    /// </remarks>
    public static class SchemaTreeBuilder
    {
        public static SchemaNodeViewModel Build(SchemaTreeViewModel owner, IDatabase database, SchemaVisibility visibility)
        {
            var root = new SchemaNodeViewModel(owner, SchemaNodeKind.Database, database.Name)
            {
                // The WinForms tree expanded the database node as it built it.
                IsExpanded = true
            };

            foreach (var folder in BuildChildren(owner, database, visibility))
            {
                root.Children.Add(folder);
            }

            return root;
        }

        /// <summary>
        /// The folder nodes for any schema object's <see cref="SchemaNodeAttribute"/> collections.
        /// Used for the database itself and, lazily, for the columns of a table or view that the user
        /// has just opened.
        /// </summary>
        public static IEnumerable<SchemaNodeViewModel> BuildChildren(
            SchemaTreeViewModel owner,
            ISchemaBase source,
            SchemaVisibility visibility)
        {
            foreach (var property in source.GetType().GetProperties())
            {
                var attribute = GetNodeAttribute(property);

                if (attribute is null)
                {
                    continue;
                }

                yield return BuildFolder(owner, source, property, attribute, visibility);
            }
        }

        private static SchemaNodeViewModel BuildFolder(
            SchemaTreeViewModel owner,
            ISchemaBase parent,
            PropertyInfo property,
            SchemaNodeAttribute attribute,
            SchemaVisibility visibility)
        {
            var folder = new SchemaNodeViewModel(owner, SchemaNodeKind.Folder, attribute.Name);
            var items = property.GetValue(parent, null) as IEnumerable<ISchemaBase>;

            if (items is null)
            {
                return folder;
            }

            var total = 0;

            foreach (var item in items)
            {
                total++;

                if (!SchemaVisibilityEvaluator.IsVisible(item, visibility))
                {
                    continue;
                }

                var text = attribute.IsFullName ? item.FullName : item.Name;
                var node = new SchemaNodeViewModel(owner, SchemaNodeKind.Object, text)
                {
                    Item = item,
                    Colour = ColourOf(item)
                };

                folder.Children.Add(node);
                folder.Colour = SchemaNodeViewModel.Combine(folder.Colour, node.Colour);
            }

            folder.Text = folder.Children.Count == total
                ? string.Format("{0} ({1})", attribute.Name, total)
                : string.Format("{0} ({1} of {2})", attribute.Name, folder.Children.Count, total);

            return folder;
        }

        /// <summary>
        /// The colour rules of the WinForms tree, in the same order: the first status that applies
        /// wins, and an object whose statuses are none of these stays at the default colour.
        /// </summary>
        public static NodeColour ColourOf(ISchemaBase item)
        {
            if (item.Status == ObjectStatus.Drop)
            {
                return NodeColour.Drop;
            }

            if (item.Status == ObjectStatus.Create)
            {
                return NodeColour.Create;
            }

            if (item.HasState(ObjectStatus.Alter) || item.HasState(ObjectStatus.Disabled))
            {
                return NodeColour.Alter;
            }

            if (item.HasState(ObjectStatus.AlterWhitespace))
            {
                return NodeColour.AlterWhitespace;
            }

            if (item.HasState(ObjectStatus.Rebuild))
            {
                return NodeColour.Rebuild;
            }

            return NodeColour.Default;
        }

        private static SchemaNodeAttribute? GetNodeAttribute(PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes(typeof(SchemaNodeAttribute), true);

            return attributes.Length > 0 ? (SchemaNodeAttribute)attributes[0] : null;
        }
    }
}
