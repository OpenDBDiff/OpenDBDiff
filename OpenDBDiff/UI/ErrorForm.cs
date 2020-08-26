using OpenDBDiff.Front.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace OpenDBDiff.UI
{
    public partial class ErrorForm : Form
    {
        private static Regex ExtractBuildPathRegex = new Regex($@"\s+at OpenDBDiff.Front.{nameof(ErrorForm)}.{nameof(GetBuildPath)}\(\) in (.*\\)OpenDBDiff\\Front\\ErrorForm.cs", RegexOptions.Compiled);
        private static Regex SystemExceptionsRegex = new Regex(@"\s+at System\.[^\r\n]+\r\n", RegexOptions.Compiled);
        private string ErrorInformation;
        private Exception Exception;
        private string SearchTerm;

        public ErrorForm()
        {
            InitializeComponent();
        }

        public ErrorForm(Exception ex) : this()
        {
            Exception = ex;
            LoadException(ex);
        }

        public void LoadException(Exception ex)
        {
            var exceptions = ex.FlattenHierarchy();
            var exceptionMessages = exceptions.Select(e => e.Message).ToList();
            var distinctMessages = exceptionMessages
                .Distinct()
                .OrderByDescending(m => exceptionMessages.IndexOf(m));

            var exceptionErrorMessage = new StringBuilder();
            exceptionErrorMessage.Append(this.Text);

            foreach (var message in distinctMessages.Skip(1))
                exceptionErrorMessage.Append($"\r\n{message}");

            var mostInnerException = exceptions.Last();

            string stackTrace = string.Empty;
            if (mostInnerException.StackTrace != null)
            {
                stackTrace = mostInnerException
                    .StackTrace;

                var buildPath = GetBuildPath();
                if (!string.IsNullOrEmpty(buildPath))
                    stackTrace = stackTrace.Replace(buildPath, string.Empty);

                stackTrace = SystemExceptionsRegex.Replace(stackTrace, string.Empty);
            }

            exceptionErrorMessage.Append($"\r\n{mostInnerException.GetType().Name}: {mostInnerException.Message}\r\n{stackTrace}");

            //var ignoreChunks = new System.Text.RegularExpressions.Regex(@": \[[^\)]*\)|\.\.\.\)|\'[^\']*\'|\([^\)]*\)|\" + '"' + @"[^\" + '"' + @"]*\" + '"' + @"|Source|Destination");

            int queryMaxLength = 280; //Bug in Github for searching issues? Q max length is 280
            var queryString = new StringBuilder();
            string orOperator = " OR ";
            foreach (var message in distinctMessages)
            {
                var roomLeft = queryMaxLength - queryString.Length;
                if (roomLeft > (orOperator.Length + 2 + message.Length))
                {
                    if (queryString.Length > 0)
                    {
                        queryString.Append(orOperator);
                    }
                    queryString.Append("\"");
                    queryString.Append(message);
                    queryString.Append("\"");
                }
            }
            string searchHash = GenerateHash(queryString.ToString());

            exceptionErrorMessage.AppendFormat("\r\n\r\n{0}", searchHash);

            ErrorInformation = string.Join("\r\n",
                "1.  To report an error search first in the Github issues to see if it's already been reported.",
                "2a. If there is no issue, click 'New issue' and paste the error details",
                "    into the body of the issue. To copy the error press \"Copy error\"",
                "    (At least email the details to opendbdiff@gmail.com)",
                "",
                "2b. If the issue exists, but your situation is different please leave a comment with the details.",
                "",
                "•   If possible, please attach a SQL script creating the two databases with",
                "    the minimum necessary to reproduce the problem.",
                ""
            ).Trim() + "\r\n\r\n" + exceptionErrorMessage.ToString();
            SearchTerm = queryString.ToString();

            this.ErrorInformationTextBox.Text = ErrorInformation;
        }

        private static string GenerateHash(string queryString)
        {
            var searchableErrorBytes = Encoding.UTF8.GetBytes(queryString);
            searchableErrorBytes = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(searchableErrorBytes);
            var searchHash = BitConverter.ToString(searchableErrorBytes).Replace("-", String.Empty);
            return searchHash;
        }

        private static string GetBuildPath()
        {
            try
            {
                throw new Exception("dummy");
            }
            catch (Exception ex)
            {
                var match = ExtractBuildPathRegex.Match(ex.StackTrace);
                if (match.Success)
                    return match.Groups[1].Value;
                else
                    return string.Empty;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(ErrorInformation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error while trying to copy the error to the clipboard: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFindIssue_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/OpenDBDiff/OpenDBDiff/issues?q=is%3Aissue+" + Uri.EscapeDataString(SearchTerm));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error while trying to search the error in the Github issues: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ErrorForm_Load(object sender, EventArgs e)
        {
            this.ErrorInformationTextBox.Text = ErrorInformation;
        }

        private void ReportIssueButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Have you searched Github to determine whether this error has already been reported?", "Searched Github", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    System.Diagnostics.Process.Start("https://github.com/OpenDBDiff/OpenDBDiff/issues/new");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Error while trying to create a new Github issue: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
