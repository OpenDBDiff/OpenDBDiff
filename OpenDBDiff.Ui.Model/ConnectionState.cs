using System;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// Where one side of the connection strip stands (section 6.1.3). The chip's status dot and the
    /// Compare enablement both read from this, so the two can never disagree.
    /// </summary>
    public enum ConnectionValidity
    {
        /// <summary>Missing host or database.</summary>
        Unset,

        /// <summary>Fields complete, not tested since the last edit.</summary>
        Untested,

        /// <summary>A test is in progress.</summary>
        Testing,

        /// <summary>The last test succeeded and the fields have not changed since.</summary>
        Valid,

        /// <summary>The last test failed.</summary>
        Failed
    }

    /// <summary>
    /// The connection strip's rules: validity transitions, the Compare enablement rule and chip text
    /// (sections 6.1.2 and 6.1.3).
    /// </summary>
    /// <remarks>
    /// These live here rather than in the view model because they are rules, not layout: section 17.1
    /// wants them exercisable without a UI, and the same transitions have to hold in both front ends.
    /// </remarks>
    public static class ConnectionRules
    {
        /// <summary>Section 6.1.2: the server is shown up to 24 characters, then ellipsised.</summary>
        public const int MaxServerChars = 24;

        public static bool IsComplete(string host, string database)
        {
            return !string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(database);
        }

        /// <summary>
        /// The state every edit lands in: an incomplete connection is Unset, a complete one drops back
        /// to Untested because the previous test no longer proves anything about the new fields.
        /// </summary>
        public static ConnectionValidity AfterEdit(string host, string database)
        {
            return IsComplete(host, database) ? ConnectionValidity.Untested : ConnectionValidity.Unset;
        }

        /// <summary>Section 6.1.3: Compare is enabled only when both sides are Valid.</summary>
        public static bool CanCompare(ConnectionValidity source, ConnectionValidity destination)
        {
            return source == ConnectionValidity.Valid && destination == ConnectionValidity.Valid;
        }

        /// <summary>
        /// Section 6.1.3: if both sides are Untested, Compare tests both in parallel and proceeds only
        /// if both succeed - otherwise it opens the panel on the failing side.
        /// </summary>
        public static bool ShouldTestBothBeforeCompare(ConnectionValidity source, ConnectionValidity destination)
        {
            return source == ConnectionValidity.Untested && destination == ConnectionValidity.Untested;
        }

        /// <summary>
        /// Whether the Compare button accepts a click.
        /// </summary>
        /// <remarks>
        /// Section 6.1.3 says Compare "is enabled only when both are Valid", and in the next sentence
        /// says that when both are Untested "clicking Compare first runs both tests in parallel". The
        /// two only hold together if the button is live in both states, so it is: two Valid sides
        /// compare straight away, two Untested sides test first. From any other combination a click
        /// could not do anything the user asked for, so the button is disabled and the panel is the
        /// way forward.
        /// </remarks>
        public static bool CanInvokeCompare(ConnectionValidity source, ConnectionValidity destination)
        {
            return CanCompare(source, destination) || ShouldTestBothBeforeCompare(source, destination);
        }

        /// <summary>The chip's tooltip, one sentence per state (section 6.1.3).</summary>
        public static string StatusDescription(ConnectionValidity validity)
        {
            switch (validity)
            {
                case ConnectionValidity.Unset:
                    return "Not set up: enter a server and a database.";
                case ConnectionValidity.Untested:
                    return "Not tested since the last edit.";
                case ConnectionValidity.Testing:
                    return "Testing the connection…";
                case ConnectionValidity.Valid:
                    return "Connection tested successfully.";
                default:
                    return "The last test failed. Open the panel to see why.";
            }
        }

        /// <summary>True when the state has a completed test result worth showing.</summary>
        public static bool HasError(ConnectionValidity validity)
        {
            return validity == ConnectionValidity.Failed;
        }

        /// <summary>Section 6.1.2: the server is never truncated below the cap; the database gives way first.</summary>
        public static string ServerDisplay(string server)
        {
            return Truncate(server, MaxServerChars);
        }

        /// <summary>Section 6.1.2: "SERVER / database", with the server capped and the database left to the chip.</summary>
        public static string ChipText(string server, string database)
        {
            if (string.IsNullOrWhiteSpace(server))
            {
                return "Not set";
            }

            if (string.IsNullOrWhiteSpace(database))
            {
                return ServerDisplay(server);
            }

            return ServerDisplay(server) + " / " + database;
        }

        /// <summary>The full text in the chip's tooltip, never truncated (section 6.1.2).</summary>
        public static string ChipTooltip(string server, string database)
        {
            if (string.IsNullOrWhiteSpace(server))
            {
                return "Not set up yet";
            }

            return string.IsNullOrWhiteSpace(database) ? server : server + " / " + database;
        }

        public static string Truncate(string text, int max)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= max)
            {
                return text ?? string.Empty;
            }

            return text.Substring(0, max) + "\u2026";
        }
    }
}
