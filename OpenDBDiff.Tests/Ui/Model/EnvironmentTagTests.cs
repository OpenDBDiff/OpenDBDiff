using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// The environment tag of section 6.1.4: the six kinds, the eight-character custom limit and the
    /// Prod auto-suggestion, which must never apply itself.
    /// </summary>
    [TestClass]
    public class EnvironmentTagTests
    {
        [TestMethod]
        public void TheFixedKindsRenderUppercase()
        {
            Assert.AreEqual("DEV", EnvironmentTag.For(EnvironmentKind.Dev).Text);
            Assert.AreEqual("TEST", EnvironmentTag.For(EnvironmentKind.Test).Text);
            Assert.AreEqual("UAT", EnvironmentTag.For(EnvironmentKind.Uat).Text);
            Assert.AreEqual("PROD", EnvironmentTag.For(EnvironmentKind.Prod).Text);
            Assert.AreEqual(string.Empty, EnvironmentTag.None.Text);
            Assert.IsTrue(EnvironmentTag.None.IsNone);
        }

        [TestMethod]
        public void OnlyProdIsProduction()
        {
            Assert.IsTrue(EnvironmentTag.For(EnvironmentKind.Prod).IsProduction);
            Assert.IsFalse(EnvironmentTag.For(EnvironmentKind.Uat).IsProduction);
            Assert.IsFalse(EnvironmentTag.None.IsProduction);
        }

        [TestMethod]
        public void ACustomTagIsUppercasedTrimmedAndCappedAtEightCharacters()
        {
            Assert.AreEqual("STAGING", EnvironmentTag.Custom("staging").Text);
            Assert.AreEqual("STAGING", EnvironmentTag.Custom("  staging  ").Text);

            // Section 6.1.4: "custom (max 8 chars)".
            Assert.AreEqual("ABCDEFGH", EnvironmentTag.Custom("abcdefghijk").Text);

            // An empty custom tag is the same as no tag at all.
            Assert.IsTrue(EnvironmentTag.Custom("").IsNone);
            Assert.IsTrue(EnvironmentTag.Custom("   ").IsNone);
            Assert.IsTrue(EnvironmentTag.Custom(null).IsNone);
        }

        [TestMethod]
        public void ProdIsSuggestedWheneverEitherNameContainsProd()
        {
            // Section 6.1.4: "if a server host or database name contains prod (case-insensitive)".
            Assert.IsTrue(EnvironmentPalette.ShouldSuggest("SRV-PROD-01", "VivaLDI"));
            Assert.IsTrue(EnvironmentPalette.ShouldSuggest("srv012023", "VivaLDI_Prod"));
            Assert.IsTrue(EnvironmentPalette.ShouldSuggest("ProductionServer", "VivaLDI"));
            Assert.IsTrue(EnvironmentPalette.ShouldSuggest("x", "REPRODUCTION"));

            Assert.IsFalse(EnvironmentPalette.ShouldSuggest("srv012023", "VivaLDI"));
            Assert.IsFalse(EnvironmentPalette.ShouldSuggest(null, null));
            Assert.IsFalse(EnvironmentPalette.ShouldSuggest("", ""));

            Assert.AreEqual(EnvironmentKind.Prod, EnvironmentPalette.Suggest("srv-prod-01", "VivaLDI"));
            Assert.AreEqual(EnvironmentKind.None, EnvironmentPalette.Suggest("srv012023", "VivaLDI"));
        }

        [TestMethod]
        public void TagsCompareByKindAndText()
        {
            Assert.AreEqual(EnvironmentTag.For(EnvironmentKind.Prod), EnvironmentTag.For(EnvironmentKind.Prod));
            Assert.AreNotEqual(EnvironmentTag.For(EnvironmentKind.Prod), EnvironmentTag.For(EnvironmentKind.Test));

            Assert.AreEqual(EnvironmentTag.For(EnvironmentKind.Prod).GetHashCode(),
                EnvironmentTag.For(EnvironmentKind.Prod).GetHashCode());

            Assert.IsTrue(EnvironmentTag.Custom("staging").Equals(EnvironmentTag.Custom("STAGING")));
            Assert.IsFalse(EnvironmentTag.Custom("staging").IsNone);
        }

        [TestMethod]
        public void TheProductionTagIsTheAmberPairFromTheSpec()
        {
            // Section 6.1.4 gives these two values explicitly.
            Assert.AreEqual("#FEF0C7", EnvironmentPalette.Background(EnvironmentKind.Prod));
            Assert.AreEqual("#93370D", EnvironmentPalette.Foreground(EnvironmentKind.Prod));
        }
    }
}
