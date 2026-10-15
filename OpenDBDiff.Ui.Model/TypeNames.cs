using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using OpenDBDiff.Abstractions.Schema;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// How an object type is written in the interface. The schema model already carries its display
    /// names as Description attributes, so those are the singular names and this class only has to
    /// pluralise them for the group headers.
    /// </summary>
    public static class TypeNames
    {
        private static readonly Dictionary<ObjectType, string> IrregularPlurals = new Dictionary<ObjectType, string>
        {
            { ObjectType.FullText, "Full Text" },
            { ObjectType.FullTextIndex, "Full Text Indexes" },
            { ObjectType.XMLSchema, "XML Schemas" },
            { ObjectType.TableType, "Table Types" },
            { ObjectType.Permission, "Permissions" }
        };

        public static string Singular(ObjectType objectType)
        {
            var description = DescriptionOf(objectType);

            return description ?? Split(objectType.ToString());
        }

        public static string Plural(ObjectType objectType)
        {
            if (IrregularPlurals.TryGetValue(objectType, out var irregular))
            {
                return irregular;
            }

            var singular = Singular(objectType);
            var lastWord = singular.LastWord();

            if (lastWord.EndsWith("y", StringComparison.OrdinalIgnoreCase)
                && lastWord.Length > 1
                && !"aeiou".Contains(lastWord.Substring(lastWord.Length - 2, 1).ToLowerInvariant()))
            {
                return singular.Substring(0, singular.Length - 1) + "ies";
            }

            if (lastWord.EndsWith("s", StringComparison.OrdinalIgnoreCase)
                || lastWord.EndsWith("x", StringComparison.OrdinalIgnoreCase)
                || lastWord.EndsWith("ch", StringComparison.OrdinalIgnoreCase))
            {
                return singular + "es";
            }

            return singular + "s";
        }

        /// <summary>"UserDataType" becomes "User Data Type" for a type without a Description.</summary>
        private static string Split(string name)
        {
            var builder = new StringBuilder();

            foreach (var character in name)
            {
                if (char.IsUpper(character) && builder.Length > 0)
                {
                    builder.Append(' ');
                }

                builder.Append(character);
            }

            return builder.ToString();
        }

        private static string DescriptionOf(ObjectType objectType)
        {
            var field = objectType.GetType().GetField(objectType.ToString());

            if (field == null)
            {
                return null;
            }

            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? ((DescriptionAttribute)attributes[0]).Description : null;
        }

        private static string LastWord(this string text)
        {
            var index = text.LastIndexOf(' ');

            return index < 0 ? text : text.Substring(index + 1);
        }
    }
}
