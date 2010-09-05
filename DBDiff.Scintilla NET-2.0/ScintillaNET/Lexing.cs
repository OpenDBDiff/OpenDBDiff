using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.IO;

namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class Lexing : ScintillaHelperBase
	{
		private const string DEFAULT_WORDCHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
		private const string DEFAULT_WHITECHARS = " \t\r\n\0";
		private Dictionary<string, int> _styleNameMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		private Dictionary<string, string> _lexerLanguageMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


		internal Lexing(Scintilla scintilla)
			: base(scintilla) 
		{
			WhiteSpaceChars = DEFAULT_WHITECHARS;
			WordChars = DEFAULT_WORDCHARS;
			_keywords = new KeywordCollection(scintilla);

			//	Language names are a superset lexer names. For instance the c and cs (c#)
			//	langauges both use the cpp lexer (by default). Languages are kind of a 
			//	SCite concept, while Scintilla only cares about Lexers. However we don't
			//	need to explicetly map a language to a lexer if they are the same name
			//	like cpp.
			_lexerLanguageMap.Add("cs", "cpp");
			_lexerLanguageMap.Add("html", "hypertext");
			_lexerLanguageMap.Add("xml", "hypertext");
		}

		internal bool ShouldSerialize()
		{
			return ShouldSerializeLexerName() ||
				ShouldSerializeLexer() ||
				ShouldSerializeWhiteSpaceChars() ||
				ShouldSerializeWordChars();
		}

		#region Lexer
		public Lexer Lexer
		{
			get
			{
				return (Lexer)NativeScintilla.GetLexer();
			}
			set
			{
				NativeScintilla.SetLexer((int)value);
				_lexerName = value.ToString().ToLower();
				if (_lexerName == "null")
					_lexerName = "";

				loadStyleMap();
			}
		}

		private bool ShouldSerializeLexer()
		{
			return Lexer != Lexer.Container;
		}

		private void ResetLexer()
		{
			Lexer = Lexer.Container;
		}

		#endregion

		#region Language
		private string _lexerName = "container";
		public string LexerName
		{
			get
			{
				return _lexerName;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
					value = "null";
					
				NativeScintilla.SetLexerLanguage(value.ToLower());

				_lexerName = value;

				loadStyleMap();
			}
		}

		private bool ShouldSerializeLexerName()
		{
			return LexerName != "container";
		}

		private void ResetLexerName()
		{
			LexerName = "container";
		} 
		#endregion

		private KeywordCollection _keywords;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public KeywordCollection Keywords
		{
			get
			{
				return _keywords;
			}
		}


		public void LoadLexerLibrary(string path)
		{
			NativeScintilla.LoadLexerLibrary(path);
		}

		public void Colorize(int startPos, int endPos)
		{
			NativeScintilla.Colourise(startPos, endPos);
		}

		public void Colorize()
		{
			Colorize(0, -1);
		}

		public string GetProperty(string name)
		{
			string s;
			NativeScintilla.GetProperty(name, out s);
			return s;
		}

		public void SetProperty(string name, string value)
		{
			NativeScintilla.SetProperty(name, value);
		}

		public string GetPropertyExpanded(string name)
		{
			string s;
			NativeScintilla.GetPropertyExpanded(name, out s);
			return s;
		}

		public int GetPropertyInt(string name)
		{
			return GetPropertyInt(name, 0);
		}

		public int GetPropertyInt(string name, int defaultValue)
		{
			return NativeScintilla.GetPropertyInt(name, defaultValue);
		}

		public void SetKeywords(int keywordSet, string list)
		{
			NativeScintilla.SetKeywords(keywordSet, list);
		}


		#region WordChars
		private string _wordChars;
		internal char[] WordCharsArr = null;
		public string WordChars
		{
			get
			{				
				return _wordChars;
			}
			set
			{
				_wordChars = value;
				WordCharsArr = _wordChars.ToCharArray();
				NativeScintilla.SetWordChars(value);
			}
		}

		private bool ShouldSerializeWordChars()
		{
			return _wordChars != DEFAULT_WORDCHARS;
		}

		private void ResetWordChars()
		{
			WordChars = DEFAULT_WORDCHARS;
		}

		#endregion

		#region WhiteSpaceChars
		internal char[] WhiteSpaceCharsArr;
		private string _whiteSpaceChars;
		[TypeConverter(typeof(ScintillaNet.WhiteSpaceStringConverter))]
		public string WhiteSpaceChars
		{
			get
			{
				return _whiteSpaceChars;
			}
			set
			{
				_whiteSpaceChars = value;
				WhiteSpaceCharsArr = _whiteSpaceChars.ToCharArray();
				NativeScintilla.SetWhitespaceChars(value);
			}
		}

		private bool ShouldSerializeWhiteSpaceChars()
		{
			return _whiteSpaceChars != DEFAULT_WHITECHARS;
		}

		private void ResetWhiteSpaceChars()
		{
			_whiteSpaceChars = DEFAULT_WHITECHARS;
		} 
		#endregion


		private void loadStyleMap()
		{
			if (Scintilla.IsDesignMode)
				return;

			_styleNameMap.Clear();

			//	These are global constants that always apply
			_styleNameMap.Add("BRACEBAD",Constants.STYLE_BRACEBAD);
			_styleNameMap.Add("BRACELIGHT",Constants.STYLE_BRACELIGHT);
			_styleNameMap.Add("CALLTIP",Constants.STYLE_CALLTIP);
			_styleNameMap.Add("CONTROLCHAR",Constants.STYLE_CONTROLCHAR);
			_styleNameMap.Add("DEFAULT",Constants.STYLE_DEFAULT);
			_styleNameMap.Add("LINENUMBER",Constants.STYLE_LINENUMBER);

			string lexname = this.Lexer.ToString().ToLower();

			using (Stream s = GetType().Assembly.GetManifestResourceStream("ScintillaNet.Configuration.Builtin.LexerStyleNames." + lexname + ".txt"))
			{
				if (s == null)
					return;

				using (StreamReader sr = new StreamReader(s))
				{
					while (!sr.EndOfStream)
					{
						string[] arr = sr.ReadLine().Split('=');
						if (arr.Length != 2)
							continue;

						string key = arr[0].Trim();
						int value = int.Parse(arr[1].Trim());
						
						_styleNameMap.Remove(key);
						_styleNameMap.Add(key, value);
					}
				}
			}

		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, int> StyleNameMap
		{
			get
			{
				return _styleNameMap;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, string> LexerLanguageMap
		{
			get
			{
				return _lexerLanguageMap;
			}
		}


		private string _lineCommentPrefix = string.Empty;
		public string LineCommentPrefix
		{
			get
			{
				return _lineCommentPrefix;
			}
			set
			{
				if (value == null)
					value = string.Empty;

				_lineCommentPrefix = value;
			}
		}

		private string _streamCommentPrefix = string.Empty;
		public string StreamCommentPrefix
		{
			get
			{
				return _streamCommentPrefix;
			}
			set
			{
				if (value == null)
					value = string.Empty;

				_streamCommentPrefix = value;
			}
		}

		private string _streamCommentSufix = string.Empty;
		public string StreamCommentSufix
		{
			get
			{
				return _streamCommentSufix;
			}
			set
			{
				if (value == null)
					value = string.Empty;

				_streamCommentSufix = value;
			}
		}

		public void LineComment()
		{
			if (string.IsNullOrEmpty(_lineCommentPrefix))
				return;

			//	So the theory behind line commenting is that for every selected line
			//	we look for the first non-whitespace character and insert the line
			//	comment prefix. Lines without non-whitespace are skipped.
			NativeScintilla.BeginUndoAction();

			Range selRange = Scintilla.Selection.Range;
			int start = selRange.StartingLine.Number;
			int end = selRange.EndingLine.Number;

			//	We're tracking the new end of the selection range including
			//	the amount it expands because we're inserting new text.
			int offset = _lineCommentPrefix.Length;

			for (int i = start; i <= end; i++)
			{
				Line l = Scintilla.Lines[i];
				int firstWordChar = findFirstNonWhitespaceChar(l.Text);
				if (firstWordChar >= 0)
				{
					Scintilla.InsertText(l.StartPosition + firstWordChar, _lineCommentPrefix);
					selRange.End += offset;
				}
			}

			NativeScintilla.EndUndoAction();

			//	An odd side-effect of InsertText is that we lose the current
			//	selection. This is undesirable. This is why we were tracking
			//	the end position offset.
			selRange.Select();
		}

		public void LineUncomment()
		{
			if (string.IsNullOrEmpty(_lineCommentPrefix))
				return;

			NativeScintilla.BeginUndoAction();

			//	Uncommenting is a lot like line commenting. However in addition
			//	to looking for a non-whitespace character, the string that follows
			//	it MUST be our line Comment Prefix. If this is the case the prefex
			//	is removed from the line at its position.
			Range selRange = Scintilla.Selection.Range;
			int start = selRange.StartingLine.Number;
			int end = selRange.EndingLine.Number;

			int offset = _lineCommentPrefix.Length;

			for (int i = start; i <= end; i++)
			{
				Line l = Scintilla.Lines[i];
				int firstWordChar = findFirstNonWhitespaceChar(l.Text);
				if (firstWordChar >= 0)
				{
					int startPos = l.StartPosition + firstWordChar;
					Range commentRange = Scintilla.GetRange(startPos, startPos + offset);
					if (commentRange.Text == _lineCommentPrefix)
						commentRange.Text = string.Empty;
				}
			}

			NativeScintilla.EndUndoAction();
		}

		public void ToggleLineComment()
		{
			if (string.IsNullOrEmpty(_lineCommentPrefix))
				return;

			NativeScintilla.BeginUndoAction();

			Range selRange = Scintilla.Selection.Range;
			int start = selRange.StartingLine.Number;
			int end = selRange.EndingLine.Number;

			int offset = _lineCommentPrefix.Length;

			for (int i = start; i <= end; i++)
			{
				Line l = Scintilla.Lines[i];
				int firstWordChar = findFirstNonWhitespaceChar(l.Text);
				if (firstWordChar >= 0)
				{
					int startPos = l.StartPosition + firstWordChar;
					Range commentRange = Scintilla.GetRange(startPos, startPos + offset);
					if (commentRange.Text == _lineCommentPrefix)
					{
						commentRange.Text = string.Empty;
						selRange.End -= offset;
					}
					else
					{
						Scintilla.InsertText(l.StartPosition + firstWordChar, _lineCommentPrefix);
						selRange.End += offset;
					}
				}
			}

			NativeScintilla.EndUndoAction();
			selRange.Select();
		}



		private int findFirstNonWhitespaceChar(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i].ToString().IndexOfAny(WhiteSpaceCharsArr) == -1)
					return i;
			}

			return -1;
		}

		public void StreamComment()
		{
			if (string.IsNullOrEmpty(_streamCommentPrefix) || string.IsNullOrEmpty(_streamCommentSufix))
				return;

			NativeScintilla.BeginUndoAction();
			
			Range selRange = Scintilla.Selection.Range;
			Scintilla.InsertText(selRange.Start, _streamCommentPrefix);
			Scintilla.InsertText(selRange.End+ _streamCommentPrefix.Length, _streamCommentSufix);
			selRange.End += _streamCommentPrefix.Length + _streamCommentSufix.Length;
			selRange.Select();

			NativeScintilla.EndUndoAction();
		}
	}


	public class KeywordCollection : ScintillaHelperBase
	{
		private Dictionary<string, string[]> _lexerKeywordListMap;
		private Dictionary<string, Dictionary<string, int>> _lexerStyleMap;
		private Dictionary<string, Lexer> _lexerAliasMap;

		internal KeywordCollection(Scintilla scintilla)
			: base(scintilla) 
		{
			//	Auugh, this plagued me for a while. Each of the lexers cna define their own "Name"
			//	and also asign themsleves to a Scintilla Lexer Constant. Most of the time these
			//	match the defined constant, but sometimes they don't. We'll always use the constant
			//	name since it's easier to use, consistent and will always have valid characters.
			//	However its still valid to access the lexers by this name (as SetLexerLanguage
			//	uses this value) so we'll create a lookup.
			_lexerAliasMap = new Dictionary<string, Lexer>(StringComparer.OrdinalIgnoreCase);
			
			//	I have no idea how Progress fits into this. It's defined with the PS lexer const
			//	and a name of "progress"

			_lexerAliasMap.Add("PL/M", Lexer.Plm);
			_lexerAliasMap.Add("props", Lexer.Properties);
			_lexerAliasMap.Add("inno", Lexer.InnoSetup);
			_lexerAliasMap.Add("clarion", Lexer.Clw);
			_lexerAliasMap.Add("clarionnocase", Lexer.ClwNoCase	);


			//_lexerKeywordListMap = new Dictionary<string,string[]>(StringComparer.OrdinalIgnoreCase);


			//_lexerKeywordListMap.Add("xml", new string[] { "HTML elements and attributes", "JavaScript keywords", "VBScript keywords", "Python keywords", "PHP keywords", "SGML and DTD keywords" });
			//_lexerKeywordListMap.Add("yaml", new string[] { "Keywords" });
			//	baan, kix, ave, scriptol, diff, props, makefile, errorlist, latex, null, lot, haskell
			//	lexers don't have keyword list names

			
		}

	
		private string[] _keywords = new string[] { "", "", "", "", "", "", "", "", "" };
		
		public string this[int keywordSet]
		{
			get
			{
				return _keywords[keywordSet];
			}
			set
            {
				_keywords[keywordSet] = value;
				NativeScintilla.SetKeywords(keywordSet, value);
            }
		}

		public string this[string keywordSetName]
		{
			get
			{
				return this[getKeyowrdSetIndex(keywordSetName)];
			}
			set
			{
				this[getKeyowrdSetIndex(keywordSetName)] = value;
			}
		}

		private int getKeyowrdSetIndex(string name)
		{
			string lexerName = Scintilla.Lexing.Lexer.ToString().ToLower();
			if (_lexerKeywordListMap.ContainsKey(lexerName))
				throw new ApplicationException("lexer " + lexerName + " does not support named keyword lists");

			int index = ((IList)_lexerKeywordListMap[lexerName]).IndexOf(name);

			if (index < 0)
				throw new ArgumentException("Keyword Set name does not exist for lexer " + lexerName, "keywordSetName");

			return index;
		}
	}
}


