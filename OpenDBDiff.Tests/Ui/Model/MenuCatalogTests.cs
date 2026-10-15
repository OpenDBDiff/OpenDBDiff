using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// The menu bar and the legacy mapping of section 12. AC-12 says no legacy capability is lost, so
    /// this pins the menu structure and every row of the mapping table.
    /// </summary>
    [TestClass]
    public class MenuCatalogTests
    {
        [TestMethod]
        public void TheMenuBarHasTheSixMenusInOrder()
        {
            CollectionAssert.AreEqual(
                new[] { "File", "Compare", "View", "Script", "Tools", "Help" },
                MenuCatalog.Menus.ToArray());
        }

        [TestMethod]
        public void EveryMenuTheSpecListsHasItsItems()
        {
            var expected = new[]
            {
                "New project", "Open project\u2026", "Save project", "Save project as\u2026", "Recent projects", "Exit",
                "Compare", "Cancel", "Swap source and destination", "Schema", "Data", "Recent comparisons",
                "Show identical objects", "Group by type", "Flat list", "Side by side", "Inline", "Script", "Reset layout", "Theme",
                "Preview", "Copy", "Save as\u2026",
                "Options\u2026", "Open reports folder",
                "Keyboard shortcuts", "Documentation", "About"
            };

            foreach (var caption in expected)
            {
                Assert.IsTrue(MenuCatalog.Items.Any(item => item.Caption == caption), caption + " is missing from the menu bar");
            }
        }

        [TestMethod]
        public void EveryMenuItemBelongsToAKnownMenu()
        {
            foreach (var item in MenuCatalog.Items)
            {
                Assert.IsTrue(MenuCatalog.Menus.Contains(item.Menu), item.Caption + " is under an unknown menu " + item.Menu);
            }
        }

        [TestMethod]
        public void NoLegacyControlIsLost()
        {
            // AC-12: every old control from section 12's mapping table has a new location.
            foreach (var legacy in MenuCatalog.LegacyControls)
            {
                Assert.IsTrue(MenuCatalog.HasHomeFor(legacy), legacy + " has no home in the new UI");
            }
        }

        [TestMethod]
        public void EveryLegacyMappingNamesANewLocation()
        {
            foreach (var mapping in MenuCatalog.LegacyMappings)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(mapping.OldControl));
                Assert.IsFalse(string.IsNullOrWhiteSpace(mapping.NewLocation), mapping.OldControl + " has an empty destination");
            }
        }

        [TestMethod]
        public void TheShortcutsTheSpecNamesAreAdvertised()
        {
            Assert.AreEqual("F5", ShortcutOf("Compare"));
            Assert.AreEqual("Ctrl+O", ShortcutOf("Open project\u2026"));
            Assert.AreEqual("Ctrl+Enter", ShortcutOf("Preview"));
            Assert.AreEqual("Ctrl+Shift+C", ShortcutOf("Copy"));
            Assert.AreEqual("Ctrl+S", ShortcutOf("Save as\u2026"));
            Assert.AreEqual("F1", ShortcutOf("Keyboard shortcuts"));
        }

        [TestMethod]
        public void TheLegacyLookupOfAControlIsExact()
        {
            Assert.IsTrue(MenuCatalog.HasHomeFor("Compare data"));
            Assert.IsTrue(MenuCatalog.HasHomeFor("Update all"));
            Assert.IsFalse(MenuCatalog.HasHomeFor("Some control that never existed"));
        }

        private static string ShortcutOf(string caption)
        {
            foreach (var item in MenuCatalog.Items)
            {
                if (item.Caption == caption)
                {
                    return item.Shortcut;
                }
            }

            Assert.Fail(caption + " is not in the menu catalogue");
            return null;
        }
    }
}
