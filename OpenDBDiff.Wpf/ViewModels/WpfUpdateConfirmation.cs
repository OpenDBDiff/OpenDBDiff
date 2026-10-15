using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Driver;
using OpenDBDiff.SqlServer.Schema.Options;
using OpenDBDiff.Wpf.Infrastructure;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// The answers a WPF window gives to the questions <see cref="Updater"/> asks while it changes a
    /// database, and an error report for the cases with no return channel.
    /// </summary>
    public sealed class WpfUpdateConfirmation : IUpdateConfirmation
    {
        private readonly Services.IDialogService dialogs;

        public WpfUpdateConfirmation(Services.IDialogService dialogs)
        {
            this.dialogs = dialogs;
        }

        public bool ConfirmAlter(ObjectType objectType, string name, string dataSource, string database)
        {
            return dialogs.Confirm(
                string.Format("Alter {0} {1} in {2}..{3}?\n(WARNING: No automatic backup is made!)",
                    objectType, name, dataSource, database),
                "ALTER Destination?");
        }

        public void InformScript(string script)
        {
            dialogs.Inform("Script", script);
        }

        public void ReportError(string message)
        {
            dialogs.Inform("Error", message);
        }
    }
}
