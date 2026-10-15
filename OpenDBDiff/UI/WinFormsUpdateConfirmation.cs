using System.Windows.Forms;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Driver;

namespace OpenDBDiff.UI
{
    /// <summary>
    /// The WinForms answers to the questions <see cref="Updater"/> asks, kept identical to the
    /// MessageBox calls the update code used to make itself.
    /// </summary>
    public sealed class WinFormsUpdateConfirmation : IUpdateConfirmation
    {
        public static readonly WinFormsUpdateConfirmation Instance = new WinFormsUpdateConfirmation();

        public bool ConfirmAlter(ObjectType objectType, string name, string dataSource, string database)
        {
            return DialogResult.Yes == MessageBox.Show(
                string.Format("Alter {0} {1} in {2}..{3}?\n(WARNING: No automatic backup is made!)",
                    objectType, name, dataSource, database),
                "ALTER Destination?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Hand,
                MessageBoxDefaultButton.Button2);
        }

        public void InformScript(string script)
        {
            MessageBox.Show(script);
        }

        public void ReportError(string message)
        {
            MessageBox.Show(message);
        }
    }
}
