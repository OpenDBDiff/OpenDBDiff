using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Attributes;
using OpenDBDiff.Abstractions.Schema.Model;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// Turns a comparison into the redesign's item list.
    /// </summary>
    /// <remarks>
    /// The engine's own vocabulary, from CompareDatabase.GenerateDifferences: the origin is the schema
    /// <em>before</em> the actions are applied and the destination is the schema <em>after</em> them. So
    /// the marked-up database - the one that describes the desired state - is the specification's
    /// Source, and the schema it will be compared against is the Destination that gets modified. That
    /// is the mapping used here, and it is the same one the existing window's labels already use.
    ///
    /// Only top-level objects appear for now: column, index and constraint-level items need the engine
    /// work listed in section 14, and the diff summary falls back to line counts until then.
    /// </remarks>
    public static class DiffItemBuilder
    {
        public static IReadOnlyList<DiffItem> Build(IDatabase source, IDatabase destination)
        {
            var items = new List<DiffItem>();

            foreach (var item in TopLevelObjectsOf(source))
            {
                var status = StatusOf(item.Status);
                var key = KeyOf(item);

                var sourceDefinition = status == DiffStatus.Drop ? null : ToSql(item);
                var destinationDefinition = status == DiffStatus.Create ? null : ToSql(destination?.Find(item.FullName));

                var diffItem = new DiffItem(key, status)
                {
                    SourceDefinition = sourceDefinition,
                    DestinationDefinition = destinationDefinition,
                    Script = status == DiffStatus.Identical ? string.Empty : Script(item),
                    DiffSummary = DiffText.Summary(status, sourceDefinition, destinationDefinition)
                };

                items.Add(diffItem);
            }

            return items;
        }

        /// <summary>
        /// Every object the schema model exposes at the top level, read from the same attributes the
        /// existing tree reads, so a new object type shows up here without changing this class.
        /// </summary>
        public static IEnumerable<ISchemaBase> TopLevelObjectsOf(ISchemaBase database)
        {
            if (database == null)
            {
                yield break;
            }

            foreach (var property in database.GetType().GetProperties())
            {
                var attributes = property.GetCustomAttributes(typeof(SchemaNodeAttribute), true);

                if (attributes.Length == 0)
                {
                    continue;
                }

                if (property.GetValue(database, null) is IEnumerable collection)
                {
                    foreach (var entry in collection)
                    {
                        if (entry is ISchemaBase schema)
                        {
                            yield return schema;
                        }
                    }
                }
            }
        }

        public static DiffItemKey KeyOf(ISchemaBase item)
        {
            var schema = SchemaOf(item);

            return new DiffItemKey(item.ObjectType, schema, item.Name, item.Parent?.Name);
        }

        /// <summary>
        /// ObjectStatus to the four words of the specification: everything that is not Original, Create
        /// or Drop is an Alter, whichever specific kind of alteration it is.
        /// </summary>
        public static DiffStatus StatusOf(ObjectStatus status)
        {
            if (status == ObjectStatus.Original)
            {
                return DiffStatus.Identical;
            }

            if (status == ObjectStatus.Create)
            {
                return DiffStatus.Create;
            }

            if (status == ObjectStatus.Drop)
            {
                return DiffStatus.Drop;
            }

            return DiffStatus.Alter;
        }

        /// <summary>
        /// The schema a fully-qualified name carries, e.g. "[dbo].[Users]" or "[dbo].[Users].[IX_1]".
        /// </summary>
        private static string SchemaOf(ISchemaBase item)
        {
            var fullName = item.FullName ?? string.Empty;
            var start = fullName.IndexOf('[');

            if (start != 0)
            {
                return string.Empty;
            }

            var end = fullName.IndexOf(']');

            return end > start ? fullName.Substring(1, end - 1) : string.Empty;
        }

        private static string ToSql(ISchemaBase item)
        {
            if (item == null)
            {
                return null;
            }

            try
            {
                return item.ToSql();
            }
            catch (Exception ex)
            {
                return "-- The definition could not be read: " + ex.Message;
            }
        }

        private static string Script(ISchemaBase item)
        {
            try
            {
                return item.ToSqlDiff(new List<ISchemaBase> { item }).ToSQL();
            }
            catch (Exception ex)
            {
                return "-- The script for this object could not be generated: " + ex.Message;
            }
        }
    }
}
