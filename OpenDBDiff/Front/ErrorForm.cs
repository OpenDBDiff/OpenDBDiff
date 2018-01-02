using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDBDiff.Front
{
    public partial class ErrorForm : Form
    {
        private string ErrorInformation;
        private string SearchTerm;
        private Exception Exception;

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
            var exceptionList = new List<Exception>();
            exceptionList.Add(ex);

            var innerException = ex.InnerException;
            while (innerException != null)
            {
                exceptionList.Insert(0, innerException);
                innerException = innerException.InnerException;

            }

            var exceptionMsg = new StringBuilder();
            var prevMessage = exceptionList[0].Message;
            exceptionMsg.Append(this.Text);
            for (int i = 1; i < exceptionList.Count; ++i)
            {
                if (exceptionList[i].Message != prevMessage)
                {
                    exceptionMsg.Append($"\r\n{exceptionList[i].Message}");
                    prevMessage = exceptionList[i].Message;
                }
            }

            var ignoreSystem = new System.Text.RegularExpressions.Regex(@"   at System\.[^\r\n]+\r\n|C:\\dev\\open-dbdiff\\");
            exceptionMsg.Append($"\r\n{exceptionList[0].GetType().Name}: {exceptionList[0].Message}\r\n{ignoreSystem.Replace(exceptionList[0].StackTrace, String.Empty)}");

            var ignoreChunks = new System.Text.RegularExpressions.Regex(@": \[[^\)]*\)|\.\.\.\)|\'[^\']*\'|\([^\)]*\)|\" + '"' + @"[^\" + '"' + @"]*\" + '"' + @"|Source|Destination");
            var searchableError = new StringBuilder();
            string joiner = " OR ";
            int queryMaxLength = 280; //Bug in github for searching issues? Q max length is 280
            foreach (var err in exceptionList)
            {
                var roomLeft = queryMaxLength - searchableError.Length;
                if (roomLeft > (joiner.Length + 2 + err.Message.Length))
                {
                    if (searchableError.Length > 0)
                    {
                        searchableError.Append(joiner);
                    }
                    searchableError.Append("\"");
                    searchableError.Append(err.Message);
                    searchableError.Append("\"");
                }
            }
            var searchableErrorBytes = Encoding.UTF8.GetBytes(searchableError.ToString());
            searchableErrorBytes = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(searchableErrorBytes);
            var searchHash = BitConverter.ToString(searchableErrorBytes).Replace("-", String.Empty);
            exceptionMsg.AppendFormat("\r\n\r\n{0}", searchHash);

            ErrorInformation = @"An unexpected error has occured during processing.

    1. To report an error please, search first in the github issues to see if it's already solved.
    2a. If there is no issue, *Please* click 'Create New Issue' and paste the error details
        into the Description field. To copy the error press _Copy Error_
        (At least email the details to opendbdiff@gmail.com...)

    2b. If the issue exists, but your situation is different please leave a comment with the details.

    • If possible, please attach a SQL script creating two dbs with
        the minimum necessary to reproduce the problem.
    
" + exceptionMsg.ToString();
            SearchTerm = searchableError.ToString();


            this.txtErrorInformation.Text = ErrorInformation;
        }

        private void ErrorForm_Load(object sender, EventArgs e)
        {
            this.txtErrorInformation.Text = ErrorInformation;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.Clipboard.SetText(ErrorInformation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while trying to copy the error to the clipboard: " + ex.Message);
            }
            finally
            {
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
                MessageBox.Show("Error while trying to search the error in the github issues: " + ex.Message);
            }
            finally
            {
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
