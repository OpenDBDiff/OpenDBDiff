using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;

namespace OpenDBDiff.SqlServer.Driver
{
    /// <summary>
    /// The questions the update operations have to ask the user, and the things they have to tell
    /// them.
    /// </summary>
    /// <remarks>
    /// These used to be MessageBox calls inside the SQL, which is why the update code could only ever
    /// run in a WinForms process. Each front end supplies its own implementation.
    /// </remarks>
    public interface IUpdateConfirmation
    {
        /// <summary>
        /// Confirms altering one object in the destination database. Asked before every ALTER,
        /// because no automatic backup is taken.
        /// </summary>
        bool ConfirmAlter(ObjectType objectType, string name, string dataSource, string database);

        /// <summary>Shows a script that the user has to run by hand.</summary>
        void InformScript(string script);

        /// <summary>Reports a failure that has no return channel.</summary>
        void ReportError(string message);
    }
}
