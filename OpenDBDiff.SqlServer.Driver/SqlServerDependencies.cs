using System;
using System.Collections.Generic;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.SqlServer.Driver
{
    /// <summary>
    /// The dependency graph section 14 asks <c>GetDependencies()</c> to return, read from the schema
    /// model the comparison produced.
    /// </summary>
    /// <remarks>
    /// Two sources, because the model keeps them in two places. Code objects (views, functions,
    /// procedures, triggers) carry <c>DependenciesIn</c> and <c>DependenciesOut</c> as names, which is
    /// what SQL Server's own dependency metadata gave them. A table's dependencies are its foreign
    /// keys' referenced tables. Names are matched back to the comparison's items, so an object outside
    /// the comparison - a system object, or one that did not change and is not listed - simply does not
    /// appear rather than producing a warning about something the user cannot select.
    ///
    /// The graph is a snapshot: it is built once per comparison, from the same database the item list
    /// came from.
    /// </remarks>
    public sealed class SqlServerDependencyGraph : IDependencyGraph
    {
        private readonly Dictionary<DiffItemKey, List<DiffItemKey>> dependencies = new Dictionary<DiffItemKey, List<DiffItemKey>>();
        private readonly Dictionary<DiffItemKey, List<DiffItemKey>> dependents = new Dictionary<DiffItemKey, List<DiffItemKey>>();

        private SqlServerDependencyGraph()
        {
        }

        public static SqlServerDependencyGraph Build(IDatabase database, IReadOnlyList<DiffItem> items, Func<DiffItemKey, ISchemaBase> resolve)
        {
            var graph = new SqlServerDependencyGraph();

            if (items == null || resolve == null)
            {
                return graph;
            }

            // Only objects the comparison knows about can be named in a warning.
            var known = new Dictionary<string, DiffItemKey>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in items)
            {
                known[Normalize(item.Key.FullName)] = item.Key;
            }

            foreach (var item in items)
            {
                var schema = SafeResolve(resolve, item.Key);

                if (schema == null)
                {
                    continue;
                }

                foreach (var name in OutgoingNames(schema))
                {
                    var normalized = Normalize(name);
                    DiffItemKey target;

                    if (normalized.Length == 0 || !known.TryGetValue(normalized, out target) || target.Equals(item.Key))
                    {
                        continue;
                    }

                    graph.Add(item.Key, target);
                }
            }

            return graph;
        }

        public IReadOnlyList<DiffItemKey> DependenciesOf(DiffItemKey key)
        {
            List<DiffItemKey> found;

            return key != null && dependencies.TryGetValue(key, out found) ? found : new List<DiffItemKey>();
        }

        public IReadOnlyList<DiffItemKey> DependentsOf(DiffItemKey key)
        {
            List<DiffItemKey> found;

            return key != null && dependents.TryGetValue(key, out found) ? found : new List<DiffItemKey>();
        }

        /// <summary>
        /// What an object needs. A code object knows its own list; a table's dependencies are the tables
        /// its foreign keys point at, which is the part of a table's needs that decides an order.
        /// </summary>
        private static IEnumerable<string> OutgoingNames(ISchemaBase schema)
        {
            var code = schema as Code;

            if (code != null)
            {
                foreach (var name in code.DependenciesIn)
                {
                    yield return name;
                }

                yield break;
            }

            var table = schema as Table;

            if (table == null)
            {
                yield break;
            }

            foreach (var constraint in table.Constraints)
            {
                if (constraint.Type == Constraint.ConstraintType.ForeignKey && !string.IsNullOrEmpty(constraint.RelationalTableFullName))
                {
                    yield return constraint.RelationalTableFullName;
                }
            }
        }

        private static ISchemaBase SafeResolve(Func<DiffItemKey, ISchemaBase> resolve, DiffItemKey key)
        {
            try
            {
                return resolve(key);
            }
            catch (Exception)
            {
                // An item that cannot be resolved simply has no known dependencies.
                return null;
            }
        }

        /// <summary>
        /// The model writes names bracketed ("[dbo].[Orders]") and the item list writes them plain
        /// ("dbo.Orders"); the graph has to match one to the other.
        /// </summary>
        public static string Normalize(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return string.Empty;
            }

            return fullName.Replace("[", string.Empty).Replace("]", string.Empty).Trim();
        }

        private void Add(DiffItemKey dependent, DiffItemKey dependency)
        {
            var outgoing = dependencies.ContainsKey(dependent) ? dependencies[dependent] : dependencies[dependent] = new List<DiffItemKey>();

            if (!outgoing.Contains(dependency))
            {
                outgoing.Add(dependency);
            }

            var incoming = dependents.ContainsKey(dependency) ? dependents[dependency] : dependents[dependency] = new List<DiffItemKey>();

            if (!incoming.Contains(dependent))
            {
                incoming.Add(dependent);
            }
        }
    }
}
