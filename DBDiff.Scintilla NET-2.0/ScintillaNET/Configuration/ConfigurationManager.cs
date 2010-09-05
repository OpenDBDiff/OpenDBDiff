using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
namespace DBDiff.Scintilla.Configuration
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class ConfigurationManager : ScintillaHelperBase
	{
		private bool _useXmlReader = true;
		public bool UseXmlReader
		{
			get { return _useXmlReader; }
			set
			{
				_useXmlReader = value;
			}
		}
        
		internal ConfigurationManager(Scintilla scintilla) : base(scintilla) { }

		internal bool ShouldSerialize()
		{
			return ShouldSerializeClearKeyBindings() ||
				ShouldSerializeClearMargins() ||
				ShouldSerializeClearSnippets() ||
				ShouldSerializeClearStyles() ||
				ShouldSerializeCustomLocation() ||
				ShouldSerializeIsBuiltInEnabled() ||
				ShouldSerializeIsUserEnabled() ||
				ShouldSerializeLanguage() ||
				ShouldSerializeLoadOrder();
		}

		

		protected internal override void Initialize()
		{
			if (_language != null)
				Configure();
		}

		#region Language
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

				if (!Scintilla.IsDesignMode)
					Configure();
			}
		}

		private bool ShouldSerializeLanguage()
		{
			return !string.IsNullOrEmpty(_language);
		}

		private void ResetLanguage()
		{
			_language = null;
		} 
		#endregion

		#region IsBuiltInEnabled
		private bool _isBuiltInEnabled = true;
		public bool IsBuiltInEnabled
		{
			get
			{
				return _isBuiltInEnabled;
			}
			set
			{
				_isBuiltInEnabled = value;
			}
		}

		private bool ShouldSerializeIsBuiltInEnabled()
		{
			return !_isBuiltInEnabled;
		}

		private void ResetIsBuiltInEnabled()
		{
			_isBuiltInEnabled = true;
		} 
		#endregion

		#region IsUserEnabled
		private bool _isUserEnabled = true;
		public bool IsUserEnabled
		{
			get
			{
				return _isUserEnabled;
			}
			set
			{
				_isUserEnabled = value;
			}
		}

		private bool ShouldSerializeIsUserEnabled()
		{
			return !_isUserEnabled;
		}

		private void ResetIsUserEnabled()
		{
			_isUserEnabled = true;
		} 
		#endregion

		#region CustomLocation
		private string _customLocation;
		public string CustomLocation
		{
			get
			{
				return _customLocation;
			}
			set
			{
				_customLocation = value;
			}
		}

		private bool ShouldSerializeCustomLocation()
		{
			return !string.IsNullOrEmpty(_customLocation);
		}

		private void ResetCustomLocation()
		{
			_customLocation = string.Empty;
		} 
		#endregion

		#region ConfigurationLoadOrder
		private ConfigurationLoadOrder _loadOrder = ConfigurationLoadOrder.BuiltInCustomUser;
		public ConfigurationLoadOrder LoadOrder
		{
			get
			{
				return _loadOrder;
			}
			set
			{
				_loadOrder = value;
			}
		}

		private bool ShouldSerializeLoadOrder()
		{
			return _loadOrder != ConfigurationLoadOrder.BuiltInCustomUser;
		}

		private void ResetLoadOrder()
		{
			_loadOrder = ConfigurationLoadOrder.BuiltInCustomUser;
		} 
		#endregion

		#region ClearKeyBindings
		private bool _clearKeyBindings = false;
		public bool ClearKeyBindings
		{
			get
			{
				return _clearKeyBindings;
			}
			set
			{
				_clearKeyBindings = value;
			}
		}

		private bool ShouldSerializeClearKeyBindings()
		{
			return _clearKeyBindings;
		}

		private void ResetClearKeyBindings()
		{
			_clearKeyBindings = false;
		}

		#endregion

		#region ClearStyles
		private bool _clearStyles = false;
		public bool ClearStyles
		{
			get
			{
				return _clearStyles;
			}
			set
			{
				_clearStyles = value;
			}
		}

		private bool ShouldSerializeClearStyles()
		{
			return _clearStyles;
		}

		private void ResetClearStyles()
		{
			_clearStyles = false;
		} 
		#endregion

		#region ClearIndicators
		private bool _clearIndicators = false;
		public bool ClearIndicators
		{
			get
			{
				return _clearIndicators;
			}
			set
			{
				_clearIndicators = value;
			}
		}

		private bool ShouldSerializeClearIndicators()
		{
			return _clearIndicators;
		}

		private void ResetClearIndicators()
		{
			_clearIndicators = false;
		} 
		#endregion

		#region ClearSnippets
		private bool _clearSnippets = false;
		public bool ClearSnippets
		{
			get
			{
				return _clearSnippets;
			}
			set
			{
				_clearSnippets = value;
			}
		}

		private bool ShouldSerializeClearSnippets()
		{
			return _clearSnippets;
		}

		private void ResetClearSnippets()
		{
			_clearSnippets = false;
		} 
		#endregion

		#region ClearMargins
		private bool _clearMargins = false;
		public bool ClearMargins
		{
			get
			{
				return _clearMargins;
			}
			set
			{
				_clearMargins = value;
			}
		}

		private bool ShouldSerializeClearMargins()
		{
			return _clearMargins;
		}

		private void ResetClearMargins()
		{
			_clearMargins = true;
		} 
		#endregion

		#region ClearMarkers
		private bool _clearMarkers = false;
		public bool ClearMarkers
		{
			get
			{
				return _clearMarkers;
			}
			set
			{
				_clearMarkers = value;
			}
		}

		private bool ShouldSerializeClearMarkers()
		{
			return _clearMarkers;
		}

		private void ResetClearMarkers()
		{
			_clearMarkers = false;
		} 
		#endregion

		private string _appDataFolder;
		private string _userFolder;
		private string userFolder
		{
			get
			{
				if (_appDataFolder == null)
					_appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

				if (_userFolder == null)
				{
					Version v = GetType().Assembly.GetName().Version;
					
					_userFolder = Path.Combine(Path.Combine(_appDataFolder, "ScintillaNET"), v.Major.ToString() + "." + v.Minor.ToString());
				}

				return _userFolder;
			}
		}

		public void Configure()
		{
			if (Scintilla.IsDesignMode || Scintilla.IsInitializing)
				return;

			Configuration builtInDefault = null, 
				builtInLang = null, 
				customDefault = null, 
				customLang = null, 
				userDefault = null, 
				userLang = null;

			if (_isBuiltInEnabled)
			{
				using(Stream s = GetType().Assembly.GetManifestResourceStream("ScintillaNet.Configuration.Builtin.default.xml"))
					builtInDefault = new Configuration(s, "default", _useXmlReader);
				if (!string.IsNullOrEmpty(_language))
					using (Stream s = GetType().Assembly.GetManifestResourceStream("ScintillaNet.Configuration.Builtin." + _language + ".xml"))
						if (s != null)
							builtInLang = new Configuration(s, _language, _useXmlReader);
			}

			if (_isUserEnabled)
			{
				string defPath = Path.Combine(userFolder, "default.xml");
				if (File.Exists(defPath))
					userDefault = new Configuration(defPath, "default", _useXmlReader);

				if (!string.IsNullOrEmpty(_language))
				{
					string langPath = Path.Combine(userFolder, _language + ".xml");
					if (File.Exists(langPath))
						userLang = new Configuration(langPath, _language, _useXmlReader);
				}
			}

			if (!string.IsNullOrEmpty(_customLocation))
			{
				string defPath = Path.Combine(_customLocation, "default.xml");
				if (File.Exists(defPath))
					customDefault = new Configuration(defPath, "default", _useXmlReader);

				if (!string.IsNullOrEmpty(_language))
				{
					string langPath = Path.Combine(_customLocation, _language + ".xml");
					if (File.Exists(langPath))
						customLang = new Configuration(langPath, _language, _useXmlReader);
				}
			}

			List<Configuration> configList = new List<Configuration>();
			if (_loadOrder == ConfigurationLoadOrder.BuiltInCustomUser)
			{
				if (builtInDefault != null)
					configList.Add(builtInDefault);
				if (builtInLang != null)
					configList.Add(builtInLang);

				if (customDefault != null)
					configList.Add(customDefault);

				if (customLang != null)
					configList.Add(customLang);

				if (userDefault != null)
					configList.Add(userDefault);

				if (userLang != null)
					configList.Add(userLang);
			}
			else if (_loadOrder == ConfigurationLoadOrder.BuiltInUserCustom)
			{
				if (builtInDefault != null)
					configList.Add(builtInDefault);

				if (builtInLang != null)
					configList.Add(builtInLang);

				if (userDefault != null)
					configList.Add(userDefault);

				if (userLang != null)
					configList.Add(userLang);

				if (customDefault != null)
					configList.Add(customDefault);

				if (customLang != null)
					configList.Add(customLang);
			}
			else if (_loadOrder == ConfigurationLoadOrder.CustomBuiltInUser)
			{
				if (customDefault != null)
					configList.Add(customDefault);

				if (customLang != null)
					configList.Add(customLang);

				if (builtInDefault != null)
					configList.Add(builtInDefault);

				if (builtInLang != null)
					configList.Add(builtInLang);

				if (userDefault != null)
					configList.Add(userDefault);

				if (userLang != null)
					configList.Add(userLang);
			}
			else if (_loadOrder == ConfigurationLoadOrder.CustomUserBuiltIn)
			{
				if (customDefault != null)
					configList.Add(customDefault);

				if (customLang != null)
					configList.Add(customLang);

				if (userDefault != null)
					configList.Add(userDefault);

				if (userLang != null)
					configList.Add(userLang);

				if (builtInDefault != null)
					configList.Add(builtInDefault);

				if (builtInLang != null)
					configList.Add(builtInLang);
			}
			else if (_loadOrder == ConfigurationLoadOrder.UserBuiltInCustom)
			{
				if (userDefault != null)
					configList.Add(userDefault);

				if (userLang != null)
					configList.Add(userLang);

				if (builtInDefault != null)
					configList.Add(builtInDefault);

				if (builtInLang != null)
					configList.Add(builtInLang);

				if (customDefault != null)
					configList.Add(customDefault);

				if (customLang != null)
					configList.Add(customLang);

			}
			else if (_loadOrder == ConfigurationLoadOrder.UserCustomBuiltIn)
			{
				if (userDefault != null)
					configList.Add(userDefault);

				if (userLang != null)
					configList.Add(userLang);

				if (customDefault != null)
					configList.Add(customDefault);

				if (customLang != null)
					configList.Add(customLang);

				if (builtInDefault != null)
					configList.Add(builtInDefault);

				if (builtInLang != null)
					configList.Add(builtInLang);
			}

			Configure(configList);
		}

		public void Configure(Configuration config)
		{
			Configure(new List<Configuration>(new Configuration[] { config }));			
		}

		internal void Configure(List<Configuration> configList)
		{
			//	So here is the general pattern: We go through each of
			//	the configurations in the list (which has been ordered
			//	by priority). If the configuration has a value we're
			//	looking for it overwrites whatever was before it.
			//	In the end if the value isn't null, we set the
			//	corresponding Scintilla Value to this.
			bool? b = null;
			int? i = null;
			Color co = Color.Empty;
			char? ch = null;
			string s = null;

			b = null;
			ch = null;
			s = null;

			co = Color.Empty;
			foreach (Configuration c in configList)
			{
				if (c.CallTip_BackColor != Color.Empty)
					co = c.CallTip_BackColor;

			}
			if (co != Color.Empty)
				Scintilla.CallTip.BackColor = co;

			co = Color.Empty;
			foreach (Configuration c in configList)
			{
				if (c.CallTip_ForeColor != Color.Empty)
					co = c.CallTip_ForeColor;

			}
			if (co != Color.Empty)
				Scintilla.CallTip.ForeColor = co;

			co = Color.Empty;
			foreach (Configuration c in configList)
			{
				if (c.CallTip_HighlightTextColor != Color.Empty)
					co = c.CallTip_HighlightTextColor;

			}
			if (co != Color.Empty)
				Scintilla.CallTip.HighlightTextColor = co;

			i = null;
			foreach (Configuration c in configList)
			{
				if (c.Caret_BlinkRate.HasValue)
					i = c.Caret_BlinkRate;
			}
			if (i.HasValue)
				Scintilla.Caret.BlinkRate = i.Value;

			co = Color.Empty;
			foreach (Configuration c in configList)
			{
				if (c.Caret_Color != Color.Empty)
					co = c.Caret_Color;

			}
			if (co != Color.Empty)
				Scintilla.Caret.Color = co;

			i = null;
			foreach (Configuration c in configList)
			{
				if (c.Caret_CurrentLineBackgroundAlpha.HasValue)
					i = c.Caret_CurrentLineBackgroundAlpha;
			}
			if (i.HasValue)
				Scintilla.Caret.CurrentLineBackgroundAlpha = i.Value;

			co = Color.Empty;
			foreach (Configuration c in configList)
			{
				if (c.Caret_CurrentLineBackgroundColor != Color.Empty)
					co = c.Caret_CurrentLineBackgroundColor;

			}
			if (co != Color.Empty)
				Scintilla.Caret.CurrentLineBackgroundColor = co;

			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Caret_HighlightCurrentLine.HasValue)
					b = c.Caret_HighlightCurrentLine;

			}
			if (b.HasValue)
				Scintilla.Caret.HighlightCurrentLine = b.Value;

			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Caret_IsSticky.HasValue)
					b = c.Caret_IsSticky;

			}
			if (b.HasValue)
				Scintilla.Caret.IsSticky = b.Value;


			CaretStyle? caretStyle = null;
			foreach (Configuration c in configList)
			{
				if (c.Caret_Style.HasValue)
					caretStyle = c.Caret_Style;
			}
			if (caretStyle.HasValue)
				Scintilla.Caret.Style = caretStyle.Value;

			i = null;
			foreach (Configuration c in configList)
			{
				if (c.Caret_Width.HasValue)
					i = c.Caret_Width;
			}
			if (i.HasValue)
				Scintilla.Caret.Width = i.Value;

			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Clipboard_ConvertEndOfLineOnPaste.HasValue)
					b = c.Clipboard_ConvertEndOfLineOnPaste;
			}
			if (b.HasValue)
				Scintilla.Clipboard.ConvertEndOfLineOnPaste = b.Value;

			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Commands_KeyBindingList.AllowDuplicateBindings.HasValue)
					b = c.Commands_KeyBindingList.AllowDuplicateBindings;
			}
			if (b.HasValue)
				Scintilla.Commands.AllowDuplicateBindings = b.Value;

			if (_clearKeyBindings)
				Scintilla.Commands.RemoveAllBindings();

			CommandBindingConfigList cbcl = new CommandBindingConfigList();
			foreach (Configuration c in configList)
			{
				if (c.Commands_KeyBindingList.Inherit.HasValue && !c.Commands_KeyBindingList.Inherit.Value)
					cbcl.Clear();

				foreach (CommandBindingConfig cbc in c.Commands_KeyBindingList)
					cbcl.Add(cbc);
			}

			foreach (CommandBindingConfig cbc in cbcl)
			{
				//	This indicates that we should clear out any
				//	existing commands bound to this key combination
				if (cbc.ReplaceCurrent.HasValue && cbc.ReplaceCurrent.Value)
					Scintilla.Commands.RemoveBinding(cbc.KeyBinding.KeyCode, cbc.KeyBinding.Modifiers);

				Scintilla.Commands.AddBinding(cbc.KeyBinding.KeyCode, cbc.KeyBinding.Modifiers, cbc.BindableCommand);
			}

			s = null;
			foreach (Configuration c in configList)
			{
				if (c.DropMarkers_SharedStackName != null)
					s = c.DropMarkers_SharedStackName;
			}
			if (s != null)
				Scintilla.DropMarkers.SharedStackName = s;
			
			
			b = null;
			foreach (Configuration c in configList)
			{
				if (c.EndOfLine_ConvertOnPaste.HasValue)
					b = c.EndOfLine_ConvertOnPaste;
			}
			if (b.HasValue)
				Scintilla.EndOfLine.ConvertOnPaste = b.Value;


			b = null;
			foreach (Configuration c in configList)
			{
				if (c.EndOfLine_IsVisisble.HasValue)
					b = c.EndOfLine_IsVisisble;
			}
			if (b.HasValue)
				Scintilla.EndOfLine.IsVisible = b.Value;


			EndOfLineMode? endOfLineMode = null;
			foreach (Configuration c in configList)
			{
				if (c.EndOfLine_Mode.HasValue)
					endOfLineMode = c.EndOfLine_Mode;
			}
			if (endOfLineMode.HasValue)
				Scintilla.EndOfLine.Mode = endOfLineMode.Value;

			//	FoldMarkerScheme moved to Markers section
			//	becuase Markers need to come first as the
			//	FoldMarkerScheme really just manipulates
			//	Markers.

			b = null;			
			co = Color.Empty;
			foreach (Configuration c in configList)
			{
				if (c.Hotspot_ActiveBackColor != Color.Empty)
					co = c.Hotspot_ActiveBackColor;

			}
			if (co != Color.Empty)
				Scintilla.HotspotStyle.ActiveBackColor = co;
			
			co = Color.Empty;
			foreach (Configuration c in configList)
			{
				if (c.Hotspot_ActiveForeColor != Color.Empty)
					co = c.Hotspot_ActiveForeColor;

			}
			if (co != Color.Empty)
				Scintilla.HotspotStyle.ActiveForeColor = co;
			
			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Hotspot_ActiveUnderline.HasValue)
					b = c.Hotspot_ActiveUnderline;
			}
			if (b.HasValue)
				Scintilla.HotspotStyle.ActiveUnderline = b.Value;			

			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Hotspot_SingleLine.HasValue)
					b = c.Hotspot_SingleLine;
			}
			if (b.HasValue)
				Scintilla.HotspotStyle.SingleLine = b.Value;
			
			
			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Hotspot_UseActiveBackColor.HasValue)
					b = c.Hotspot_UseActiveBackColor;
			}
			if (b.HasValue)
				Scintilla.HotspotStyle.UseActiveBackColor = b.Value;

			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Hotspot_UseActiveForeColor.HasValue)
					b = c.Hotspot_UseActiveForeColor;
			}
			if (b.HasValue)
				Scintilla.HotspotStyle.UseActiveForeColor = b.Value;
			
			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Indentation_BackspaceUnindents.HasValue)
					b = c.Indentation_BackspaceUnindents;
			}
			if (b.HasValue)
				Scintilla.Indentation.BackspaceUnindents = b.Value;


			i = null;
			foreach (Configuration c in configList)
			{
				if (c.Indentation_IndentWidth.HasValue)
					i = c.Indentation_IndentWidth;
			}
			if (i.HasValue)
				Scintilla.Indentation.IndentWidth = i.Value;


			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Indentation_ShowGuides.HasValue)
					b = c.Indentation_ShowGuides;
			}
			if (b.HasValue)
				Scintilla.Indentation.ShowGuides = b.Value;
			
			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Indentation_TabIndents.HasValue)
					b = c.Indentation_TabIndents;
			}
			if (b.HasValue)
				Scintilla.Indentation.TabIndents = b.Value;			


			i = null;
			foreach (Configuration c in configList)
			{
				if (c.Indentation_TabWidth.HasValue)
					i = c.Indentation_TabWidth;
			}
			if (i.HasValue)
				Scintilla.Indentation.TabWidth = i.Value;			
			
			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Indentation_UseTabs.HasValue)
					b = c.Indentation_UseTabs;
			}
			if (b.HasValue)
				Scintilla.Indentation.UseTabs = b.Value;

			SmartIndent? si = null;
			foreach (Configuration c in configList)
			{
				if (c.Indentation_SmartIndentType.HasValue)
					si = c.Indentation_SmartIndentType;
			}
			if (si.HasValue)
				Scintilla.Indentation.SmartIndentType = si.Value;

			if (_clearIndicators)
				Scintilla.Indicators.Reset();

			IndicatorConfigList resolvedIndicators = new IndicatorConfigList();
			foreach (Configuration c in configList)
			{
				if (c.Indicator_List.Inherit.HasValue && !c.Indicator_List.Inherit.Value)
					resolvedIndicators.Clear();

				foreach (IndicatorConfig ic in c.Indicator_List)
				{
					if (!resolvedIndicators.Contains(ic.Number) || !(ic.Inherit.HasValue && ic.Inherit.Value))
					{
						resolvedIndicators.Remove(ic.Number);
						resolvedIndicators.Add(ic);
					}
					else
					{
						IndicatorConfig rc = resolvedIndicators[ic.Number];
						if (ic.Color != Color.Empty)
							rc.Color = ic.Color;

						if (ic.Style.HasValue)
							rc.Style = ic.Style;

						if (ic.IsDrawnUnder.HasValue)
							rc.IsDrawnUnder = ic.IsDrawnUnder;
					}
				}
			}

			foreach (IndicatorConfig ic in resolvedIndicators)
			{
				Indicator ind = Scintilla.Indicators[ic.Number];
				if (ic.Color != Color.Empty)
					ind.Color = ic.Color;

				if (ic.IsDrawnUnder.HasValue)
					ind.IsDrawnUnder = ic.IsDrawnUnder.Value;

				if (ic.Style.HasValue)
					ind.Style = ic.Style.Value;
			}

			foreach(Configuration c in configList)
            {
            	foreach (KeyWordConfig kwc in c.Lexing_Keywords)
				{
					if (kwc.Inherit.HasValue && kwc.Inherit.Value)
						Scintilla.Lexing.Keywords[kwc.List] += kwc.Value;
					else
						Scintilla.Lexing.Keywords[kwc.List] = kwc.Value;
				}
            }

			//	Hrm... unfortunately there's no way to clear
			//	Scintilla's Lexing Properties. Guess we'll just
			//	have to live with adding to the existing list 
			//	and/or just overriding with new values. This
			//	means that the "Inherit" attribute is really
			//	meaningless. Nevertheless I'm leaving it in
			//	just in case it ever becomes useful.
			foreach (Configuration c in configList)
			{
				foreach (KeyValuePair<string,string> item in c.Lexing_Properties)
				{
					Scintilla.Lexing.SetProperty(item.Key, item.Value);
				}
			}
			
			s = null;
			foreach (Configuration c in configList)
			{
				if (c.Lexing_WhiteSpaceChars != null)
					s = c.Lexing_WhiteSpaceChars;
			}
			if (s != null)
				Scintilla.Lexing.WhiteSpaceChars = s;


			s = null;
			foreach (Configuration c in configList)
			{
				if (c.Lexing_WordChars != null)
					s = c.Lexing_WordChars;
			}
			if (s != null)
				Scintilla.Lexing.WordChars = s;

			s = null;
			foreach (Configuration c in configList)
			{
				if (c.Lexing_LineCommentPrefix != null)
					s = c.Lexing_LineCommentPrefix;
			}
			if (s != null)
				Scintilla.Lexing.LineCommentPrefix = s;

			s = null;
			foreach (Configuration c in configList)
			{
				if (c.Lexing_StreamCommentPrefix != null)
					s = c.Lexing_StreamCommentPrefix;
			}
			if (s != null)
				Scintilla.Lexing.StreamCommentPrefix = s;

			s = null;	
			foreach (Configuration c in configList)
			{
				if (c.Lexing_StreamCommentSuffix != null)
					s = c.Lexing_StreamCommentSuffix;
			}
			if (s != null)
				Scintilla.Lexing.StreamCommentSufix = s;

			s = null;
			foreach (Configuration c in configList)
			{
				if (c.Lexing_Language != null)
					s = c.Lexing_Language;
			}
			
			if (s == null)
			{
				//	None of the configs specified a lexer. First let's see if
				//	we have a Language-Lexer map defined:
				if (Scintilla.Lexing.LexerLanguageMap.ContainsKey(_language))
				{
					s = Scintilla.Lexing.LexerLanguageMap[_language];
				}
				else
				{
					try
					{
						Enum.Parse(typeof(Lexer), _language, true);

						//	If we made it here, the language matches one of
						//	the lexer names, just use that.
						s = _language;
					}
					catch (ArgumentException) 
					{ 
						//	No match, oh well. Don't set the lexer.
					}
				}
			}
			Scintilla.Lexing.LexerName = s;

			LineCache? lc = null;
			foreach (Configuration c in configList)
			{
				if (c.LineWrap_LayoutCache.HasValue)
					lc = c.LineWrap_LayoutCache;
			}
			if (lc.HasValue)
				Scintilla.LineWrap.LayoutCache = lc.Value;

			WrapMode? wm = null;
			foreach (Configuration c in configList)
			{
				if (c.LineWrap_Mode.HasValue)
					wm = c.LineWrap_Mode;
			}
			if (wm.HasValue)
				Scintilla.LineWrap.Mode = wm.Value;
			

			i = null;
			foreach (Configuration c in configList)
			{
				if (c.LineWrap_PositionCacheSize.HasValue)
					i = c.LineWrap_PositionCacheSize;
			}
			if (i.HasValue)
				Scintilla.LineWrap.PositionCacheSize = i.Value;			
			
			i = null;
			foreach (Configuration c in configList)
			{
				if (c.LineWrap_StartIndent.HasValue)
					i = c.LineWrap_StartIndent;
			}
			if (i.HasValue)
				Scintilla.LineWrap.StartIndent = i.Value;


			WrapVisualFlag? wvf = null;
			foreach (Configuration c in configList)
			{
				if (c.LineWrap_VisualFlags.HasValue)
					wvf = c.LineWrap_VisualFlags;
			}
			if (wvf.HasValue)
				Scintilla.LineWrap.VisualFlags = wvf.Value;

			WrapVisualLocation? wvl = null;
			foreach (Configuration c in configList)
			{
				if (c.LineWrap_VisualFlagsLocation.HasValue)
					wvl = c.LineWrap_VisualFlagsLocation;
			}
			if (wvl.HasValue)
				Scintilla.LineWrap.VisualFlagsLocation = wvl.Value;
			

			co = Color.Empty;
			foreach (Configuration c in configList)
			{
				if (c.LongLines_EdgeColor != Color.Empty)
					co = c.LongLines_EdgeColor;

			}
			if (co != Color.Empty)
				Scintilla.LongLines.EdgeColor = co;


			i = null;
			foreach (Configuration c in configList)
			{
				if (c.LongLines_EdgeColumn.HasValue)
					i = c.LongLines_EdgeColumn;
			}
			if (i.HasValue)
				Scintilla.LongLines.EdgeColumn = i.Value;


			EdgeMode? em = null;
			foreach (Configuration c in configList)
			{
				if (c.LongLines_EdgeMode.HasValue)
					em = c.LongLines_EdgeMode;

			}
			if (em.HasValue)
				Scintilla.LongLines.EdgeMode = em.Value;

			
			if (_clearMargins)
				Scintilla.Margins.Reset();

			Dictionary<int, MarginConfig> margins = new Dictionary<int, MarginConfig>();
			foreach (Configuration c in configList)
			{
				if (c.Margin_List.Inherit.HasValue && !c.Margin_List.Inherit.Value)
					margins.Clear();

				foreach (MarginConfig mc in c.Margin_List)
				{
					
					if (!margins.ContainsKey(mc.Number) || (mc.Inherit.HasValue && !mc.Inherit.Value))
					{
						margins.Remove(mc.Number);
						margins.Add(mc.Number, mc);
					}
					else
					{	
						MarginConfig m = margins[mc.Number];

						if (mc.AutoToggleMarkerNumber.HasValue)
							m.AutoToggleMarkerNumber = mc.AutoToggleMarkerNumber.Value;

						if (mc.IsClickable.HasValue)
							m.IsClickable = mc.IsClickable.Value;

						if (mc.IsFoldMargin.HasValue)
							m.IsFoldMargin = mc.IsFoldMargin.Value;

						if (mc.IsMarkerMargin.HasValue)
							m.IsMarkerMargin = mc.IsMarkerMargin.Value;

						if (mc.Type.HasValue)
							m.Type = mc.Type.Value;

						if (mc.Width.HasValue)
							m.Width = mc.Width.Value;
					}
				}
			}

			foreach (MarginConfig mc in margins.Values)
			{
				Margin m = Scintilla.Margins[mc.Number];

				if (mc.AutoToggleMarkerNumber.HasValue)
					m.AutoToggleMarkerNumber = mc.AutoToggleMarkerNumber.Value;

				if (mc.IsClickable.HasValue)
					m.IsClickable = mc.IsClickable.Value;

				if (mc.IsFoldMargin.HasValue)
					m.IsFoldMargin = mc.IsFoldMargin.Value;

				if (mc.IsMarkerMargin.HasValue)
					m.IsMarkerMargin = mc.IsMarkerMargin.Value;

				if (mc.Type.HasValue)
					m.Type = mc.Type.Value;

				if (mc.Width.HasValue)
					m.Width = mc.Width.Value;
			}

			MarkersConfigList resolvedMarkers = new MarkersConfigList();
			foreach (Configuration c in configList)
			{
				if (c.Markers_List.Inherit.HasValue && !c.Markers_List.Inherit.Value)
					resolvedMarkers.Clear();

				foreach (MarkersConfig mc in c.Markers_List)
				{
					if (!resolvedMarkers.Contains(mc.Number.Value) || (mc.Inherit.HasValue && !mc.Inherit.Value))
					{
						resolvedMarkers.Remove(mc.Number.Value);
						resolvedMarkers.Add(mc);
					}
					else
					{
						if (!mc.Number.HasValue)
							mc.Number = (int)(MarkerOutline)Enum.Parse(typeof(MarkerOutline), mc.Name, true);

						MarkersConfig m = resolvedMarkers[mc.Number.Value];
						if (mc.Alpha.HasValue)
							m.Alpha = mc.Alpha;

						if (mc.BackColor != Color.Empty)
							m.BackColor = mc.BackColor;

						if (mc.ForeColor != Color.Empty)
							m.ForeColor = mc.ForeColor;

						if (mc.Symbol.HasValue)
							m.Symbol = mc.Symbol;
					}
				}
			}
					
			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Scrolling_EndAtLastLine.HasValue)
					b = c.Scrolling_EndAtLastLine;
			}
			if (b.HasValue)
				Scintilla.Scrolling.EndAtLastLine = b.Value;			

			i = null;
			foreach (Configuration c in configList)
			{
				if (c.Scrolling_HorizontalWidth.HasValue)
					i = c.Scrolling_HorizontalWidth;
			}
			if (i.HasValue)
				Scintilla.Scrolling.HorizontalWidth = i.Value;

			ScrollBars? sb = null;
			foreach (Configuration c in configList)
			{
				if (c.Scrolling_ScrollBars.HasValue)
					sb = c.Scrolling_ScrollBars;
			}
			if (sb.HasValue)
				Scintilla.Scrolling.ScrollBars = sb.Value;


			i = null;
			foreach (Configuration c in configList)
			{
				if (c.Scrolling_XOffset.HasValue)
					i = c.Scrolling_XOffset;
			}
			if (i.HasValue)
				Scintilla.Scrolling.XOffset = i.Value;


			co = Color.Empty;
			foreach (Configuration c in configList)
			{
				if (c.Selection_BackColor != Color.Empty)
					co = c.Selection_BackColor;

			}
			if (co != Color.Empty)
				Scintilla.Selection.BackColor = co;
			

			co = Color.Empty;
			foreach (Configuration c in configList)
			{
				if (c.Selection_BackColorUnfocused != Color.Empty)
					co = c.Selection_BackColorUnfocused;

			}
			if (co != Color.Empty)
				Scintilla.Selection.BackColorUnfocused = co;
			
			co = Color.Empty;
			foreach (Configuration c in configList)
			{
				if (c.Selection_ForeColor != Color.Empty)
					co = c.Selection_ForeColor;

			}
			if (co != Color.Empty)
				Scintilla.Selection.ForeColor = co;
			
			co = Color.Empty;
			foreach (Configuration c in configList)
			{
				if (c.Selection_ForeColorUnfocused != Color.Empty)
					co = c.Selection_ForeColorUnfocused;

			}
			if (co != Color.Empty)
				Scintilla.Selection.ForeColorUnfocused = co;


			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Selection_Hidden.HasValue)
					b = c.Selection_Hidden;
			}
			if (b.HasValue)
				Scintilla.Selection.Hidden = b.Value;	
		

			b = null;
			foreach (Configuration c in configList)
			{
				if (c.Selection_HideSelection.HasValue)
					b = c.Selection_HideSelection;
			}
			if (b.HasValue)
				Scintilla.Selection.HideSelection = b.Value;

			SelectionMode? selectionMode = null;
			foreach (Configuration c in configList)
			{
				if (c.Selection_Mode.HasValue)
					selectionMode = c.Selection_Mode;
			}
			if (selectionMode.HasValue)
				Scintilla.Selection.Mode = selectionMode.Value;


			b = null;
			foreach (Configuration c in configList)
			{
				if (c.UndoRedoIsUndoEnabled.HasValue)
					b = c.UndoRedoIsUndoEnabled;
			}
			if (b.HasValue)
				Scintilla.UndoRedo.IsUndoEnabled = b.Value;

			co = Color.Empty;
			foreach (Configuration c in configList)
			{
				if (c.WhiteSpace_BackColor != Color.Empty)
					co = c.WhiteSpace_BackColor;

			}
			if (co != Color.Empty)
				Scintilla.WhiteSpace.BackColor = co;


			co = Color.Empty;
			foreach (Configuration c in configList)
			{
				if (c.WhiteSpace_ForeColor != Color.Empty)
					co = c.WhiteSpace_ForeColor;
			}
			if (co != Color.Empty)
				Scintilla.WhiteSpace.ForeColor = co;

			WhiteSpaceMode? wsm = null;
			foreach (Configuration c in configList)
			{
				if (c.WhiteSpace_Mode.HasValue)
					wsm = c.WhiteSpace_Mode;
			}
			if (wsm.HasValue)
				Scintilla.WhiteSpace.Mode = wsm.Value;


			b = null;
			foreach (Configuration c in configList)
			{
				if (c.WhiteSpace_UseWhiteSpaceBackColor.HasValue)
					b = c.WhiteSpace_UseWhiteSpaceBackColor.HasValue;
			}
			if (b.HasValue)
				Scintilla.WhiteSpace.UseWhiteSpaceBackColor = b.Value;


			b = null;
			foreach (Configuration c in configList)
			{
				if (c.WhiteSpace_UseWhiteSpaceForeColor.HasValue)
					b = c.WhiteSpace_UseWhiteSpaceForeColor.HasValue;
			}
			if (co != Color.Empty)
				Scintilla.WhiteSpace.UseWhiteSpaceForeColor = b.Value;




			//	OK so we saved the best for last instead of going in
			//	strict lexical order. Styles! This is really the section
			//	that people care about most in the config, and is also
			//	the most complex.
			if (_clearStyles)
				Scintilla.Styles.Reset();

			i = null;
			foreach (Configuration c in configList)
			{
				if (c.Styles.Bits.HasValue)
					i = c.Styles.Bits;
			}
			if (i.HasValue)
				Scintilla.Styles.Bits = i.Value;

			Dictionary<string, int> styleNameMap =  Scintilla.Lexing.StyleNameMap;
			ResolvedStyleList resolvedStyles = new ResolvedStyleList();

			int _unmappedStyleNumber = -1;
			Dictionary<string, int> _unmappedStyleMap = new Dictionary<string,int>();
			foreach (Configuration c in configList)
			{
				if (c.Styles.Inherit.HasValue && !c.Styles.Inherit.Value)
					resolvedStyles.Clear();

				foreach (StyleConfig sc in c.Styles)
				{
					i = sc.Number;

					if (!i.HasValue)
					{
						if (!styleNameMap.ContainsKey(sc.Name))
						{
							if (_unmappedStyleMap.ContainsKey(sc.Name))
							{
								i = _unmappedStyleMap[sc.Name];
								sc.Number = i;
							}
							else
							{
								i = _unmappedStyleNumber--;
								sc.Number = i;

								_unmappedStyleMap[sc.Name] = sc.Number.Value;
							}							
						}
						else
						{
							i = styleNameMap[sc.Name];
							sc.Number = i;
						}
					}

					StyleConfig baseStyleConfig = null;
					if (!string.IsNullOrEmpty(sc.Name) && sc.Name.Contains("."))
					{
						baseStyleConfig = resolvedStyles.FindByName(sc.Name.Substring(sc.Name.IndexOf(".") + 1));
					}

					if (!resolvedStyles.ContainsKey(i.Value) || (sc.Inherit.HasValue && !sc.Inherit.Value))
					{
						resolvedStyles.Remove(i.Value);
						resolvedStyles.Add(i.Value, sc);
					}

					StyleConfig rs = resolvedStyles[i.Value];

					if (sc.BackColor != Color.Empty)
						rs.BackColor = sc.BackColor;
					else if (baseStyleConfig != null && baseStyleConfig.BackColor != Color.Empty)
						rs.BackColor = baseStyleConfig.BackColor;

					if (sc.Bold.HasValue)
						rs.Bold = sc.Bold.Value;
					else if (baseStyleConfig != null && baseStyleConfig.Bold.HasValue)
						rs.Bold = baseStyleConfig.Bold.Value;

					if (sc.Case.HasValue)
						rs.Case = sc.Case.Value;
					else if (baseStyleConfig != null && baseStyleConfig.Case.HasValue)
						rs.Case = baseStyleConfig.Case.Value;

					if (sc.CharacterSet.HasValue)
						rs.CharacterSet = sc.CharacterSet.Value;
					else if (baseStyleConfig != null && baseStyleConfig.CharacterSet.HasValue)
						rs.CharacterSet = baseStyleConfig.CharacterSet.Value;

					if (sc.FontName != null)
						rs.FontName = sc.FontName;
					else if (baseStyleConfig != null && baseStyleConfig.FontName != null)
						rs.FontName = baseStyleConfig.FontName;

					if (sc.ForeColor != Color.Empty)
						rs.ForeColor = sc.ForeColor;
					else if (baseStyleConfig != null && baseStyleConfig.ForeColor != Color.Empty)
						rs.ForeColor = baseStyleConfig.ForeColor;

					if (sc.IsChangeable.HasValue)
						rs.IsChangeable = sc.IsChangeable.Value;
					else if (baseStyleConfig != null && baseStyleConfig.IsChangeable.HasValue)
						rs.IsChangeable = baseStyleConfig.IsChangeable.Value;

					if (sc.IsHotspot.HasValue)
						rs.IsHotspot = sc.IsHotspot.Value;
					else if (baseStyleConfig != null && baseStyleConfig.IsHotspot.HasValue)
						rs.IsHotspot = baseStyleConfig.IsHotspot.Value;

					if (sc.IsSelectionEolFilled.HasValue)
						rs.IsSelectionEolFilled = sc.IsSelectionEolFilled.Value;
					else if (baseStyleConfig != null && baseStyleConfig.IsSelectionEolFilled.HasValue)
						rs.IsSelectionEolFilled = baseStyleConfig.IsSelectionEolFilled.Value;

					if (sc.IsVisible.HasValue)
						rs.IsVisible = sc.IsVisible.Value;
					else if (baseStyleConfig != null && baseStyleConfig.IsVisible.HasValue)
						rs.IsVisible = baseStyleConfig.IsVisible.Value;

					if (sc.Italic.HasValue)
						rs.Italic = sc.Italic.Value;
					else if (baseStyleConfig != null && baseStyleConfig.Italic.HasValue)
						rs.Italic = baseStyleConfig.Italic.Value;

					if (sc.Size.HasValue)
						rs.Size = sc.Size.Value;
					else if (baseStyleConfig != null && baseStyleConfig.Size.HasValue)
						rs.Size = baseStyleConfig.Size.Value;

					if (sc.Underline.HasValue)
						rs.Underline = sc.Underline.Value;
					else if (baseStyleConfig != null && baseStyleConfig.Underline.HasValue)
						rs.Underline = baseStyleConfig.Underline.Value;
				}


			}
			//	If a Default styles exist we want them at the top of the list because
			//	it needs to be applied first, then StyleClearAll() called so that all
			//	other styles will "inherit" this style. Then the other styles will 
			//	override the default with any defined properties.
			StyleConfig[] arr = new StyleConfig[resolvedStyles.Count];
			resolvedStyles.Values.CopyTo(arr, 0);
			Array.Sort<StyleConfig>(arr, new Comparison<StyleConfig>(delegate(StyleConfig sc1, StyleConfig sc2)
			{
				int v1 = sc1.Number.Value == Constants.STYLE_DEFAULT ? -1 : sc1.Number.Value;
				int v2 = sc2.Number.Value == Constants.STYLE_DEFAULT ? -1 : sc2.Number.Value;

				if (v1 < v2)
					return -1;
				else if (v2 < v1)
					return 1;

				return 0;
			}));
			

			foreach (StyleConfig sc in arr)
			{
				if (sc.Number < 0)
					continue;

				Style style = Scintilla.Styles[sc.Number.Value];

				if (sc.BackColor != Color.Empty)
					style.BackColor = sc.BackColor;

				if (sc.Bold.HasValue)
					style.Bold = sc.Bold.Value;

				if (sc.Case.HasValue)
					style.Case = sc.Case.Value;

				if (sc.CharacterSet.HasValue)
					style.CharacterSet = sc.CharacterSet.Value;

				if (sc.FontName != null)
					style.FontName = sc.FontName;

				if (sc.ForeColor != Color.Empty)
					style.ForeColor = sc.ForeColor;

				if (sc.IsChangeable.HasValue)
					style.IsChangeable = sc.IsChangeable.Value;

				if (sc.IsHotspot.HasValue)
					style.IsHotspot = sc.IsHotspot.Value;

				if (sc.IsSelectionEolFilled.HasValue)
					style.IsSelectionEolFilled = sc.IsSelectionEolFilled.Value;

				if (sc.IsVisible.HasValue)
					style.IsVisible = sc.IsVisible.Value;

				if (sc.Italic.HasValue)
					style.Italic = sc.Italic.Value;

				if (sc.Size.HasValue)
					style.Size = sc.Size.Value;

				if (sc.Underline.HasValue)
					style.Underline = sc.Underline.Value;

				if (sc.Number == Constants.STYLE_DEFAULT)
					Scintilla.Styles.ClearAll();
			}
			
		}



	}

	public enum ConfigurationLoadOrder
	{
		BuiltInCustomUser,
		BuiltInUserCustom,
		CustomBuiltInUser,
		CustomUserBuiltIn,		
		UserBuiltInCustom,
		UserCustomBuiltIn
	}
}

