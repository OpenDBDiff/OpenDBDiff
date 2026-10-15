using System;
using System.Collections.Generic;
using System.Linq;
using OpenDBDiff.Abstractions.Schema;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>How much a dependency problem matters (section 9.4).</summary>
    public enum DependencySeverity
    {
        Info,

        /// <summary>Worth knowing, but Apply may still run.</summary>
        Warning,

        /// <summary>Apply is refused until it is fixed.</summary>
        Blocking
    }

    /// <summary>Which rule of section 9.4 produced a warning.</summary>
    public enum DependencyWarningKind
    {
        /// <summary>An included item needs an object that is created by this run but is not included.</summary>
        RequiredDependencyNotIncluded,

        /// <summary>An included item needs an object that is altered by this run but is not included.</summary>
        DependencyAlterNotIncluded,

        /// <summary>An included Drop has dependents that are included and are not themselves dropped.</summary>
        DependentNotDropped,

        /// <summary>Two or more included items depend on each other.</summary>
        Circular
    }

    /// <summary>How a warning can be made to go away in one step (section 9.4's "fix where possible").</summary>
    public enum DependencyFixKind
    {
        None,

        /// <summary>Include the object the item depends on: the "Select {object}" button.</summary>
        SelectDependency,

        /// <summary>Exclude the object that would break: the "Deselect {object}" button.</summary>
        DeselectItem
    }

    /// <summary>One clickable remedy for a warning.</summary>
    public sealed class DependencyFix
    {
        public DependencyFix(DependencyFixKind kind, DiffItemKey key)
        {
            Kind = kind;
            Key = key;
        }

        public DependencyFixKind Kind { get; }

        public DiffItemKey Key { get; }

        public bool Exists
        {
            get { return Kind != DependencyFixKind.None; }
        }

        public bool IsInclude
        {
            get { return Kind == DependencyFixKind.SelectDependency; }
        }
    }

    /// <summary>
    /// One dependency problem with one included item, in the words of section 6.5.5.
    /// </summary>
    public sealed class DependencyWarning
    {
        public DependencyWarning(DiffItemKey item, DiffItemKey related, DependencyWarningKind kind, DependencySeverity severity, string message, DependencyFix fix)
        {
            Item = item;
            Related = related;
            Kind = kind;
            Severity = severity;
            Message = message;
            Fix = fix;
        }

        /// <summary>The included item the warning is about.</summary>
        public DiffItemKey Item { get; }

        /// <summary>The other object - the dependency, the dependent, or the other end of the cycle.</summary>
        public DiffItemKey Related { get; }

        public DependencyWarningKind Kind { get; }

        public DependencySeverity Severity { get; }

        public string Message { get; }

        public DependencyFix Fix { get; }

        public bool IsBlocking
        {
            get { return Severity == DependencySeverity.Blocking; }
        }
    }

    /// <summary>
    /// The engine's dependency knowledge, as section 14's <c>GetDependencies()</c> supplies it.
    /// </summary>
    /// <remarks>
    /// A graph rather than a pair of lists on each item: the same answer is then reachable from either
    /// end, so "what does this need" and "what needs this" cannot disagree.
    /// </remarks>
    public interface IDependencyGraph
    {
        /// <summary>The objects this one needs; empty when it needs nothing or is unknown to the graph.</summary>
        IReadOnlyList<DiffItemKey> DependenciesOf(DiffItemKey key);

        /// <summary>The objects that need this one.</summary>
        IReadOnlyList<DiffItemKey> DependentsOf(DiffItemKey key);
    }

    /// <summary>
    /// The dependency rules of section 9.4, run over the included set.
    /// </summary>
    /// <remarks>
    /// Section 9.4 is explicit that the engine, not the UI, supplies the graph, and that the check runs
    /// on every inclusion change. Only the rules the specification lists are implemented; the
    /// combination of an included item whose dependency is itself included as a Drop is not one of
    /// them, and pretending otherwise would block applies the specification allows.
    /// </remarks>
    public static class DependencyRules
    {
        /// <summary>Section 9.4: the Apply tooltip while anything is blocking.</summary>
        public static string FixFirstText(int blockingCount)
        {
            return string.Format("Fix {0} dependency problem{1} first", blockingCount, blockingCount == 1 ? string.Empty : "s");
        }

        public static IReadOnlyList<DependencyWarning> Analyze(
            IDependencyGraph graph,
            IReadOnlyList<DiffItem> items,
            Func<DiffItemKey, bool> isIncluded)
        {
            var warnings = new List<DependencyWarning>();

            if (graph == null || items == null || items.Count == 0)
            {
                return warnings;
            }

            var statuses = new Dictionary<DiffItemKey, DiffStatus>();
            var included = new HashSet<DiffItemKey>();

            foreach (var item in items)
            {
                statuses[item.Key] = item.Status;

                if (isIncluded != null && isIncluded(item.Key))
                {
                    included.Add(item.Key);
                }
            }

            foreach (var item in items)
            {
                if (!included.Contains(item.Key))
                {
                    continue;
                }

                if (item.Status == DiffStatus.Drop)
                {
                    // Rule 3: dropping something an included item still needs breaks that item.
                    foreach (var dependent in graph.DependentsOf(item.Key))
                    {
                        if (included.Contains(dependent) && StatusOf(statuses, dependent) != DiffStatus.Drop)
                        {
                            warnings.Add(new DependencyWarning(
                                item.Key,
                                dependent,
                                DependencyWarningKind.DependentNotDropped,
                                DependencySeverity.Blocking,
                                string.Format("{0} is still needed by {1}, which is also selected. Deselect one of the two.",
                                    Name(item.Key), Name(dependent)),
                                new DependencyFix(DependencyFixKind.DeselectItem, item.Key)));
                        }
                    }

                    continue;
                }

                foreach (var dependency in graph.DependenciesOf(item.Key))
                {
                    if (included.Contains(dependency))
                    {
                        continue;
                    }

                    var status = StatusOf(statuses, dependency);

                    if (status == DiffStatus.Create)
                    {
                        // Rule 1: the dependency does not exist in the destination yet.
                        warnings.Add(new DependencyWarning(
                            item.Key,
                            dependency,
                            DependencyWarningKind.RequiredDependencyNotIncluded,
                            DependencySeverity.Blocking,
                            string.Format("Depends on {0}, which is not selected and does not exist in the destination. Applying will fail unless you also select it.",
                                Name(dependency)),
                            new DependencyFix(DependencyFixKind.SelectDependency, dependency)));
                    }
                    else if (status == DiffStatus.Alter)
                    {
                        // Rule 2: the dependency exists but is being changed by this run.
                        warnings.Add(new DependencyWarning(
                            item.Key,
                            dependency,
                            DependencyWarningKind.DependencyAlterNotIncluded,
                            DependencySeverity.Warning,
                            string.Format("Depends on {0}, which is also being changed and is not selected. Applying may fail.",
                                Name(dependency)),
                            new DependencyFix(DependencyFixKind.SelectDependency, dependency)));
                    }

                    // Rule 4: a dependency that is unchanged in the destination is not a problem.
                }
            }

            warnings.AddRange(CircularWarnings(graph, included));

            return warnings;
        }

        public static int BlockingCount(IEnumerable<DependencyWarning> warnings)
        {
            return warnings == null ? 0 : warnings.Count(warning => warning.IsBlocking);
        }

        /// <summary>
        /// Section 9.4's last row: a cycle is a warning with an explanation, and the script generator
        /// orders it as best it can. One warning per cycle, not one per edge.
        /// </summary>
        private static IEnumerable<DependencyWarning> CircularWarnings(IDependencyGraph graph, HashSet<DiffItemKey> included)
        {
            var reported = new HashSet<string>(StringComparer.Ordinal);
            var found = new List<DependencyWarning>();

            foreach (var start in included)
            {
                var seen = new HashSet<DiffItemKey>();

                if (Reaches(graph, start, start, seen, new HashSet<DiffItemKey>()) && seen.Count > 0)
                {
                    var members = seen.OrderBy(key => key.FullName, StringComparer.OrdinalIgnoreCase).ToList();
                    var id = string.Join("|", members.Select(key => key.ToString()));

                    if (!reported.Add(id))
                    {
                        continue;
                    }

                    var other = members.FirstOrDefault(key => !key.Equals(start)) ?? start;

                    found.Add(new DependencyWarning(
                        start,
                        other,
                        DependencyWarningKind.Circular,
                        DependencySeverity.Warning,
                        string.Format("Depends on {0}, which depends back on it. The script orders them as best it can; check the result.",
                            Name(other)),
                        new DependencyFix(DependencyFixKind.None, other)));
                }
            }

            return found;
        }

        /// <summary>Depth-first search for a path back to <paramref name="target"/>, collecting the nodes visited.</summary>
        private static bool Reaches(IDependencyGraph graph, DiffItemKey current, DiffItemKey target, HashSet<DiffItemKey> visited, HashSet<DiffItemKey> onPath)
        {
            if (!onPath.Add(current))
            {
                return false;
            }

            visited.Add(current);

            foreach (var next in graph.DependenciesOf(current))
            {
                if (next.Equals(target))
                {
                    return true;
                }

                if (Reaches(graph, next, target, visited, onPath))
                {
                    return true;
                }
            }

            onPath.Remove(current);
            return false;
        }

        private static DiffStatus StatusOf(Dictionary<DiffItemKey, DiffStatus> statuses, DiffItemKey key)
        {
            DiffStatus status;

            return statuses.TryGetValue(key, out status) ? status : DiffStatus.Identical;
        }

        /// <summary>
        /// How a warning names an object: the schema-qualified name, because two schemas can hold the
        /// same table name.
        /// </summary>
        public static string Name(DiffItemKey key)
        {
            if (key == null)
            {
                return "(unknown)";
            }

            return string.IsNullOrEmpty(key.Schema) ? key.Name : key.Schema + "." + key.Name;
        }
    }
}
