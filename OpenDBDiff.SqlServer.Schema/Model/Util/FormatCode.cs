using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Text.RegularExpressions;
using TSQL;
using TSQL.Tokens;

namespace OpenDBDiff.SqlServer.Schema.Model.Util
{
    internal static class FormatCode
    {
        private static readonly Regex RegCreateAlter = new Regex("CREATE", RegexOptions.Compiled);
        private static readonly Regex SchemaBindingRegex = new Regex("WITH SCHEMABINDING", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly char[] TrimCharacters = { ' ', '\r', '\n', '\t' };

        /// <summary>
        /// Find the first entry with the full name within a function, store, view, trigger or rule.
        /// Ignore comments.
        /// </summary>
        public static SearchItem FindAndNormalizeCreate(ISchemaBase item, string prevText)
        {
            var result = FindCreate(prevText) ?? throw new InvalidOperationException($"Could not find the CREATE statement for object '{item.Name}'");

            // normalize the object name for better comparison
            var body = prevText.Substring(0, result.TypeEndPosition + 1) + " " + item.FullName + prevText.Substring(result.NameEndPosition + 1);

            return new SearchItem { Body = body, FindPosition = result.CreateBeginPosition };
        }

        public static FindCreateStatementResult FindCreate(string body)
        {
            var tokenizer = new TSQLTokenizer(body);

            tokenizer.MoveNext();

            while (tokenizer.Current != null && tokenizer.Current.AsKeyword?.Keyword != TSQLKeywords.CREATE)
            {
                tokenizer.MoveNext();
            }

            if (tokenizer.Current == null)
                // Oops, we reached the end of the file and did not find the CREATE!
                return null;

            var createToken = tokenizer.Current.AsKeyword;

            tokenizer.MoveNext();

            var typeKeyword = tokenizer.Current.AsKeyword;

            tokenizer.MoveNext();

            // the object owner is optional
            var token0 = tokenizer.Current;
            tokenizer.MoveNext();
            var token1 = tokenizer.Current;
            tokenizer.MoveNext();
            var token2 = tokenizer.Current;
            tokenizer.MoveNext();

            TSQLIdentifier entityNameToken;
            if (token1.AsCharacter?.Text == ".")
            {
                entityNameToken = token2.AsIdentifier;
            }
            else
            {
                entityNameToken = token0.AsIdentifier;
            }

            return new FindCreateStatementResult
            {
                CreateBeginPosition = createToken.BeginPosition,
                TypeEndPosition = typeKeyword.EndPosition,
                NameEndPosition = entityNameToken.EndPosition
            };
        }

        public static string FormatAlter(string ObjectType, string body, ISchemaBase item, Boolean quitSchemaBinding)
        {
            string prevText = null;
            try
            {
                prevText = (string)body.Clone();
                SearchItem sitem = FindAndNormalizeCreate(item, prevText);

                if (!quitSchemaBinding)
                    return RegCreateAlter.Replace(sitem.Body, "ALTER", 1, sitem.FindPosition);
                //return prevText.Substring(0, iFind) + "ALTER " + sitem.ObjectType + " " + prevText.Substring(iFind + sitem.ObjectType.Length + 7, prevText.Length - (iFind + sitem.ObjectType.Length + 7)).TrimStart();
                else
                {
                    string text = RegCreateAlter.Replace(sitem.Body, "ALTER", 1, sitem.FindPosition);
                    return SchemaBindingRegex.Replace(text, "");
                }
                //return "";
            }
            catch
            {
                return prevText;
            }
        }

        public static string FormatCreate(string ObjectType, string body, ISchemaBase item)
        {
            try
            {
                string prevText = (string)body.Clone();
                prevText = FindAndNormalizeCreate(item, prevText).Body;
                if (String.IsNullOrEmpty(prevText))
                    prevText = body;
                prevText = CleanLast(prevText);
                return SmartGO(prevText);
            }
            catch
            {
                return SmartGO(CleanLast(body));
            }
        }

        /// <summary>
        /// Clears all unnecessary characters that are after the END statement of the body, and whitespaces at the beginning.
        /// </summary>
        private static string CleanLast(string body)
        {
            if (string.IsNullOrEmpty(body))
            {
                return string.Empty;
            }

            return body.TrimStart().TrimEnd(TrimCharacters);
        }

        /// <summary>
        /// Ensure statement ends with a GO statement
        /// </summary>
        private static string SmartGO(string code)
        {
            string prevText = code;
            try
            {
                if (!prevText.EndsWith("\r\n"))
                    prevText += "\r\n";
                return prevText + "GO\r\n";
            }
            catch
            {
                return prevText;
            }
        }

        internal struct SearchItem
        {
            public string Body;
            public int FindPosition;
        }

        internal class FindCreateStatementResult
        {
            public int CreateBeginPosition;
            public int NameEndPosition;
            public int TypeEndPosition;
        }
    }
}
