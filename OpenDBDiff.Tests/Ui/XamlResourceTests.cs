using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenDBDiff.Tests.Ui
{
    /// <summary>
    /// Checks the XAML the way the WPF loader does, so a missing resource key fails here instead of
    /// throwing XamlParseException when a window is shown.
    ///
    /// This matters because the XAML compiler does not check StaticResource references at all: a key
    /// that does not exist builds cleanly on every leg and then crashes at runtime, which neither the
    /// build nor any other test here would catch.
    /// </summary>
    [TestClass]
    public class XamlResourceTests
    {
        private static readonly Regex KeyPattern = new Regex("x:Key=\"([^\"]+)\"", RegexOptions.Compiled);
        private static readonly Regex ReferencePattern = new Regex(@"\{StaticResource\s+([^}]+)\}", RegexOptions.Compiled);
        private static readonly Regex MergePattern = new Regex("Source=\"Themes/([^\"]+)\"", RegexOptions.Compiled);

        [TestMethod]
        public void EveryStaticResourceReferenceResolves()
        {
            var root = FindWpfProject();
            var appXaml = File.ReadAllText(Path.Combine(root, "App.xaml"));

            // What a view can see: the application's dictionaries, plus whatever the view defines itself.
            var appLevel = new HashSet<string>(
                Keys(appXaml).Concat(KeysOf(Directory.GetFiles(Path.Combine(root, "Themes"), "*.xaml"))),
                StringComparer.Ordinal);

            var unresolved = new List<string>();

            foreach (var file in Directory.GetFiles(root, "*.xaml", SearchOption.AllDirectories))
            {
                var name = Path.GetFileName(file);

                if (name == "App.xaml" || file.Contains(Path.DirectorySeparatorChar + "Themes" + Path.DirectorySeparatorChar))
                {
                    continue;
                }

                var text = File.ReadAllText(file);
                var local = Keys(text);

                foreach (Match match in ReferencePattern.Matches(text))
                {
                    var key = match.Groups[1].Value.Trim();

                    if (!appLevel.Contains(key) && !local.Contains(key))
                    {
                        unresolved.Add(Path.GetFileName(file) + " -> " + key);
                    }
                }
            }

            Assert.IsTrue(unresolved.Count == 0,
                "These StaticResource references have no definition, so the view throws XamlParseException when it loads: "
                + string.Join("; ", unresolved.Distinct()));
        }

        /// <summary>
        /// A dictionary can only see the dictionaries merged before it, so a reference to a key defined in
        /// a later one fails at load even though the key exists in the application as a whole.
        /// </summary>
        [TestMethod]
        public void EachMergedDictionaryResolvesAgainstItselfAndEarlierOnes()
        {
            var root = FindWpfProject();
            var appXaml = File.ReadAllText(Path.Combine(root, "App.xaml"));
            var mergeOrder = MergePattern.Matches(appXaml).Cast<Match>().Select(m => m.Groups[1].Value).ToList();

            Assert.IsTrue(mergeOrder.Count >= 3, "Expected the application to merge its theme dictionaries.");

            var keysByName = mergeOrder.ToDictionary(
                name => name,
                name => Keys(File.ReadAllText(Path.Combine(root, "Themes", name))));

            for (var i = 0; i < mergeOrder.Count; i++)
            {
                var name = mergeOrder[i];
                var visible = new HashSet<string>(
                    mergeOrder.Take(i + 1).SelectMany(other => keysByName[other]),
                    StringComparer.Ordinal);
                var text = File.ReadAllText(Path.Combine(root, "Themes", name));

                foreach (Match match in ReferencePattern.Matches(text))
                {
                    var key = match.Groups[1].Value.Trim();

                    Assert.IsTrue(visible.Contains(key),
                        name + " uses {StaticResource " + key + "}, which is not defined in it or in a dictionary merged before it.");
                }
            }
        }

        /// <summary>The application icon is what the file, the taskbar and the title bar show.</summary>
        [TestMethod]
        public void TheApplicationPublishesItsIcon()
        {
            var root = FindWpfProject();
            var project = File.ReadAllText(Path.Combine(root, "OpenDBDiff.Wpf.csproj"));

            Assert.IsTrue(project.Contains("<ApplicationIcon>App.ico</ApplicationIcon>"),
                "The project must set ApplicationIcon, otherwise the exe keeps the default .NET icon.");
            Assert.IsTrue(File.Exists(Path.Combine(root, "App.ico")),
                "App.ico is what both the file icon and the window icon use.");
            Assert.IsTrue(File.ReadAllText(Path.Combine(root, "Views", "MainWindow.xaml")).Contains("Icon=\"App.ico\""),
                "The window must set Icon, for the title bar and the taskbar entry.");
        }

        private static IEnumerable<string> Keys(string xaml)
        {
            return KeyPattern.Matches(xaml).Cast<Match>().Select(m => m.Groups[1].Value);
        }

        /// <summary>Keys of a set of XAML files. Keys() takes XAML text, not a path, so read first.</summary>
        private static IEnumerable<string> KeysOf(IEnumerable<string> files)
        {
            return files.SelectMany(file => Keys(File.ReadAllText(file)));
        }

        /// <summary>Walks up from the test output to the repository, so it works from any build directory.</summary>
        private static string FindWpfProject()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory != null)
            {
                var candidate = Path.Combine(directory.FullName, "OpenDBDiff.Wpf", "OpenDBDiff.Wpf.csproj");

                if (File.Exists(candidate))
                {
                    return Path.GetDirectoryName(candidate);
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException("Could not find OpenDBDiff.Wpf above " + AppContext.BaseDirectory);
        }
    }
}
