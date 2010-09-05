using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace ScintillaNet.Configuration
{
	public class Configuration
	{
		private string _language;
		public string Language
		{
			get
			{
				return _language;
			}
			set
			{
				_language = value;
			}
		}

		#region Constructor
		public Configuration(string language)
		{
			_language = language;
		}



		public Configuration(XmlDocument configDocument, string language)
		{
			_language = language;
			Load(configDocument);
		}

		public Configuration(string fileName, string language, bool useXmlReader)
		{
			_language = language;
			Load(fileName, useXmlReader);
		}


		public Configuration(Stream inStream, string language, bool useXmlReader)
		{
			_language = language;
			Load(inStream, useXmlReader);
		}

		public Configuration(TextReader txtReader, string language)
		{
			_language = language;
			Load(txtReader);
		}

		public Configuration(XmlReader reader, string language)
		{
			_language = language;
			Load(reader);
		}

		#endregion

		#region Load
		public void Load(string fileName, bool useXmlReader)
		{
			if (useXmlReader)
			{
				XmlReaderSettings s = new XmlReaderSettings();
				s.IgnoreComments = true;
				s.IgnoreWhitespace = true;

				Load(XmlReader.Create(fileName, s));
			}
			else
			{
				XmlDocument doc = new XmlDocument();
				doc.PreserveWhitespace = true;
				doc.Load(fileName);

				Load(doc);
			}
		}

		public void Load(Stream inStream, bool useXmlReader)
		{
			if (useXmlReader)
			{
				XmlReaderSettings s = new XmlReaderSettings();
				s.IgnoreComments = true;
				s.IgnoreWhitespace = true;

				Load(XmlReader.Create(inStream, s));
			}
			else
			{
				XmlDocument doc = new XmlDocument();
				doc.PreserveWhitespace = true;
				doc.Load(inStream);
				Load(doc);
			}
		}

		public void Load(TextReader txtReader)
		{
			XmlDocument configDocument = new XmlDocument();
			configDocument.PreserveWhitespace = true;
			configDocument.Load(txtReader);
			Load(configDocument);
		}

		public void Load(XmlReader reader)
		{
			reader.ReadStartElement();

			while (!reader.EOF)
			{
				if (reader.Name.Equals("language", StringComparison.OrdinalIgnoreCase) && reader.HasAttributes)
				{

					while (reader.MoveToNextAttribute())
					{
						if (reader.Name.Equals("name", StringComparison.OrdinalIgnoreCase) && reader.Value.Equals(_language, StringComparison.OrdinalIgnoreCase))
							readLanguage(reader);
					}
				}

				if (reader.NodeType == XmlNodeType.EndElement)
					reader.Read();
			}
		}

		private void readLanguage(XmlReader reader)
		{
			_commands_KeyBindingList = new CommandBindingConfigList();
			_lexing_Properties = new LexerPropertiesConfig();
			_lexing_Keywords = new KeyWordConfigList();
			_margin_List = new MarginConfigList();
			_markers_List = new MarkersConfigList();			

			reader.Read();
			while (reader.NodeType == XmlNodeType.Element)
			{
				string elName = reader.Name.ToLower();
				switch (elName)
				{
					case "calltip":
						readCallTip(reader);
						break;
					case "caret":
						readCaret(reader);
						break;
					case "clipboard":
						readClipboard(reader);
						break;
					case "commands":
						readCommands(reader);
						break;
					case "endofline":
						readEndOfLine(reader);
						break;
					case "hotspot":
						readHotSpot(reader);
						break;
					case "indentation":
						readIndentation(reader);
						break;
					case "indicators":
						readIndicators(reader);
						break;
					case "lexer":
						readLexer(reader);
						break;
					case "linewrap":
						readLineWrap(reader);
						break;
					case "longlines":
						readLongLines(reader);
						break;
					case "margins":
						readMargins(reader);
						break;
					case "markers":
						readMarkers(reader);
						break;
					case "scrolling":
						readScrolling(reader);
						break;
					case "selection":
						readSelection(reader);
						break;
					case "styles":
						readStyles(reader);
						break;
					case "undoredo":
						readUndoRedo(reader);
						break;
					case "whiteSpace":
						readWhiteSpace(reader);
						break;
					default:
						reader.Skip();
						break;
				}
				
			}
		}

		private void readWhiteSpace(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "backcolor":
							_whiteSpace_BackColor = getColor(reader.Value);
							break;
						case "forecolor":
							_whiteSpace_ForeColor = getColor(reader.Value);
							break;
						case "mode":
							_whiteSpace_Mode = (WhiteSpaceMode)Enum.Parse(typeof(WhiteSpaceMode), reader.Value, true);
							break;
						case "usewhitespacebackcolor":
							_whiteSpace_UseWhiteSpaceBackColor = getBool(reader.Value);
							break;
						case "usewhitespaceforecolor":
							_whiteSpace_UseWhiteSpaceForeColor = getBool(reader.Value);
							break;
					}
				}
			}
			reader.Skip();
		}

		private StyleConfig getStyleConfigFromElement(XmlReader reader)
		{
			StyleConfig sc = new StyleConfig();
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "name":
							sc.Name = reader.Value;
							break;
						case "number":
							sc.Number = getInt(reader.Value);
							break;
						case "backcolor":
							sc.BackColor = getColor(reader.Value);
							break;
						case "bold":
							sc.Bold = getBool(reader.Value);
							break;
						case "case":
							sc.Case = (StyleCase)Enum.Parse(typeof(StyleCase), reader.Value, true);
							break;
						case "characterset":
							sc.CharacterSet = (CharacterSet)Enum.Parse(typeof(CharacterSet), reader.Value, true);
							break;
						case "fontname":
							sc.FontName = reader.Value;
							break;
						case "forecolor":
							sc.ForeColor = getColor(reader.Value);
							break;
						case "ischangeable":
							sc.IsChangeable = getBool(reader.Value);
							break;
						case "ishotspot":
							sc.IsHotspot = getBool(reader.Value);
							break;
						case "isselectioneolfilled":
							sc.IsSelectionEolFilled = getBool(reader.Value);
							break;
						case "isvisible":
							sc.IsVisible = getBool(reader.Value);
							break;
						case "italic":
							sc.Italic = getBool(reader.Value);
							break;
						case "size":
							sc.Size = getInt(reader.Value);
							break;
						case "underline":
							sc.Underline = getBool(reader.Value);
							break;
						case "inherit":
							sc.Inherit = getBool(reader.Value);
							break;
					}
				}
				reader.MoveToElement();
			}

			return sc;
        }

		private void readStyles(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
					if (reader.Name.ToLower() == "bits")
						_undoRedoIsUndoEnabled = getBool(reader.Value);

				reader.MoveToElement();
			}

			if (!reader.IsEmptyElement)
			{
				while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("styles", StringComparison.OrdinalIgnoreCase)))
				{
					reader.Read();
					if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("style", StringComparison.OrdinalIgnoreCase))
					{
						_styles.Add(getStyleConfigFromElement(reader));
					}
					else if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("sublanguage", StringComparison.OrdinalIgnoreCase))
					{
						readSubLanguage(reader);
					}
				}
			}

			reader.Read();
		}

		private void readSubLanguage(XmlReader reader)
		{
			//	This is a nifty added on hack made specifically for HTML.
			//	Normally the style config elements are quite managable as there
			//	are typically less than 10 when you don't count common styles.
			//	
			//	However HTML uses 9 different Sub languages that combined make 
			//	use of all 128 styles (well there are some small gaps). In order
			//	to make this more managable I did added a SubLanguage element that
			//	basically just prepends the Language's name and "." to the Style 
			//	Name definition.
			//
			//	So for example if you had the following
			//	<Styles>
			//		<SubLanguage Name="ASP JavaScript">
			//			<Style Name="Keyword" Bold="True" />
			//		</SubLanguage>
			//	</Styles>
			//	That style's name will get interpreted as "ASP JavaScript.Keyword".
			//	which if you look at the html.txt in LexerStyleNames you'll see it
			//	maps to Style # 62
			string subLanguageName = string.Empty;
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
					if (reader.Name.ToLower() == "name")
						subLanguageName = reader.Value;

				reader.MoveToElement();
			}

			if (!reader.IsEmptyElement)
			{
				while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("sublanguage", StringComparison.OrdinalIgnoreCase)))
				{
					reader.Read();
					if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("style", StringComparison.OrdinalIgnoreCase))
					{
						StyleConfig sc = getStyleConfigFromElement(reader);
						sc.Name = subLanguageName + "." + sc.Name;
						_styles.Add(sc);
					}
				}
			}

			reader.Read();
		}
		

		private void readUndoRedo(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
					if (reader.Name.ToLower() == "isundoenabled")
						_undoRedoIsUndoEnabled = getBool(reader.Value);

				reader.MoveToElement();
			}
			reader.Skip();
		}

		private void readSelection(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "backcolor":
							_selection_BackColor = getColor(reader.Value);
							break;
						case "backcolorunfocused":
							_selection_BackColorUnfocused = getColor(reader.Value);
							break;
						case "forecolor":
							_selection_ForeColor = getColor(reader.Value);
							break;
						case "forecolorunfocused":
							_selection_ForeColorUnfocused = getColor(reader.Value);
							break;
						case "hidden":
							_selection_Hidden = getBool(reader.Value);
							break;
						case "hideselection":
							_selection_HideSelection = getBool(reader.Value);
							break;
						case "mode":
							_selection_Mode = (SelectionMode)Enum.Parse(typeof(SelectionMode), reader.Value, true);
							break;
					}
				}
				reader.MoveToElement();
			}

			reader.Skip();
		}

		private void readScrolling(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "endatlastline":
							_scrolling_EndAtLastLine = getBool(reader.Value);
							break;
						case "horizontalwidth":
							_scrolling_HorizontalWidth = getInt(reader.Value);
							break;
						case "scrollbars":
							string flags = reader.Value.Trim();
							if (flags != string.Empty)
							{
								ScrollBars? sb = null;
								foreach (string flag in flags.Split(' '))
									sb |= (ScrollBars)Enum.Parse(typeof(ScrollBars), flag.Trim(), true);

								if (sb.HasValue)
									_scrolling_ScrollBars = sb;
							}
							break;
						case "xoffset":
							_scrolling_XOffset = getInt(reader.Value);
							break;
					}
				}
				reader.MoveToElement();
			}

			reader.Skip();
		}

		private void readMarkers(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
					if(reader.Name.ToLower() == "inherit")
						_markers_List.Inherit = getBool(reader.Value);

				reader.MoveToElement();
			}

			if (!reader.IsEmptyElement)
			{
				while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("markers", StringComparison.OrdinalIgnoreCase)))
				{
					reader.Read();
					if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("marker", StringComparison.OrdinalIgnoreCase))
					{
						if (reader.HasAttributes)
						{
							MarkersConfig mc = new MarkersConfig();
							while (reader.MoveToNextAttribute())
							{
								string attrName = reader.Name.ToLower();
								switch (attrName)
								{
									case "alpha":
										mc.Alpha = getInt(reader.Value);
										break;
									case "backcolor":
										mc.BackColor = getColor(reader.Value);
										break;
									case "forecolor":
										mc.ForeColor = getColor(reader.Value);
										break;
									case "name":
										mc.Name = reader.Value;
										break;
									case "number":
										mc.Number = getInt(reader.Value);
										break;
									case "inherit":
										mc.Inherit = getBool(reader.Value);
										break;
									case "symbol":
										mc.Symbol = (MarkerSymbol)Enum.Parse(typeof(MarkerSymbol), reader.Value, true);
										break;
								}
							}
							
							reader.MoveToElement();
							_markers_List.Add(mc);
						}
					}
				}
			}
			reader.Read();
		}

		private void readMargins(XmlReader reader)
		{			
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "foldmargincolor":
							_margin_List.FoldMarginColor = getColor(reader.Value);
							break;
						case "foldmarginhighlightcolor":
							_margin_List.FoldMarginHighlightColor = getColor(reader.Value);
							break;
						case "left":
							_margin_List.Left = getInt(reader.Value);
							break;
						case "right":
							_margin_List.Right = getInt(reader.Value);
							break;
						case "inherit":
							_margin_List.Inherit = getBool(reader.Value);
							break;
					}
				}
				reader.MoveToElement();
			}
			
			
			if (!reader.IsEmptyElement)
			{
				while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("margins", StringComparison.OrdinalIgnoreCase)))
				{
					reader.Read();
					if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("margin", StringComparison.OrdinalIgnoreCase))
					{
						if (reader.HasAttributes)
						{
							MarginConfig mc = new MarginConfig();
							while (reader.MoveToNextAttribute())
							{
								string attrName = reader.Name.ToLower();
								switch (attrName)
								{
									case "number":
										mc.Number = int.Parse(reader.Value);
										break;
									case "inherit":
										mc.Inherit = getBool(reader.Value);
										break;
									case "autotogglemarkernumber":
										mc.AutoToggleMarkerNumber = getInt(reader.Value);
										break;
									case "isclickable":
										mc.IsClickable = getBool(reader.Value);
										break;
									case "isfoldmargin":
										mc.IsFoldMargin = getBool(reader.Value);
										break;
									case "ismarkermargin":
										mc.IsMarkerMargin = getBool(reader.Value);
										break;
									case "type":
										mc.Type = (MarginType)Enum.Parse(typeof(MarginType), reader.Value, true);
										break;
									case "width":
										mc.Width = getInt(reader.Value);
										break;
								}
							}
							_margin_List.Add(mc);
							reader.MoveToElement();
						}
					}
				}
			}

			reader.Read();
		}

		private void readLongLines(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "edgecolor":
							_longLines_EdgeColor = getColor(reader.Value);
							break;
						case "edgecolumn":
							_longLines_EdgeColumn = getInt(reader.Value);
							break;
						case "edgemode":
							_longLines_EdgeMode = (EdgeMode)Enum.Parse(typeof(EdgeMode), reader.Value, true);
							break;
					}
				}
				reader.MoveToElement();
			}
			reader.Skip();
		}

		private void readLineWrap(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "layoutcache":
							_lineWrap_LayoutCache = (LineCache)Enum.Parse(typeof(LineCache), reader.Value, true);
							break;
						case "mode":
							_lineWrap_Mode = (WrapMode)Enum.Parse(typeof(WrapMode), reader.Value, true);
							break;
						case "positioncachesize":
							_lineWrap_PositionCacheSize = getInt(reader.Value);
							break;
						case "startindent":
							_lineWrap_StartIndent = getInt(reader.Value);
							break;
						case "visualflags":
							string flags = reader.Value.Trim();
							if (flags != string.Empty)
							{
								WrapVisualFlag? wvf = null;
								foreach (string flag in flags.Split(' '))
									wvf |= (WrapVisualFlag)Enum.Parse(typeof(WrapVisualFlag), flag.Trim(), true);

								if (wvf.HasValue)
									_lineWrap_VisualFlags = wvf;
							}
							break;
						case "visualflagslocation":
							_lineWrap_VisualFlagsLocation = (WrapVisualLocation)Enum.Parse(typeof(WrapVisualLocation), reader.Value, true);
							break;
					}
				}
				reader.MoveToElement();
			}

			reader.Skip();
		}

		private void readLexer(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "whitespacechars":
							_lexing_WhiteSpaceChars = reader.Value;
							break;
						case "wordchars":
							_lexing_WordChars = reader.Value;
							break;
						case "lexername":
							_lexing_Language = reader.Value;
							break;
						case "linecommentprefix":
							_lexing_LineCommentPrefix = reader.Value;
							break;
						case "streamcommentprefix":
							_lexing_StreamCommentPrefix = reader.Value;
							break;
						case "streamcommentsuffix":
							_lexing_StreamCommentSuffix = reader.Value;
							break;
					}
				}
				reader.MoveToElement();
			}

			if (!reader.IsEmptyElement)
			{
				reader.Read();
				while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("lexer", StringComparison.OrdinalIgnoreCase)))
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.Name.Equals("properties", StringComparison.OrdinalIgnoreCase))
							readLexerProperties(reader);
						else if (reader.Name.Equals("keywords", StringComparison.OrdinalIgnoreCase))
							readLexerKeywords(reader);
					}
				}
			}
			reader.Read();
		}

		private void readLexerKeywords(XmlReader reader)
		{
			bool? inherit = null;
			int? list = null;
			string keywords = null;

			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "inherit":
							inherit = getBool(reader.Value);
							break;
						case "list":
							list = getInt(reader.Value);
							break;
					}
				}

				reader.MoveToElement();
			}
			
			if (!reader.IsEmptyElement)
				keywords = reader.ReadString().Trim();

			_lexing_Keywords.Add(new KeyWordConfig(list.Value, keywords, inherit));

			reader.Read();
		}

		private void readLexerProperties(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
					if (reader.Name.ToLower() == "inherit")
						_lexing_Properties.Inherit = getBool(reader.Value);

				reader.MoveToElement();
			}

			if (!reader.IsEmptyElement)
			{
				while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("properties", StringComparison.OrdinalIgnoreCase)))
				{
					reader.Read();
					if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("property", StringComparison.OrdinalIgnoreCase))
					{
						if (reader.HasAttributes)
						{
							string name = string.Empty;
							string value = string.Empty;
							while (reader.MoveToNextAttribute())
							{
								string attrName = reader.Name.ToLower();
								switch (attrName)
								{
									case "name":
										name = reader.Value;
										break;
									case "value":
										value = reader.Value;
										break;
								}
							}
							_lexing_Properties.Add(name, value);
							reader.MoveToElement();
						}
					}
				}
			}

			reader.Read();
		}			
		

		private void readIndicators(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "inherit":
							_indicator_List.Inherit = getBool(reader.Value);
							break;
					}
				}
				reader.MoveToElement();
			}

			if (!reader.IsEmptyElement)
			{
				while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("indicators", StringComparison.OrdinalIgnoreCase)))
				{
					reader.Read();
					if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("indicator", StringComparison.OrdinalIgnoreCase))
					{
						if (reader.HasAttributes)
						{
							IndicatorConfig ic = new IndicatorConfig();
							while (reader.MoveToNextAttribute())
							{
								string attrName = reader.Name.ToLower();
								switch (attrName)
								{
									case "number":
										ic.Number = int.Parse(reader.Value);
										break;
									case "color":
										ic.Color = getColor(reader.Value);
										break;
									case "inherit":
										ic.Inherit = getBool(reader.Value);
										break;
									case "isdrawnunder":
										ic.IsDrawnUnder = getBool(reader.Value);
										break;
									case "style":
										ic.Style = (IndicatorStyle)Enum.Parse(typeof(IndicatorStyle), reader.Value, true);
										break;
								}
							}
							_indicator_List.Add(ic);
							reader.MoveToElement();
						}
					}
				}
			}
			reader.Read();
		}

		private void readIndentation(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "backspaceunindents":
							_indentation_BackspaceUnindents = getBool(reader.Value);
							break;
						case "indentwidth":
							_indentation_IndentWidth = getInt(reader.Value);
							break;
						case "showguides":
							_indentation_ShowGuides = getBool(reader.Value);
							break;
						case "tabindents":
							_indentation_TabIndents = getBool(reader.Value);
							break;
						case "tabwidth":
							_indentation_TabWidth = getInt(reader.Value);
							break;
						case "usetabs":
							_indentation_UseTabs = getBool(reader.Value);
							break;
						case "smartindenttype":
							_indentation_SmartIndentType = (SmartIndent)Enum.Parse(typeof(SmartIndent), reader.Value, true);
							break;
					}
				}

				reader.MoveToElement();
			}

			reader.Skip();
		}

		private void readHotSpot(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "activebackcolor":
							_hotspot_ActiveBackColor = getColor(reader.Value);
							break;
						case "activeforecolor":
							_hotspot_ActiveForeColor = getColor(reader.Value);
							break;
						case "activeunderline":
							_hotspot_ActiveUnderline = getBool(reader.Value);
							break;
						case "singleline":
							_hotspot_SingleLine = getBool(reader.Value);
							break;
						case "useactivebackcolor":
							_hotspot_UseActiveBackColor = getBool(reader.Value);
							break;
						case "useactiveforecolor":
							_hotspot_UseActiveForeColor = getBool(reader.Value);
							break;
					}
				}

				reader.MoveToElement();
			}

			reader.Skip();
		}

		private void readEndOfLine(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "convertonpaste":
							_endOfLine_ConvertOnPaste = getBool(reader.Value);
							break;
						case "isvisible":
							_endOfLine_IsVisisble = getBool(reader.Value);
							break;
						case "mode":					
							_endOfLine_Mode = (EndOfLineMode)Enum.Parse(typeof(EndOfLineMode), reader.Value, true);
							break;
					}
				}

				reader.MoveToElement();
			}

			reader.Skip();
		}

		private void readCommands(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "Inherit":
							_commands_KeyBindingList.Inherit = getBool(reader.Value);
							break;
						case "AllowDuplicateBindings":
							_commands_KeyBindingList.AllowDuplicateBindings = getBool(reader.Value);
							break;
					}
				}

				reader.MoveToElement();
			}

			if (!reader.IsEmptyElement)
			{
				while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("commands", StringComparison.OrdinalIgnoreCase)))
				{
					reader.Read();
					if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("binding", StringComparison.OrdinalIgnoreCase))
					{
						if (reader.HasAttributes)
						{
							KeyBinding kb = new KeyBinding();
							BindableCommand cmd = new BindableCommand();
							bool? replaceCurrent = null;

							while (reader.MoveToNextAttribute())
							{
								string attrName = reader.Name.ToLower();
								switch (attrName)
								{
									case "key":
										kb.KeyCode = Utilities.GetKeys(reader.Value);
										break;
									case "modifier":
										if (reader.Value != string.Empty)
										{
											foreach (string modifier in reader.Value.Split(' '))
												kb.Modifiers |= (Keys)Enum.Parse(typeof(Keys), modifier.Trim(), true);
										}
										break;
									case "command":
										cmd = (BindableCommand)Enum.Parse(typeof(BindableCommand), reader.Value, true);
										break;
									case "replacecurrent":
										replaceCurrent = getBool(reader.Value);
										break;
								}
							}

							_commands_KeyBindingList.Add(new CommandBindingConfig(kb, replaceCurrent, cmd));
						}

						reader.MoveToElement();
					}
				}
			}
			reader.Read();
		}

		private void readClipboard(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "convertendoflineonpaste":
							_clipboard_ConvertEndOfLineOnPaste = getBool(reader.Value);
							break;
					}
				}
				reader.MoveToElement();
			}
			reader.Skip();
		}

		private void readCaret(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "blinkrate":
							//	This guy is a bit of an oddball becuase null means "I don't Care"
							//	and we need some way of using the OS value.
							string blinkRate = reader.Value;
							if (blinkRate.ToLower() == "system")
								_caret_BlinkRate = SystemInformation.CaretBlinkTime;
							else
								_caret_BlinkRate = getInt(blinkRate);
							break;
						case "color":
							_caret_Color = getColor(reader.Value);
							break;
						case "currentlinebackgroundalpha":
							_caret_CurrentLineBackgroundAlpha = getInt(reader.Value);
							break;
						case "currentlinebackgroundcolor":
							_caret_CurrentLineBackgroundColor = getColor(reader.Value);
							break;
						case "highlightcurrentline":
							_caret_HighlightCurrentLine = getBool(reader.Value);
							break;
						case "issticky":
							_caret_IsSticky = getBool(reader.Value);
							break;
						case "style":
							_caret_Style = (CaretStyle)Enum.Parse(typeof(CaretStyle), reader.Value, true);
							break;
						case "width":
							_caret_Width = getInt(reader.Value);
							break;
					}
				}
				reader.MoveToElement();
			}
			reader.Skip();
		}

		private void readCallTip(XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					string attrName = reader.Name.ToLower();
					switch (attrName)
					{
						case "backcolor":
							_callTip_BackColor = getColor(reader.Value);
							break;
						case "forecolor":
							_callTip_ForeColor = getColor(reader.Value);
							break;
						case "highlighttextcolor":
							_callTip_HighlightTextColor = getColor(reader.Value);
							break;
					}
				}

				reader.MoveToElement();
			}

			reader.Skip();
		}

		public void Load(XmlDocument configDocument)
		{
			
			XmlNode langNode = configDocument.DocumentElement.SelectSingleNode("./Language[@Name='" + _language + "']");
			if (langNode == null)
				return;

			XmlElement callTipNode = langNode.SelectSingleNode("CallTip") as XmlElement;
			if (callTipNode != null)
			{
				_callTip_BackColor = getColor(callTipNode.GetAttribute("BackColor"));
				_callTip_ForeColor = getColor(callTipNode.GetAttribute("ForeColor"));
				_callTip_HighlightTextColor = getColor(callTipNode.GetAttribute("HighlightTextColor"));
			}
			callTipNode = null;

			XmlElement caretNode = langNode.SelectSingleNode("Caret") as XmlElement;
			if (caretNode != null)
			{
				//	This guy is a bit of an oddball becuase null means "I don't Care"
				//	and we need some way of using the OS value.
				string blinkRate = caretNode.GetAttribute("BlinkRate");
				if (blinkRate.ToLower() == "system")
					_caret_BlinkRate = SystemInformation.CaretBlinkTime;
				else
					_caret_BlinkRate = getInt(blinkRate);

				_caret_Color = getColor(caretNode.GetAttribute("Color"));
				_caret_CurrentLineBackgroundAlpha = getInt(caretNode.GetAttribute("CurrentLineBackgroundAlpha"));
				_caret_CurrentLineBackgroundColor = getColor(caretNode.GetAttribute("CurrentLineBackgroundColor"));
				_caret_HighlightCurrentLine = getBool(caretNode.GetAttribute("HighlightCurrentLine"));
				_caret_IsSticky = getBool(caretNode.GetAttribute("IsSticky"));
				try
				{
					_caret_Style = (CaretStyle)Enum.Parse(typeof(CaretStyle), caretNode.GetAttribute("Style"), true);
				}
				catch (ArgumentException) { }
				_caret_Width = getInt(caretNode.GetAttribute("Width"));
			}
			caretNode = null;

			XmlElement clipboardNode = langNode.SelectSingleNode("Clipboard") as XmlElement;
			if (clipboardNode != null)
			{
				_clipboard_ConvertEndOfLineOnPaste = getBool(clipboardNode.GetAttribute("ConvertEndOfLineOnPaste"));
			}
			clipboardNode = null;

			_commands_KeyBindingList = new CommandBindingConfigList();
			XmlElement commandsNode = langNode.SelectSingleNode("Commands") as XmlElement;
			if (commandsNode != null)
			{
				_commands_KeyBindingList.Inherit = getBool(commandsNode.GetAttribute("Inherit"));
				_commands_KeyBindingList.AllowDuplicateBindings = getBool(commandsNode.GetAttribute("AllowDuplicateBindings"));
				foreach (XmlElement el in commandsNode.SelectNodes("./Binding"))
				{
					KeyBinding kb = new KeyBinding();
					kb.KeyCode = Utilities.GetKeys(el.GetAttribute("Key"));

					string modifiers = el.GetAttribute("Modifier");
					if (modifiers != string.Empty)
					{
						foreach (string modifier in modifiers.Split(' '))
							kb.Modifiers |= (Keys)Enum.Parse(typeof(Keys), modifier.Trim(), true);
					}

					BindableCommand cmd = (BindableCommand)Enum.Parse(typeof(BindableCommand), el.GetAttribute("Command"), true);
					CommandBindingConfig cfg = new CommandBindingConfig(kb, getBool(el.GetAttribute("ReplaceCurrent")), cmd);
					_commands_KeyBindingList.Add(cfg);
				}
			}
			commandsNode = null;

			XmlElement endOfLineNode = langNode.SelectSingleNode("EndOfLine") as XmlElement;
			if (endOfLineNode != null)
			{
				_endOfLine_ConvertOnPaste = getBool(endOfLineNode.GetAttribute("ConvertOnPaste"));
				_endOfLine_IsVisisble = getBool(endOfLineNode.GetAttribute("IsVisible"));

				try
				{
					_endOfLine_Mode = (EndOfLineMode)Enum.Parse(typeof(EndOfLineMode), endOfLineNode.GetAttribute("Mode"), true);
				}
				catch (ArgumentException) { }
			}
			endOfLineNode = null;

			XmlElement hotSpotNode = langNode.SelectSingleNode("HotSpot") as XmlElement;
			if (hotSpotNode != null)
			{
				_hotspot_ActiveBackColor = getColor(hotSpotNode.GetAttribute("ActiveBackColor"));
				_hotspot_ActiveForeColor = getColor(hotSpotNode.GetAttribute("ActiveForeColor"));
				_hotspot_ActiveUnderline = getBool(hotSpotNode.GetAttribute("ActiveUnderline"));
				_hotspot_SingleLine = getBool(hotSpotNode.GetAttribute("SingleLine"));
				_hotspot_UseActiveBackColor = getBool(hotSpotNode.GetAttribute("UseActiveBackColor"));
				_hotspot_UseActiveForeColor = getBool(hotSpotNode.GetAttribute("UseActiveForeColor"));
			}
			hotSpotNode = null;

			XmlElement indentationNode = langNode.SelectSingleNode("Indentation") as XmlElement;
			if (indentationNode != null)
			{
				_indentation_BackspaceUnindents = getBool(indentationNode.GetAttribute("BackspaceUnindents"));
				_indentation_IndentWidth = getInt(indentationNode.GetAttribute("IndentWidth"));
				_indentation_ShowGuides = getBool(indentationNode.GetAttribute("ShowGuides"));
				_indentation_TabIndents = getBool(indentationNode.GetAttribute("TabIndents"));
				_indentation_TabWidth = getInt(indentationNode.GetAttribute("TabWidth"));
				_indentation_UseTabs = getBool(indentationNode.GetAttribute("UseTabs"));

				try
				{
					_indentation_SmartIndentType = (SmartIndent)Enum.Parse(typeof(SmartIndent), indentationNode.GetAttribute("SmartIndentType"), true);
				}
				catch (ArgumentException) { }

			}
			indentationNode = null;

			XmlElement indicatorNode = langNode.SelectSingleNode("Indicators") as XmlElement;
			if (indicatorNode != null)
			{
				_indicator_List.Inherit = getBool(indicatorNode.GetAttribute("Inherit"));
				foreach (XmlElement el in indicatorNode.SelectNodes("Indicator"))
				{
					IndicatorConfig ic = new IndicatorConfig();
					ic.Number = int.Parse(el.GetAttribute("Number"));
					ic.Color = getColor(el.GetAttribute("Color"));
					ic.Inherit = getBool(el.GetAttribute("Inherit"));
					ic.IsDrawnUnder = getBool(el.GetAttribute("IsDrawnUnder"));
					try
					{
						ic.Style = (IndicatorStyle)Enum.Parse(typeof(IndicatorStyle), el.GetAttribute("Style"), true);
					}
					catch (ArgumentException) { }

					_indicator_List.Add(ic);
				}
			}

			_lexing_Properties = new LexerPropertiesConfig();
			_lexing_Keywords = new KeyWordConfigList();
			XmlElement lexerNode = langNode.SelectSingleNode("Lexer") as XmlElement;
			if (lexerNode != null)
			{
				_lexing_WhiteSpaceChars = getString(lexerNode.GetAttributeNode("WhiteSpaceChars"));
				_lexing_WordChars = getString(lexerNode.GetAttributeNode("WordChars"));
				_lexing_Language = getString(lexerNode.GetAttributeNode("LexerName"));
				_lexing_LineCommentPrefix = getString(lexerNode.GetAttributeNode("LineCommentPrefix"));
				_lexing_StreamCommentPrefix = getString(lexerNode.GetAttributeNode("StreamCommentPrefix"));
				_lexing_StreamCommentSuffix = getString(lexerNode.GetAttributeNode("StreamCommentSuffix"));

				XmlElement propNode = lexerNode.SelectSingleNode("Properties") as XmlElement;
				if (propNode != null)
				{
					_lexing_Properties.Inherit = getBool(propNode.GetAttribute("Inherit"));

					foreach (XmlElement el in propNode.SelectNodes("Property"))
						_lexing_Properties.Add(el.GetAttribute("Name"), el.GetAttribute("Value"));
				}

				foreach (XmlElement el in lexerNode.SelectNodes("Keywords"))
					_lexing_Keywords.Add(new KeyWordConfig(getInt(el.GetAttribute("List")).Value, el.InnerText.Trim(), getBool(el.GetAttribute("Inherit"))));

			}
			lexerNode = null;

			XmlElement lineWrapNode = langNode.SelectSingleNode("LineWrap") as XmlElement;
			if (lineWrapNode != null)
			{
				try
				{
					_lineWrap_LayoutCache = (LineCache)Enum.Parse(typeof(LineCache), lineWrapNode.GetAttribute("LayoutCache"), true);
				}
				catch (ArgumentException) { }

				try
				{
					_lineWrap_Mode = (WrapMode)Enum.Parse(typeof(WrapMode), lineWrapNode.GetAttribute("Mode"), true);
				}
				catch (ArgumentException) { }

				_lineWrap_PositionCacheSize = getInt(lineWrapNode.GetAttribute("PositionCacheSize"));
				_lineWrap_StartIndent = getInt(lineWrapNode.GetAttribute("StartIndent"));

				string flags = lineWrapNode.GetAttribute("VisualFlags").Trim();
				if (flags != string.Empty)
				{
					WrapVisualFlag? wvf = null;
					foreach (string flag in flags.Split(' '))
						wvf |= (WrapVisualFlag)Enum.Parse(typeof(WrapVisualFlag), flag.Trim(), true);

					if (wvf.HasValue)
						_lineWrap_VisualFlags = wvf;
				}

				try
				{
					_lineWrap_VisualFlagsLocation = (WrapVisualLocation)Enum.Parse(typeof(WrapVisualLocation), lineWrapNode.GetAttribute("VisualFlagsLocation"), true);
				}
				catch (ArgumentException) { }
			}
			lineWrapNode = null;

			XmlElement longLinesNode = langNode.SelectSingleNode("LongLines") as XmlElement;
			if (longLinesNode != null)
			{
				_longLines_EdgeColor = getColor(longLinesNode.GetAttribute("EdgeColor"));
				_longLines_EdgeColumn = getInt(longLinesNode.GetAttribute("EdgeColumn"));
				try
				{
					_longLines_EdgeMode = (EdgeMode)Enum.Parse(typeof(EdgeMode), longLinesNode.GetAttribute("EdgeMode"), true);
				}
				catch (ArgumentException) { }
			}
			longLinesNode = null;

			_margin_List = new MarginConfigList();
			XmlElement marginNode = langNode.SelectSingleNode("Margins") as XmlElement;
			if (marginNode != null)
			{
				_margin_List.FoldMarginColor = getColor(marginNode.GetAttribute("FoldMarginColor"));
				_margin_List.FoldMarginHighlightColor = getColor(marginNode.GetAttribute("FoldMarginHighlightColor"));
				_margin_List.Left = getInt(marginNode.GetAttribute("Left"));
				_margin_List.Right = getInt(marginNode.GetAttribute("Right"));
				_margin_List.Inherit = getBool(marginNode.GetAttribute("Inherit"));

				foreach (XmlElement el in marginNode.SelectNodes("./Margin"))
				{
					MarginConfig mc = new MarginConfig();
					mc.Number = int.Parse(el.GetAttribute("Number"));
					mc.Inherit = getBool(el.GetAttribute("Inherit"));
					mc.AutoToggleMarkerNumber = getInt(el.GetAttribute("AutoToggleMarkerNumber"));
					mc.IsClickable = getBool(el.GetAttribute("IsClickable"));
					mc.IsFoldMargin = getBool(el.GetAttribute("IsFoldMargin"));
					mc.IsMarkerMargin = getBool(el.GetAttribute("IsMarkerMargin"));
					try
					{
						mc.Type = (MarginType)Enum.Parse(typeof(MarginType), el.GetAttribute("Type"), true);
					}
					catch (ArgumentException) { }

					mc.Width = getInt(el.GetAttribute("Width"));

					_margin_List.Add(mc);
				}
			}
			marginNode = null;

			XmlElement markersNode = langNode.SelectSingleNode("Markers") as XmlElement;
			_markers_List = new MarkersConfigList();
			if (markersNode != null)
			{
				_markers_List.Inherit = getBool(markersNode.GetAttribute("Inherit"));

				foreach (XmlElement el in markersNode.SelectNodes("Marker"))
				{
					MarkersConfig mc = new MarkersConfig();
					mc.Alpha = getInt(el.GetAttribute("Alpha"));
					mc.BackColor = getColor(el.GetAttribute("BackColor"));
					mc.ForeColor = getColor(el.GetAttribute("ForeColor"));
					mc.Name = getString(el.GetAttributeNode("Name"));
					mc.Number = getInt(el.GetAttribute("Number"));
					mc.Inherit = getBool(el.GetAttribute("Inherit"));
					try
					{
						mc.Symbol = (MarkerSymbol)Enum.Parse(typeof(MarkerSymbol), el.GetAttribute("Symbol"), true);
					}
					catch (ArgumentException) { }
					_markers_List.Add(mc);
				}
			}

			XmlElement scrollingNode = langNode.SelectSingleNode("Scrolling") as XmlElement;
			if (scrollingNode != null)
			{
				_scrolling_EndAtLastLine = getBool(scrollingNode.GetAttribute("EndAtLastLine"));
				_scrolling_HorizontalWidth = getInt(scrollingNode.GetAttribute("HorizontalWidth"));

				string flags = scrollingNode.GetAttribute("ScrollBars").Trim();
				if (flags != string.Empty)
				{
					ScrollBars? sb = null;
					foreach (string flag in flags.Split(' '))
						sb |= (ScrollBars)Enum.Parse(typeof(ScrollBars), flag.Trim(), true);

					if (sb.HasValue)
						_scrolling_ScrollBars = sb;
				}

				_scrolling_XOffset = getInt(scrollingNode.GetAttribute("XOffset"));
			}
			scrollingNode = null;


			XmlElement selectionNode = langNode.SelectSingleNode("Selection") as XmlElement;
			if (selectionNode != null)
			{
				_selection_BackColor = getColor(selectionNode.GetAttribute("BackColor"));
				_selection_BackColorUnfocused = getColor(selectionNode.GetAttribute("BackColorUnfocused"));
				_selection_ForeColor = getColor(selectionNode.GetAttribute("ForeColor"));
				_selection_ForeColorUnfocused = getColor(selectionNode.GetAttribute("ForeColorUnfocused"));
				_selection_Hidden = getBool(selectionNode.GetAttribute("Hidden"));
				_selection_HideSelection = getBool(selectionNode.GetAttribute("HideSelection"));
				try
				{
					_selection_Mode = (SelectionMode)Enum.Parse(typeof(SelectionMode), selectionNode.GetAttribute("Mode"), true);
				}
				catch (ArgumentException) { }
			}
			selectionNode = null;

			_styles = new StyleConfigList();
			XmlElement stylesNode = langNode.SelectSingleNode("Styles") as XmlElement;
			if (stylesNode != null)
			{
				_styles.Bits = getInt(stylesNode.GetAttribute("Bits"));
				foreach (XmlElement el in stylesNode.SelectNodes("Style"))
				{
					StyleConfig sc = new StyleConfig();
					sc.Name = el.GetAttribute("Name");
					sc.Number = getInt(el.GetAttribute("Number"));
					sc.BackColor = getColor(el.GetAttribute("BackColor"));
					sc.Bold = getBool(el.GetAttribute("Bold"));
					try
					{
						sc.Case = (StyleCase)Enum.Parse(typeof(StyleCase), el.GetAttribute("Case"), true);
					}
					catch (ArgumentException) { }

					try
					{
						sc.CharacterSet = (CharacterSet)Enum.Parse(typeof(CharacterSet), el.GetAttribute("CharacterSet"), true);
					}
					catch (ArgumentException) { }

					sc.FontName = getString(el.GetAttributeNode("FontName"));
					sc.ForeColor = getColor(el.GetAttribute("ForeColor"));
					sc.IsChangeable = getBool(el.GetAttribute("IsChangeable"));
					sc.IsHotspot = getBool(el.GetAttribute("IsHotspot"));
					sc.IsSelectionEolFilled = getBool(el.GetAttribute("IsSelectionEolFilled"));
					sc.IsVisible = getBool(el.GetAttribute("IsVisible"));
					sc.Italic = getBool(el.GetAttribute("Italic"));
					sc.Size = getInt(el.GetAttribute("Size"));
					sc.Underline = getBool(el.GetAttribute("Underline"));
					sc.Inherit = getBool(el.GetAttribute("Inherit"));
					
					_styles.Add(sc);
				}

				//	This is a nifty added on hack made specifically for HTML.
				//	Normally the style config elements are quite managable as there
				//	are typically less than 10 when you don't count common styles.
				//	
				//	However HTML uses 9 different Sub languages that combined make 
				//	use of all 128 styles (well there are some small gaps). In order
				//	to make this more managable I did added a SubLanguage element that
				//	basically just prepends the Language's name and "." to the Style 
				//	Name definition.
				//
				//	So for example if you had the following
				//	<Styles>
				//		<SubLanguage Name="ASP JavaScript">
				//			<Style Name="Keyword" Bold="True" />
				//		</SubLanguage>
				//	</Styles>
				//	That style's name will get interpreted as "ASP JavaScript.Keyword".
				//	which if you look at the html.txt in LexerStyleNames you'll see it
				//	maps to Style # 62

				//	Yeah I copied and pasted from above. I know. Feel free to refactor
				//	this and check it in since you're so high and mighty.
				foreach (XmlElement subLanguage in stylesNode.SelectNodes("SubLanguage"))
				{
					string subLanguageName = subLanguage.GetAttribute("Name");
					foreach (XmlElement el in subLanguage.SelectNodes("Style"))
					{
						StyleConfig sc = new StyleConfig();
						sc.Name = subLanguageName + "." + el.GetAttribute("Name");
						sc.Number = getInt(el.GetAttribute("Number"));
						sc.BackColor = getColor(el.GetAttribute("BackColor"));
						sc.Bold = getBool(el.GetAttribute("Bold"));
						try
						{
							sc.Case = (StyleCase)Enum.Parse(typeof(StyleCase), el.GetAttribute("Case"), true);
						}
						catch (ArgumentException) { }

						try
						{
							sc.CharacterSet = (CharacterSet)Enum.Parse(typeof(CharacterSet), el.GetAttribute("CharacterSet"), true);
						}
						catch (ArgumentException) { }

						sc.FontName = getString(el.GetAttributeNode("FontName"));
						sc.ForeColor = getColor(el.GetAttribute("ForeColor"));
						sc.IsChangeable = getBool(el.GetAttribute("IsChangeable"));
						sc.IsHotspot = getBool(el.GetAttribute("IsHotspot"));
						sc.IsSelectionEolFilled = getBool(el.GetAttribute("IsSelectionEolFilled"));
						sc.IsVisible = getBool(el.GetAttribute("IsVisible"));
						sc.Italic = getBool(el.GetAttribute("Italic"));
						sc.Size = getInt(el.GetAttribute("Size"));
						sc.Underline = getBool(el.GetAttribute("Underline"));
						sc.Inherit = getBool(el.GetAttribute("Inherit"));

						_styles.Add(sc);
					}
				}
			}
			stylesNode = null;

			XmlElement undoRedoNode = langNode.SelectSingleNode("UndoRedo") as XmlElement;
			if (undoRedoNode != null)
			{
				_undoRedoIsUndoEnabled = getBool(undoRedoNode.GetAttribute("IsUndoEnabled"));
			}
			undoRedoNode = null;


			XmlElement whiteSpaceNode = langNode.SelectSingleNode("WhiteSpace") as XmlElement;
			if (whiteSpaceNode != null)
			{
				_whiteSpace_BackColor = getColor(whiteSpaceNode.GetAttribute("BackColor"));
				_whiteSpace_ForeColor = getColor(whiteSpaceNode.GetAttribute("ForeColor"));
				_whiteSpace_Mode = (WhiteSpaceMode)Enum.Parse(typeof(WhiteSpaceMode), whiteSpaceNode.GetAttribute("Mode"), true);
				_whiteSpace_UseWhiteSpaceBackColor = getBool(whiteSpaceNode.GetAttribute("UseWhiteSpaceBackColor"));
				_whiteSpace_UseWhiteSpaceForeColor = getBool(whiteSpaceNode.GetAttribute("UseWhiteSpaceForeColor"));
			}
			whiteSpaceNode = null;

			configDocument = null;
		}
		#endregion

		#region Utility Methods

		private string getString(XmlAttribute a)
		{
			if (a == null)
				return null;

			return a.Value;
		}

		private bool? getBool(string s)
		{
			s = s.ToLower();

			switch (s)
			{
				case "true":
				case "t":
				case "1":
				case "y":
				case "yes":
					return true;
				case "false":
				case "f":
				case "0":
				case "n":
				case "no":
					return false;
			}

			return null;
		}

		private int? getInt(string s)
		{
			int i;
			if (int.TryParse(s, out i))
				return i;

			return null;
		}

		private Color getColor(string s)
		{
			return (Color)new ColorConverter().ConvertFromString(s);
		}

		private char? getChar(string s)
		{
			if (string.IsNullOrEmpty(s))
				return null;

			return s[0];
		}
		#endregion		

		#region CallTip
		private Color _callTip_ForeColor;
		public Color CallTip_ForeColor
		{
			get
			{
				return _callTip_ForeColor;
			}
			set
			{
				_callTip_ForeColor = value;
			}
		}


		private Color _callTip_BackColor;
		public Color CallTip_BackColor
		{
			get
			{
				return _callTip_BackColor;
			}
			set
			{
				_callTip_BackColor = value;
			}
		}

		private Color _callTip_HighlightTextColor;
		public Color CallTip_HighlightTextColor
		{
			get
			{
				return _callTip_HighlightTextColor;
			}
			set
			{
				_callTip_HighlightTextColor = value;
			}
		}

		#endregion

		#region Caret
		private int? _caret_Width;
		public int? Caret_Width
		{
			get
			{
				return _caret_Width;
			}
			set
			{
				_caret_Width = value;
			}
		}

		private CaretStyle? _caret_Style;
		public CaretStyle? Caret_Style
		{
			get
			{
				return _caret_Style;
			}
			set
			{
				_caret_Style = value;
			}
		}

		private Color _caret_Color;
		public Color Caret_Color
		{
			get
			{
				return _caret_Color;
			}
			set
			{
				_caret_Color = value;
			}
		}

		private Color _caret_CurrentLineBackgroundColor;
		public Color Caret_CurrentLineBackgroundColor
		{
			get
			{
				return _caret_CurrentLineBackgroundColor;
			}
			set
			{
				_caret_CurrentLineBackgroundColor = value;
			}
		}

		private bool? _caret_HighlightCurrentLine;
		public bool? Caret_HighlightCurrentLine
		{
			get
			{
				return _caret_HighlightCurrentLine;
			}
			set
			{
				_caret_HighlightCurrentLine = value;
			}
		}

		private int? _caret_CurrentLineBackgroundAlpha;
		public int? Caret_CurrentLineBackgroundAlpha
		{
			get
			{
				return _caret_CurrentLineBackgroundAlpha;
			}
			set
			{
				_caret_CurrentLineBackgroundAlpha = value;
			}
		}

		private int? _caret_BlinkRate;
		public int? Caret_BlinkRate
		{
			get
			{
				return _caret_BlinkRate;
			}
			set
			{
				_caret_BlinkRate = value;
			}
		}

		private bool? _caret_IsSticky;
		public bool? Caret_IsSticky
		{
			get
			{
				return _caret_IsSticky;
			}
			set
			{
				_caret_IsSticky = value;
			}
		}
		#endregion

		#region Clipboard
		private bool? _clipboard_ConvertEndOfLineOnPaste;
		public bool? Clipboard_ConvertEndOfLineOnPaste
		{
			get
			{
				return _clipboard_ConvertEndOfLineOnPaste;
			}
			set
			{
				_clipboard_ConvertEndOfLineOnPaste = value;
			}
		}
		#endregion

		#region Commands
		private CommandBindingConfigList _commands_KeyBindingList = new CommandBindingConfigList();
		public CommandBindingConfigList Commands_KeyBindingList
		{
			get
			{
				return _commands_KeyBindingList;
			}
			set
			{
				_commands_KeyBindingList = value;
			}
		}
		#endregion

		#region DropMarkers
		private string _dropMarkers_SharedStackName;
		public string DropMarkers_SharedStackName
		{
			get
			{
				return _dropMarkers_SharedStackName;
			}
			set
			{
				_dropMarkers_SharedStackName = value;
			}
		}
		#endregion

		#region EndOfLine
		private bool? _endOfLine_ConvertOnPaste;
		public bool? EndOfLine_ConvertOnPaste
		{
			get
			{
				return _endOfLine_ConvertOnPaste;
			}
			set
			{
				_endOfLine_ConvertOnPaste = value;
			}
		}

		private EndOfLineMode? _endOfLine_Mode;
		public EndOfLineMode? EndOfLine_Mode
		{
			get
			{
				return _endOfLine_Mode;
			}
			set
			{
				_endOfLine_Mode = value;
			}
		}

		private bool? _endOfLine_IsVisisble;
		public bool? EndOfLine_IsVisisble
		{
			get
			{
				return _endOfLine_IsVisisble;
			}
			set
			{
				_endOfLine_IsVisisble = value;
			}
		}
		#endregion		

		#region Hotspot
		private Color _hotspot_ActiveForeColor;
		public Color Hotspot_ActiveForeColor
		{
			get
			{
				return _hotspot_ActiveForeColor;
			}
			set
			{
				_hotspot_ActiveForeColor = value;
			}
		}

		private Color _hotspot_ActiveBackColor;
		public Color Hotspot_ActiveBackColor
		{
			get
			{
				return _hotspot_ActiveBackColor;
			}
			set
			{
				_hotspot_ActiveBackColor = value;
			}
		}

		private bool? _hotspot_ActiveUnderline;
		public bool? Hotspot_ActiveUnderline
		{
			get
			{
				return _hotspot_ActiveUnderline;
			}
			set
			{
				_hotspot_ActiveUnderline = value;
			}
		}

		private bool? _hotspot_SingleLine;
		public bool? Hotspot_SingleLine
		{
			get
			{
				return _hotspot_SingleLine;
			}
			set
			{
				_hotspot_SingleLine = value;
			}
		}

		private bool? _hotspot_UseActiveForeColor;
		public bool? Hotspot_UseActiveForeColor
		{
			get
			{
				return _hotspot_UseActiveForeColor;
			}
			set
			{
				_hotspot_UseActiveForeColor = value;
			}
		}

		private bool? _hotspot_UseActiveBackColor;
		public bool? Hotspot_UseActiveBackColor
		{
			get
			{
				return _hotspot_UseActiveBackColor;
			}
			set
			{
				_hotspot_UseActiveBackColor = value;
			}
		}
		#endregion

		#region Indentation
		private int? _indentation_TabWidth;
		public int? Indentation_TabWidth
		{
			get
			{
				return _indentation_TabWidth;
			}
			set
			{
				_indentation_TabWidth = value;
			}
		}

		private bool? _indentation_UseTabs;
		public bool? Indentation_UseTabs
		{
			get
			{
				return _indentation_UseTabs;
			}
			set
			{
				_indentation_UseTabs = value;
			}
		}

		private int? _indentation_IndentWidth;
		public int? Indentation_IndentWidth
		{
			get
			{
				return _indentation_IndentWidth;
			}
			set
			{
				_indentation_IndentWidth = value;
			}
		}

		private SmartIndent? _indentation_SmartIndentType;
		public SmartIndent? Indentation_SmartIndentType
		{
			get
			{
				return _indentation_SmartIndentType;
			}
			set
			{
				_indentation_SmartIndentType = value;
			}
		}

		private bool? _indentation_TabIndents;
		public bool? Indentation_TabIndents
		{
			get
			{
				return _indentation_TabIndents;
			}
			set
			{
				_indentation_TabIndents = value;
			}
		}

		private bool? _indentation_BackspaceUnindents;
		public bool? Indentation_BackspaceUnindents
		{
			get
			{
				return _indentation_BackspaceUnindents;
			}
			set
			{
				_indentation_BackspaceUnindents = value;
			}
		}

		private bool? _indentation_ShowGuides;
		public bool? Indentation_ShowGuides
		{
			get
			{
				return _indentation_ShowGuides;
			}
			set
			{
				_indentation_ShowGuides = value;
			}
		}
		#endregion

		private IndicatorConfigList _indicator_List = new IndicatorConfigList();
		public IndicatorConfigList Indicator_List
		{
			get
			{
				return _indicator_List;
			}
			set
			{
				_indicator_List = value;
			}
		}
		#region Lexing
		private string _lexing_WhiteSpaceChars;
		public string Lexing_WhiteSpaceChars
		{
			get
			{
				return _lexing_WhiteSpaceChars;
			}
			set
			{
				_lexing_WhiteSpaceChars = value;
			}
		}

		private string _lexing_WordChars;
		public string Lexing_WordChars
		{
			get
			{
				return _lexing_WordChars;
			}
			set
			{
				_lexing_WordChars = value;
			}
		}

		private string _lexing_Language;
		public string Lexing_Language
		{
			get
			{
				return _lexing_Language;
			}
			set
			{
				_lexing_Language = value;
			}
		}

		private string _lexing_LineCommentPrefix;
		public string Lexing_LineCommentPrefix
		{
			get
			{
				return _lexing_LineCommentPrefix;
			}
			set
			{
				_lexing_LineCommentPrefix = value;
			}
		}
		private string _lexing_StreamCommentSuffix;
		public string Lexing_StreamCommentSuffix
		{
			get
			{
				return _lexing_StreamCommentSuffix;
			}
			set
			{
				_lexing_StreamCommentSuffix = value;
			}
		}

		private string _lexing_StreamCommentPrefix;
		public string Lexing_StreamCommentPrefix
		{
			get
			{
				return _lexing_StreamCommentPrefix;
			}
			set
			{
				_lexing_StreamCommentPrefix = value;
			}
		}

		private LexerPropertiesConfig _lexing_Properties = new LexerPropertiesConfig();
		public LexerPropertiesConfig Lexing_Properties
		{
			get
			{
				return _lexing_Properties;
			}
			set
			{
				_lexing_Properties = value;
			}
		}

		private KeyWordConfigList _lexing_Keywords = new KeyWordConfigList();
		public KeyWordConfigList Lexing_Keywords
		{
			get
			{
				return _lexing_Keywords;
			}
			set
			{
				_lexing_Keywords = value;
			}
		}
		#endregion

		#region LineWrap
		private WrapMode? _lineWrap_Mode;
		public WrapMode? LineWrap_Mode
		{
			get
			{
				return _lineWrap_Mode;
			}
			set
			{
				_lineWrap_Mode = value;
			}
		}

		private WrapVisualFlag? _lineWrap_VisualFlags;
		public WrapVisualFlag? LineWrap_VisualFlags
		{
			get
			{
				return _lineWrap_VisualFlags;
			}
			set
			{
				_lineWrap_VisualFlags = value;
			}
		}

		private WrapVisualLocation? _lineWrap_VisualFlagsLocation;
		public WrapVisualLocation? LineWrap_VisualFlagsLocation
		{
			get
			{
				return _lineWrap_VisualFlagsLocation;
			}
			set
			{
				_lineWrap_VisualFlagsLocation = value;
			}
		}

		private int? _lineWrap_StartIndent;
		public int? LineWrap_StartIndent
		{
			get
			{
				return _lineWrap_StartIndent;
			}
			set
			{
				_lineWrap_StartIndent = value;
			}
		}

		private LineCache? _lineWrap_LayoutCache;
		public LineCache? LineWrap_LayoutCache
		{
			get
			{
				return _lineWrap_LayoutCache;
			}
			set
			{
				_lineWrap_LayoutCache = value;
			}
		}

		private int? _lineWrap_PositionCacheSize;
		public int? LineWrap_PositionCacheSize
		{
			get
			{
				return _lineWrap_PositionCacheSize;
			}
			set
			{
				_lineWrap_PositionCacheSize = value;
			}
		}
		#endregion

		#region LongLines
		private EdgeMode? _longLines_EdgeMode;
		public EdgeMode? LongLines_EdgeMode
		{
			get
			{
				return _longLines_EdgeMode;
			}
			set
			{
				_longLines_EdgeMode = value;
			}
		}

		private int? _longLines_EdgeColumn;
		public int? LongLines_EdgeColumn
		{
			get
			{
				return _longLines_EdgeColumn;
			}
			set
			{
				_longLines_EdgeColumn = value;
			}
		}
		private Color _longLines_EdgeColor;
		public Color LongLines_EdgeColor
		{
			get
			{
				return _longLines_EdgeColor;
			}
			set
			{
				_longLines_EdgeColor = value;
			}
		}
		#endregion

		#region Margin
		private MarginConfigList _margin_List = new MarginConfigList();
		public MarginConfigList Margin_List
		{
			get
			{
				return _margin_List;
			}
			set
			{
				_margin_List = value;
			}
		}

		#endregion

		private MarkersConfigList _markers_List;
		public MarkersConfigList Markers_List
		{
			get
			{
				return _markers_List;
			}
			set
			{
				_markers_List = value;
			}
		}

		#region ScrollBars
		private ScrollBars? _scrolling_ScrollBars;
		public ScrollBars? Scrolling_ScrollBars
		{
			get
			{
				return _scrolling_ScrollBars;
			}
			set
			{
				_scrolling_ScrollBars = value;
			}
		}

		private int? _scrolling_XOffset;
		public int? Scrolling_XOffset
		{
			get
			{
				return _scrolling_XOffset;
			}
			set
			{
				_scrolling_XOffset = value;
			}
		}

		private int? _scrolling_HorizontalWidth;
		public int? Scrolling_HorizontalWidth
		{
			get
			{
				return _scrolling_HorizontalWidth;
			}
			set
			{
				_scrolling_HorizontalWidth = value;
			}
		}

		private bool? _scrolling_EndAtLastLine;
		public bool? Scrolling_EndAtLastLine
		{
			get
			{
				return _scrolling_EndAtLastLine;
			}
			set
			{
				_scrolling_EndAtLastLine = value;
			}
		}
		#endregion

		#region Selection
		private Color _selection_ForeColor;
		public Color Selection_ForeColor
		{
			get
			{
				return _selection_ForeColor;
			}
			set
			{
				_selection_ForeColor = value;
			}
		}

		private Color _selection_ForeColorUnfocused;
		public Color Selection_ForeColorUnfocused
		{
			get
			{
				return _selection_ForeColorUnfocused;
			}
			set
			{
				_selection_ForeColorUnfocused = value;
			}
		}

		private Color _selection_BackColorUnfocused;
		public Color Selection_BackColorUnfocused
		{
			get
			{
				return _selection_BackColorUnfocused;
			}
			set
			{
				_selection_BackColorUnfocused = value;
			}
		}

		private Color _selection_BackColor;
		public Color Selection_BackColor
		{
			get
			{
				return _selection_BackColor;
			}
			set
			{
				_selection_BackColor = value;
			}
		}

		private bool? _selection_Hidden;
		public bool? Selection_Hidden
		{
			get
			{
				return _selection_Hidden;
			}
			set
			{
				_selection_Hidden = value;
			}
		}

		private bool? _selection_HideSelection;
		public bool? Selection_HideSelection
		{
			get
			{
				return _selection_HideSelection;
			}
			set
			{
				_selection_HideSelection = value;
			}
		}

		private SelectionMode? _selection_Mode;
		public SelectionMode? Selection_Mode
		{
			get
			{
				return _selection_Mode;
			}
			set
			{
				_selection_Mode = value;
			}
		}
		#endregion		

		#region StyleConfigList

		private StyleConfigList _styles = new StyleConfigList();
		public StyleConfigList Styles
		{
			get
			{
				return _styles;
			}
			set
			{
				_styles = value;
			}
		}

		#endregion

		#region UndoRedo
		private bool? _undoRedoIsUndoEnabled;
		public bool? UndoRedoIsUndoEnabled
		{
			get
			{
				return _undoRedoIsUndoEnabled;
			}
			set
			{
				_undoRedoIsUndoEnabled = value;
			}
		}
		#endregion

		#region WhiteSpace
		private Color _whiteSpace_BackColor;
		public Color WhiteSpace_BackColor
		{
			get
			{
				return _whiteSpace_BackColor;
			}
			set
			{
				_whiteSpace_BackColor = value;
			}
		}

		private Color _whiteSpace_ForeColor;
		public Color WhiteSpace_ForeColor
		{
			get
			{
				return _whiteSpace_ForeColor;
			}
			set
			{
				_whiteSpace_ForeColor = value;
			}
		}

		private WhiteSpaceMode? _whiteSpace_Mode;
		public WhiteSpaceMode? WhiteSpace_Mode
		{
			get
			{
				return _whiteSpace_Mode;
			}
			set
			{
				_whiteSpace_Mode = value;
			}
		}

		private bool? _whiteSpace_UseWhiteSpaceForeColor;
		public bool? WhiteSpace_UseWhiteSpaceForeColor
		{
			get
			{
				return _whiteSpace_UseWhiteSpaceForeColor;
			}
			set
			{
				_whiteSpace_UseWhiteSpaceForeColor = value;
			}
		}

		private bool? _whiteSpace_UseWhiteSpaceBackColor;
		public bool? WhiteSpace_UseWhiteSpaceBackColor
		{
			get
			{
				return _whiteSpace_UseWhiteSpaceBackColor;
			}
			set
			{
				_whiteSpace_UseWhiteSpaceBackColor = value;
			}
		}
		#endregion
	}
}

