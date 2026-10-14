using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenDBDiff.Abstractions.Schema.Model
{
    /// <summary>
    /// Which objects a schema tree should show.
    /// </summary>
    [Flags]
    public enum SchemaVisibility
    {
        None = 0,
        New = 1,
        Missing = 2,
        Changed = 4,
        Unchanged = 8,

        /// <summary>Everything except unchanged objects, which is what both front ends start with.</summary>
        Default = New | Missing | Changed
    }

    /// <summary>
    /// The visibility rules of the schema tree: one object, given the statuses it carries and the
    /// visibility flags the user chose.
    /// </summary>
    /// <remarks>
    /// This used to live in the WinForms tree control's CanNodeAdd method, which meant the rules
    /// could only be exercised by opening a window. They are domain rules, so they live with the
    /// schema model and both front ends call them.
    /// </remarks>
    public static class SchemaVisibilityEvaluator
    {
        /// <summary>
        /// Statuses that make an object part of a comparison at all, i.e. everything except
        /// <see cref="ObjectStatus.Original"/>.
        /// </summary>
        public static readonly ObjectStatus ChangeStatuses =
            ObjectStatus.Alter |
            ObjectStatus.AlterBody |
            ObjectStatus.AlterWhitespace |
            ObjectStatus.Bind |
            ObjectStatus.ChangeOwner |
            ObjectStatus.Disabled |
            ObjectStatus.DropOlder |
            ObjectStatus.PermissionSet |
            ObjectStatus.Rebuild |
            ObjectStatus.RebuildDependencies |
            ObjectStatus.Update;

        /// <summary>
        /// True when a tree should list this object.
        /// </summary>
        public static bool IsVisible(ISchemaBase item, SchemaVisibility visibility)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            // Original is zero, so it cannot be tested with HasState.
            if (item.Status == ObjectStatus.Original && visibility.HasFlag(SchemaVisibility.Unchanged))
            {
                return true;
            }

            if (item.HasState(ObjectStatus.Drop) && visibility.HasFlag(SchemaVisibility.Missing))
            {
                return true;
            }

            if (item.HasState(ObjectStatus.Create) && visibility.HasFlag(SchemaVisibility.New))
            {
                return true;
            }

            if (!visibility.HasFlag(SchemaVisibility.Changed))
            {
                return false;
            }

            return ChangeStatuses
                .Value()
                .Any(status => item.HasState(status));
        }

        /// <summary>
        /// Every <see cref="ObjectStatus"/> value, OR'd together.
        /// </summary>
        public static ObjectStatus AllStatuses()
        {
            var all = ObjectStatus.Original;

            foreach (ObjectStatus status in Enum.GetValues(typeof(ObjectStatus)))
            {
                all |= status;
            }

            return all;
        }

        /// <summary>
        /// Statuses that <see cref="IsVisible"/> does not account for. Empty in a healthy build: the
        /// callers rely on this to notice when someone adds a status without teaching the tree what
        /// to do with it.
        /// </summary>
        public static IReadOnlyCollection<ObjectStatus> UnhandledStatuses()
        {
            var handled = ObjectStatus.Original |
                          ObjectStatus.Create |
                          ObjectStatus.Drop |
                          ChangeStatuses;

            // Original is excluded rather than tested: it is zero, and "handled & 0" is always zero,
            // so it would report itself as unhandled. It is handled by the first rule in IsVisible.
            return Enum.GetValues(typeof(ObjectStatus))
                .Cast<ObjectStatus>()
                .Where(status => status != ObjectStatus.Original && (handled & status) == 0)
                .ToList();
        }

        private static IEnumerable<ObjectStatus> Value(this ObjectStatus statuses)
        {
            foreach (ObjectStatus status in Enum.GetValues(typeof(ObjectStatus)))
            {
                if (status != ObjectStatus.Original && statuses.HasFlag(status))
                {
                    yield return status;
                }
            }
        }
    }
}
