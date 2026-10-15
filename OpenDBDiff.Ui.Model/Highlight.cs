using System;
using System.Collections.Generic;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>A piece of a list row's text, marked when the search matched it (section 6.3).</summary>
    public sealed class HighlightRun
    {
        public HighlightRun(string text, bool isMatch)
        {
            Text = text ?? string.Empty;
            IsMatch = isMatch;
        }

        public string Text { get; }

        public bool IsMatch { get; }
    }

    /// <summary>
    /// Section 6.3's highlighting: "matches are highlighted in list rows (bold the matched substring;
    /// background highlight)".
    /// </summary>
    /// <remarks>
    /// The split is here rather than in the view for the usual reason: two views have to agree about what
    /// a match is, and the rule that a search is case-insensitive and may match several times already
    /// lives beside the filtering. What the view gets is the text cut into runs, so a row can be drawn as
    /// runs rather than as one string the view cannot mark up.
    /// </remarks>
    public static class Highlight
    {
        /// <summary>
        /// Cuts text into alternating unmatched and matched runs. The runs always reconstruct the text
        /// exactly, which is what lets a row show the file's own spelling in the search's own marking.
        /// </summary>
        public static IReadOnlyList<HighlightRun> Split(string text, string query)
        {
            var runs = new List<HighlightRun>();

            if (string.IsNullOrEmpty(text))
            {
                return runs;
            }

            var term = query == null ? string.Empty : query.Trim();

            if (term.Length == 0)
            {
                runs.Add(new HighlightRun(text, false));
                return runs;
            }

            var start = 0;

            while (start < text.Length)
            {
                var found = text.IndexOf(term, start, StringComparison.OrdinalIgnoreCase);

                if (found < 0)
                {
                    runs.Add(new HighlightRun(text.Substring(start), false));
                    break;
                }

                if (found > start)
                {
                    runs.Add(new HighlightRun(text.Substring(start, found - start), false));
                }

                runs.Add(new HighlightRun(text.Substring(found, term.Length), true));
                start = found + term.Length;
            }

            return runs;
        }
    }
}
