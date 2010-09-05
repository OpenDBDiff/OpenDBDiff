using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLRichControl
{
    internal class Word:IComparable<Word>
    {
        public enum WordClassType
        {
            SQLWord = 1,
            FunctionWord = 2,
            StringWord = 3,
            NumberWord = 4,
            CommentWord = 5,
            OperatorWord = 6,
            SysTablesWord = 7,
            TypeWord = 8,
            SysProcWord = 9
        }

        private string text;
        private WordClassType type;
        private int len;

        public string Text
        {
            get { return text; }
            set 
            { 
                text = value;
                len = text.Length;
            }
        }

        public WordClassType Type
        {
            get { return type; }
            set { type = value; }
        }

        public int Size
        {
            get { return len; }
        }
        
        public bool IsOpenWordType
        {
            get
            {
                return (type == WordClassType.StringWord) || (type == WordClassType.CommentWord);
            }
        }

        public int CompareTo(Word other)
        {
            return text.CompareTo(other.text);
        }

    }
}
