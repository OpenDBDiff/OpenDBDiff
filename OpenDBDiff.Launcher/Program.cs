using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace OpenDBDiff.Launcher
{
    /// <summary>
    /// Starts the front end, and says something useful when the desktop runtime it needs is missing.
    /// </summary>
    /// <remarks>
    /// A .NET application cannot report its own missing runtime: nothing managed runs without it, and
    /// what the apphost shows is terse enough that "the app does not open" is a fair description of it.
    /// This launcher is compiled ahead of time and brings its own runtime, so it can look for the
    /// desktop runtime and explain the situation before the user is left with a window that never
    /// appears. It is the entry point in the framework dependent download; the self-contained one does
    /// not need it.
    /// </remarks>
    internal static class Program
    {
        private const string FrameworkName = "Microsoft.WindowsDesktop.App";
        private const string FrontEnd = "OpenDBDiff.Wpf.exe";
        private const string DownloadUrl = "https://dotnet.microsoft.com/download/dotnet/10.0";

        /// <summary>The runtime the front end is built against: net10.0-windows targets 10.0.</summary>
        private const int RequiredMajorVersion = 10;

        private const uint MB_OK = 0x00000000;
        private const uint MB_YESNO = 0x00000004;
        private const uint MB_ICONWARNING = 0x00000030;
        private const uint MB_SETFOREGROUND = 0x00010000;
        private const int IDYES = 6;

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int MessageBox(IntPtr window, string text, string caption, uint type);

        [STAThread]
        private static int Main(string[] args)
        {
            var directory = AppContext.BaseDirectory;
            var frontEnd = Path.Combine(directory, FrontEnd);

            if (!File.Exists(frontEnd))
            {
                Warn("This launcher needs " + FrontEnd + " in the same folder, and it is not there.\r\n\r\n"
                     + directory);
                return 2;
            }

            var installed = FindDesktopRuntime(RequiredMajorVersion);

            if (installed == null)
            {
                var open = Confirm(
                    "OpenDBDiff needs the .NET " + RequiredMajorVersion + " Desktop Runtime, and this machine "
                    + "does not have it installed.\r\n\r\n"
                    + "OpenDBDiff is built for .NET " + RequiredMajorVersion + ", so it cannot start without it.\r\n\r\n"
                    + "Open the download page now? (" + DownloadUrl + ")");

                if (open)
                {
                    Open(DownloadUrl);
                }

                return 1;
            }

            return Start(frontEnd, directory, args);
        }

        private static int Start(string frontEnd, string directory, string[] args)
        {
            var start = new ProcessStartInfo(frontEnd)
            {
                WorkingDirectory = directory,
                UseShellExecute = true,
            };

            foreach (var argument in args)
            {
                start.ArgumentList.Add(argument);
            }

            try
            {
                Process.Start(start);
            }
            catch (Exception ex)
            {
                Warn("Could not start " + frontEnd + ".\r\n\r\n" + ex.Message);
                return 3;
            }

            return 0;
        }

        /// <summary>
        /// The newest installed desktop runtime of the given major version, or null when there is none.
        /// The front end alone decides which version it needs; this only answers whether there is one,
        /// so that a missing runtime is reported rather than a silently absent window.
        /// </summary>
        private static Version? FindDesktopRuntime(int majorVersion)
        {
            Version? newest = null;

            foreach (var root in DotNetRoots())
            {
                var shared = Path.Combine(root, "shared", FrameworkName);

                if (!Directory.Exists(shared))
                {
                    continue;
                }

                foreach (var installation in Directory.GetDirectories(shared))
                {
                    var name = Path.GetFileName(installation);
                    var suffix = name.IndexOf('-');

                    if (suffix > 0)
                    {
                        // A preview or release candidate reads as its version: 10.0.0-rc.2 is 10.0.0.
                        name = name.Substring(0, suffix);
                    }

                    if (Version.TryParse(name, out var version) && version.Major == majorVersion
                        && (newest == null || version > newest))
                    {
                        newest = version;
                    }
                }
            }

            return newest;
        }

        /// <summary>
        /// The folders a .NET installation can live in: the environment first, then the three layouts the
        /// installers use. The user-local one matters - installing .NET "for me only" puts it under
        /// LocalApplicationData, and missing it would report a runtime that is present as absent.
        /// </summary>
        private static IEnumerable<string> DotNetRoots()
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var fromEnvironment = new List<string?>
            {
                Environment.GetEnvironmentVariable("DOTNET_ROOT"),
                Environment.GetEnvironmentVariable("DOTNET_ROOT(x86)"),
            };

            foreach (var root in fromEnvironment)
            {
                if (!string.IsNullOrEmpty(root) && seen.Add(root!))
                {
                    yield return root!;
                }
            }

            var folders = new[] { Environment.SpecialFolder.ProgramFiles, Environment.SpecialFolder.ProgramFilesX86 };

            foreach (var folder in folders)
            {
                var programFiles = Environment.GetFolderPath(folder);

                if (!string.IsNullOrEmpty(programFiles))
                {
                    var dotnet = Path.Combine(programFiles, "dotnet");

                    if (seen.Add(dotnet))
                    {
                        yield return dotnet;
                    }
                }
            }

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            if (!string.IsNullOrEmpty(userProfile))
            {
                var userLocal = Path.Combine(userProfile, "Microsoft", "dotnet");

                if (seen.Add(userLocal))
                {
                    yield return userLocal;
                }
            }
        }

        private static void Open(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception)
            {
                // Nothing useful to add: the address is already in the message that was shown.
            }
        }

        private static bool Confirm(string text)
        {
            return MessageBox(IntPtr.Zero, text, "OpenDBDiff",
                MB_YESNO | MB_ICONWARNING | MB_SETFOREGROUND) == IDYES;
        }

        private static void Warn(string text)
        {
            MessageBox(IntPtr.Zero, text, "OpenDBDiff", MB_OK | MB_ICONWARNING | MB_SETFOREGROUND);
        }
    }
}
