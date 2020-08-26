using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Text.RegularExpressions;

namespace OpenDBDiff.SqlServer.Schema.Model.Util
{
    internal static class FormatCode
    {
        private static readonly char[] TrimCharacters = { ' ', '\r', '\n', '\t' };

        private class SearchItem
        {
            public int FindPosition;
            public string Body = "";
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
        /// Inserta la sentencia GO dentro del body
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

        /// <summary>
        /// Busca la primer entrada con el nombre completo dentro de una funcion, store, vista, trigger o rule.
        /// Ignora los comentarios.
        /// </summary>
        private static SearchItem FindCreate(string ObjectType, ISchemaBase item, string prevText)
        {
            var searchItem = new SearchItem();
            Regex regex = new Regex(@"((/\*)(\w|\s|\d|\[|\]|\.)*(\*/))|((\-\-)(.)*)", RegexOptions.IgnoreCase);
            Regex reg2 = new Regex(@"CREATE " + ObjectType + @"(\s|\r|\n|\t|\w|\/|\*|-|@|_|&|#)*((\[)?" + item.Owner + @"(\])?((\s)*)?\.)?((\s)*)?(\[)?" + item.Name + @"(\])?", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex reg3 = new Regex(@"((\[)?" + item.Owner + @"(\])?\.)?((\s)+\.)?(\s)*(\[)?" + item.Name + @"(\])?", RegexOptions.IgnoreCase);
            Regex reg4 = new Regex(@"( )*\[");
            //Regex reg3 = new Regex(@"((\[)?" + item.Owner + @"(\])?.)?(\[)?" + item.Name + @"(\])?", RegexOptions.Multiline);

            var matches = regex.Matches(prevText);
            Boolean finish = false;
            int indexStart = 0;
            int indexBegin = 0;
            int iAux = -1;

            while (!finish)
            {
                Match match = reg2.Match(prevText, indexBegin);
                if (match.Success)
                    iAux = match.Index;
                else
                    iAux = -1;
                if ((matches.Count == indexStart) || (match.Success))
                    finish = true;
                else
                {
                    if ((iAux < matches[indexStart].Index) || (iAux > matches[indexStart].Index + matches[indexStart].Length))
                        finish = true;
                    else
                    {
                        //indexBegin = abiertas[indexStart].Index + abiertas[indexStart].Length;
                        indexBegin = iAux + 1;
                        indexStart++;
                    }
                }
            }
            string result = reg3.Replace(prevText, " " + item.FullName, 1, iAux + 1);
            if (iAux != -1)
                searchItem.Body = reg4.Replace(result, " [", 1, iAux);
            searchItem.FindPosition = iAux;
            return searchItem;
        }

        public static string FormatCreate(string ObjectType, string body, ISchemaBase item)
        {
            try
            {
                string prevText = (string)body.Clone();
                prevText = FindCreate(ObjectType, item, prevText).Body;
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

        public static string FormatAlter(string ObjectType, string body, ISchemaBase item, Boolean quitSchemaBinding)
        {
            string prevText = null;
            try
            {
                prevText = (string)body.Clone();
                SearchItem sitem = FindCreate(ObjectType, item, prevText);
                Regex regAlter = new Regex("CREATE");

                if (!quitSchemaBinding)
                    return regAlter.Replace(sitem.Body, "ALTER", 1, sitem.FindPosition);
                //return prevText.Substring(0, iFind) + "ALTER " + sitem.ObjectType + " " + prevText.Substring(iFind + sitem.ObjectType.Length + 7, prevText.Length - (iFind + sitem.ObjectType.Length + 7)).TrimStart();
                else
                {
                    string text = regAlter.Replace(sitem.Body, "ALTER", 1, sitem.FindPosition);
                    Regex regex = new Regex("WITH SCHEMABINDING", RegexOptions.IgnoreCase);
                    return regex.Replace(text, "");
                }
                //return "";
            }
            catch
            {
                return prevText;
            }
        }
    }
}
