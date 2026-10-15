using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui
{
    /// <summary>
    /// Section 7.2's keyboard map against the keys the window actually handles.
    /// </summary>
    /// <remarks>
    /// The map's own comment says an advertised-but-unbound shortcut is a bug the tests can find, and until
    /// now nothing did: the overlay could claim a key that never reached a command and only a user would
    /// have noticed. The rows come from the model, the bindings come from the window's XAML as text, and
    /// every row has to be reachable one way or the other: a KeyBinding row in Window.InputBindings, a
    /// CodeBehind row mentioning its own key, and a Native row - the toolkit's own behaviour - exempt
    /// because there is nothing of ours to look for.
    /// </remarks>
    [TestClass]
    public class KeyboardMapTests
    {
        [TestMethod]
        public void EveryAdvertisedKeyBindingRowIsBoundInTheWindow()
        {
            var bound = WindowKeyBindings();

            foreach (var row in KeyboardMap.Rows.Where(row => row.Wiring == ShortcutWiring.KeyBinding))
            {
                Assert.IsTrue(bound.Contains(Chord(row)),
                    "the keyboard map advertises " + row.Chord + " (" + row.Action + ") as a key binding, "
                        + "but Window.InputBindings does not bind it");
            }
        }

        /// <summary>The other direction: a key that works but that the overlay never mentions.</summary>
        [TestMethod]
        public void EveryKeyBindingInTheWindowIsAdvertised()
        {
            var advertised = KeyboardMap.Rows
                .Where(row => row.Wiring == ShortcutWiring.KeyBinding)
                .Select(Chord)
                .ToList();

            foreach (var binding in WindowKeyBindings())
            {
                Assert.IsTrue(advertised.Contains(binding),
                    "the window binds " + binding + ", which section 7.2's map does not list");
            }
        }

        /// <summary>
        /// The keys the code-behind owns - because they move focus or depend on what is on screen - have to
        /// be somewhere in the code that handles keys.
        /// </summary>
        [TestMethod]
        public void EveryCodeBehindRowNamesItsKeyInTheCodeBehind()
        {
            var code = WindowCodeBehind() + ReviewListCodeBehind();

            foreach (var row in KeyboardMap.Rows.Where(row => row.Wiring == ShortcutWiring.CodeBehind))
            {
                StringAssert.Contains(code, "Key." + row.Key,
                    "the keyboard map advertises " + row.Chord + " (" + row.Action + ") as handled in code, "
                        + "but no code-behind looks at Key." + row.Key);
            }
        }

        /// <summary>
        /// The chord is what the overlay prints, so it has to be the row's own key and modifiers written the
        /// way a person says them. This is the rule that stops "Ctrl+Shift+X" being bound to Ctrl+X.
        /// </summary>
        [TestMethod]
        public void EveryChordMatchesItsOwnKeyAndModifiers()
        {
            foreach (var row in KeyboardMap.Rows.Where(row => row.Wiring != ShortcutWiring.Native))
            {
                Assert.AreEqual(Chord(row), row.Chord, row.Action + " is written one way and bound another");
            }
        }

        [TestMethod]
        public void NoTwoRowsClaimTheSameChord()
        {
            var duplicates = KeyboardMap.Rows
                .GroupBy(row => row.Chord, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            Assert.AreEqual(0, duplicates.Count, "claimed twice: " + string.Join(", ", duplicates));
        }

        [TestMethod]
        public void EveryRowSaysWhatItDoes()
        {
            foreach (var row in KeyboardMap.Rows)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(row.Action), row.Chord + " says nothing about what it does");
                Assert.IsFalse(string.IsNullOrWhiteSpace(row.Key), row.Chord + " names no key");
            }
        }

        /// <summary>The chord a row's key and modifiers add up to, as a person would write it.</summary>
        private static string Chord(ShortcutRow row)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(row.Modifiers))
            {
                parts.AddRange(row.Modifiers.Split(',').Select(part => part.Trim() == "Control" ? "Ctrl" : part.Trim()));
            }

            parts.Add(SaidKey(row.Key));

            return string.Join("+", parts);
        }

        /// <summary>The key as it is said rather than as WPF spells it.</summary>
        private static string SaidKey(string key)
        {
            switch (key)
            {
                case "D1": return "1";
                case "D2": return "2";
                case "D3": return "3";
                case "D4": return "4";
                case "D5": return "5";
                case "Escape": return "Esc";
                default: return key;
            }
        }

        /// <summary>
        /// The window's own InputBindings, as "Ctrl+Shift+X" strings.
        /// </summary>
        private static IReadOnlyList<string> WindowKeyBindings()
        {
            var text = Read("MainWindow.xaml");
            var block = Between(text, "<Window.InputBindings>", "</Window.InputBindings>");
            var bindings = new List<string>();

            foreach (Match match in Regex.Matches(block, "<KeyBinding\\b[^>]*/>"))
            {
                var key = Attribute(match.Value, "Key");
                var modifiers = Attribute(match.Value, "Modifiers");

                if (key == null)
                {
                    continue;
                }

                var parts = new List<string>();

                if (!string.IsNullOrEmpty(modifiers))
                {
                    parts.AddRange(modifiers.Split(',').Select(part => part.Trim() == "Control" ? "Ctrl" : part.Trim()));
                }

                parts.Add(SaidKey(key));
                bindings.Add(string.Join("+", parts));
            }

            return bindings;
        }

        private static string Attribute(string tag, string name)
        {
            var match = Regex.Match(tag, name + "=\"([^\"]*)\"");

            return match.Success ? match.Groups[1].Value : null;
        }

        private static string Between(string text, string start, string end)
        {
            var from = text.IndexOf(start, StringComparison.Ordinal);

            Assert.IsTrue(from >= 0, "the window has no " + start);

            var to = text.IndexOf(end, from, StringComparison.Ordinal);

            Assert.IsTrue(to > from, "the window's " + start + " is never closed");

            return text.Substring(from, to - from);
        }

        private static string WindowCodeBehind()
        {
            return Read("MainWindow.xaml.cs");
        }

        private static string ReviewListCodeBehind()
        {
            return Read("ReviewListView.xaml.cs");
        }

        private static string Read(string fileName)
        {
            return File.ReadAllText(Path.Combine(ViewsDirectory(), fileName));
        }

        private static string ViewsDirectory()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory != null)
            {
                var candidate = Path.Combine(directory.FullName, "OpenDBDiff.Wpf", "Views");

                if (Directory.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException("Could not find OpenDBDiff.Wpf\\Views above " + AppContext.BaseDirectory);
        }
    }
}
