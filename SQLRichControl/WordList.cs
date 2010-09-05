using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLRichControl
{
    internal class WordList:List<Word>
    {
        private Word word = new Word();

        public void Add(string text, Word.WordClassType type)
        {
            Word word = new Word();
            word.Text = text;
            word.Type = type;
            this.Add(word);
        }

        public Word this[string name]
        {
            get
            {
                word.Text = name;
                int index = base.BinarySearch(word);
                if (index >= 0)
                    return this[index];
                else
                    return null;
            }
        }
    }
}
