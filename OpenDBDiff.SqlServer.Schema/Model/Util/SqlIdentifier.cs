namespace OpenDBDiff.SqlServer.Schema.Model.Util
{
    internal static class SqlIdentifier
    {
        /// <summary>
        /// Wraps a name in brackets and doubles any closing bracket in it, so a name such as
        /// "IX_Photos [+Deleted]" becomes [IX_Photos [+Deleted]]] instead of ending early.
        /// </summary>
        public static string Quote(string name)
        {
            return "[" + name.Replace("]", "]]") + "]";
        }
    }
}
