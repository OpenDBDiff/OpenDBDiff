using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Permissions;
using System.Text;
using DBDiff.Scintilla.Configuration;

namespace DBDiff.Scintilla
{
	[DefaultBindingProperty("Text"), DefaultProperty("Text"), DefaultEvent("DocumentChanged")]
	public partial class Scintilla : System.Windows.Forms.Control, INativeScintilla, ISupportInitialize
	{
		public const string DefaultDllName = "DbDiff.SciLexer.dll";
		private string _sciLexerDllName = null;
		

		#region Property Bags
		private Dictionary<string, Color> _colorBag = new Dictionary<string, Color>();
		internal Dictionary<string, Color> ColorBag { get { return _colorBag; } }

		private Hashtable _propertyBag = new Hashtable();
		internal Hashtable PropertyBag { get { return _propertyBag; } }

		#endregion

		#region Constructor / Dispose

		public Scintilla()
		{
			if (IntPtr.Size == 4)
                _sciLexerDllName = "DbDiff.SciLexer32.dll";
            else
                _sciLexerDllName = "DbDiff.SciLexer64.dll";
			_ns = (INativeScintilla)this;

			// Set up default encoding
			_encoding = Encoding.GetEncoding(NativeInterface.GetCodePage());

			//	Ensure all style values have at least defaults
			_ns.StyleClearAll();

			_caret					= new CaretInfo(this);
			_lines					= new LinesCollection(this);
			_selection				= new Selection(this);
			_indicators				= new IndicatorCollection(this);
			_margins				= new MarginCollection(this);
			_scrolling				= new Scrolling(this);
			_whiteSpace				= new WhiteSpace(this);
			_endOfLine				= new EndOfLine(this);
			_clipboard				= new Clipboard(this);
			_undoRedo				= new UndoRedo(this);
			_dropMarkers			= new DropMarkers(this);
			_hotspotStyle			= new HotspotStyle(this);
			_callTip				= new CallTip(this);
			_styles					= new StyleCollection(this);
			_indentation			= new Indentation(this);
			_documentHandler		= new DocumentHandler(this);
			_lineWrap				= new LineWrap(this);
			_lexing					= new Lexing(this);
			_longLines				= new LongLines(this);
			_commands				= new Commands(this);
			_configurationManager	= new ConfigurationManager(this);
			_printing				= new Printing(this);
			_documentNavigation		= new DocumentNavigation(this);
			_goto					= new GoTo(this);


			_helpers.AddRange(new ScintillaHelperBase[] 
			{ 
				_caret, 
				_lines, 
				_selection,
				_indicators, 
				_margins,
				_scrolling,
				_whiteSpace,
				_endOfLine,
				_clipboard,
				_undoRedo,
				_dropMarkers,
				_hotspotStyle,
				_styles,
				_indentation,
				_documentHandler,
				_lineWrap,
				_lexing,
				_longLines,
				_commands,
				_configurationManager,
				_printing,
				_documentNavigation,
				_goto
			});


			//	Changing the Default values from Scintilla's default Black on White
			//	to platform defaults for Edits
			BackColor = SystemColors.Window;
			ForeColor = SystemColors.WindowText;
		}

		protected override void Dispose(bool disposing)
		{
			foreach (ScintillaHelperBase heler in _helpers)
			{
				heler.Dispose();
			}
			base.Dispose(disposing);
		}
		#endregion

		#region Protected Control Overrides

		protected override void WndProc(ref Message m)
		{
			if ((int)m.Msg == NativeMethods.WM_PAINT)
			{
				//	I tried toggling the ControlStyles.UserPaint flag and sending the message
				//	to both base.WndProc and DefWndProc in order to get the best of both worlds,
				//	Scintilla Paints as normal and .NET fires the Paint Event with the proper
				//	clipping regions and such. This didn't work too well, I kept getting weird
				//	phantom paints, or sometimes the .NET paint events would seem to get painted
				//	over by Scintilla. This technique I use below seems to work perfectly.
				
				base.WndProc(ref m);

				if(_isCustomPaintingEnabled)
				{
					RECT r;
					if (!NativeMethods.GetUpdateRect(Handle, out r, false))
						r = ClientRectangle;
					
					Graphics g = CreateGraphics();
					g.SetClip(r);
					
					OnPaint(new PaintEventArgs(CreateGraphics(), r));
				}
				return;
			}
			else if ((m.Msg) == NativeMethods.WM_DROPFILES)
			{
				handleFileDrop(m.WParam);
				return;
			}

			//	Uh-oh. Code based on undocumented unsupported .NET behavior coming up!
			//	Windows Forms Sends Notify messages back to the originating
			//	control ORed with 0x2000. This is way cool becuase we can listen for
			//	WM_NOTIFY messages originating form our own hWnd (from Scintilla)
			if ((m.Msg ^ 0x2000) != NativeMethods.WM_NOTIFY)
			{
				switch (m.Msg)
				{
					case NativeMethods.WM_HSCROLL:
					case NativeMethods.WM_VSCROLL:
						FireScroll(ref m);

						//	FireOnScroll calls WndProc so no need to call it again
						return;
				}
				base.WndProc(ref m);
				return;
			}
			else if ((int)m.Msg >= 10000)
			{
				_commands.Execute((BindableCommand)m.Msg);
				return;
			}
			

			SCNotification scn				= (SCNotification)Marshal.PtrToStructure(m. LParam, typeof(SCNotification));
			NativeScintillaEventArgs nsea	= new NativeScintillaEventArgs(m, scn);
			
			switch(scn.nmhdr.code)
			{
				case Constants.SCN_CALLTIPCLICK:
					FireCallTipClick(nsea);
					break;

				case Constants.SCN_CHARADDED:
					FireCharAdded(nsea);
					break;

				case Constants.SCEN_CHANGE:
					FireChange(nsea);
					break;

				case Constants.SCN_DOUBLECLICK:
					FireDoubleClick(nsea);
					break;

				case Constants.SCN_DWELLEND:
					FireDwellEnd(nsea);
					break;

				case Constants.SCN_DWELLSTART:
					FireDwellStart(nsea);
					break;

				case Constants.SCN_HOTSPOTCLICK:
					FireHotSpotClick(nsea);
					break;

				case Constants.SCN_HOTSPOTDOUBLECLICK:
					FireHotSpotDoubleclick(nsea);
					break;

				case Constants.SCN_INDICATORCLICK:
					FireIndicatorClick(nsea);
					break;

				case Constants.SCN_INDICATORRELEASE:
					FireIndicatorRelease(nsea);
					break;

				case Constants.SCN_KEY:
					FireKey(nsea);
					break;

				case Constants.SCN_MACRORECORD:
					FireMacroRecord(nsea);
					break;

				case Constants.SCN_MARGINCLICK:
					FireMarginClick(nsea);
					break;

				case Constants.SCN_MODIFIED:
					FireModified(nsea);
					break;

				case Constants.SCN_MODIFYATTEMPTRO:
					FireModifyAttemptRO(nsea);
					break;

				case Constants.SCN_NEEDSHOWN:
					FireNeedShown(nsea);
					break;

				case Constants.SCN_PAINTED:
					FirePainted(nsea);
					break;

				case Constants.SCN_SAVEPOINTLEFT:
					FireSavePointLeft(nsea);
					break;

				case Constants.SCN_SAVEPOINTREACHED:
					FireSavePointReached(nsea);
					break;

				case Constants.SCN_STYLENEEDED:
					FireStyleNeeded(nsea);
					break;

				case Constants.SCN_UPDATEUI:
					FireUpdateUI(nsea);
					break;

				case Constants.SCN_URIDROPPED:
					FireUriDropped(nsea);
					break;

				case Constants.SCN_USERLISTSELECTION:
					FireUserListSelection(nsea);
					break;

				case Constants.SCN_ZOOM:
					FireZoom(nsea);
					break;

			}

			base.WndProc(ref m);
		}

		protected override CreateParams CreateParams
		{
			[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				//	Otherwise Scintilla won't paint. When UserPaint is set to
				//	true the base Class (Control) eats the WM_PAINT message.
				//	Of course when this set to false we can't use the Paint
				//	events. This is why I'm relying on the Paint notification
				//	sent from scintilla to paint the Marker Arrows.
				SetStyle(ControlStyles.UserPaint, false);

				//	Registers the Scintilla Window Class
				//	I'm relying on the fact that a version specific renamed
				//	SciLexer exists either in the Current Dir or a global path
				//	(See LoadLibrary Windows API Search Rules)
				NativeMethods.LoadLibrary(_sciLexerDllName);

				//	Tell Windows Forms to create a Scintilla
				//	derived Window Class for this control
				CreateParams cp = base.CreateParams;
				cp.ClassName = "Scintilla";

				return cp;
			}
		}

		

		protected override bool IsInputKey(Keys keyData)
		{
			if((keyData & Keys.Shift) != Keys.None)
				keyData ^= Keys.Shift;

			switch(keyData)
			{
				case Keys.Tab:
					return _acceptsTab;
				case Keys.Enter:
					return _acceptsReturn;
				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
				case Keys.F:

					return true;
			}

			return base.IsInputKey(keyData);
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (_supressControlCharacters && (int)e.KeyChar < 32)
				e.Handled = true;

			base.OnKeyPress(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled)
				e.SuppressKeyPress = _commands.ProcessKey(e);
		}

		internal void FireKeyDown(KeyEventArgs e)
		{
			OnKeyDown(e);
		}

		protected override bool ProcessKeyMessage(ref Message m)
		{
			//	For some reason IsInputKey isn't working for
			//	Key.Enter. This seems to make it work as expected
			if((int)m.WParam == (int)Keys.Enter && !AcceptsReturn)
			{
				return true;
			}
			else
			{
				return base.ProcessKeyMessage(ref m);
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(200,100);
			}
		}

		protected override void OnLostFocus(EventArgs e)
		{
			if(Selection.HideSelection)
				_ns.HideSelection(true);

			_ns.SetSelBack(true, Utilities.ColorToRgb(Selection.BackColorUnfocused));
			_ns.SetSelFore(true, Utilities.ColorToRgb(Selection.ForeColorUnfocused));

			base.OnLostFocus(e);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			if (!Selection.Hidden)
				_ns.HideSelection(false);

			_ns.SetSelBack(true, Utilities.ColorToRgb(Selection.BackColor));
			_ns.SetSelFore(true, Utilities.ColorToRgb(Selection.ForeColor));

			base.OnGotFocus(e);
		}

		/// <summary>
		/// Provides the support for code block selection
		/// </summary>
		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);

			if (_isBraceMatching)
			{
				int position = CurrentPos - 1,
					   bracePosStart = -1,
					   bracePosEnd = -1;

				char character = (char)CharAt(position);

				switch (character)
				{
					case '{':
					case '(':
					case '[':
						if (!this.PositionIsOnComment(position))
						{
							bracePosStart = position;
							bracePosEnd = _ns.BraceMatch(position, 0) + 1;
							_selection.Start = bracePosStart;
							_selection.End = bracePosEnd;
						}
						break;
				}
			}
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			OnLoad(EventArgs.Empty);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			paintRanges(e.Graphics);


		}
		#endregion

		#region Public Properties
		
		#region AcceptsReturn

		private bool _acceptsReturn = true;

		[DefaultValue(true), Category("Behavior")]
		public bool AcceptsReturn
		{
			get
			{
				return _acceptsReturn;
			}
			set
			{
				_acceptsReturn = value;
			}
		}
		#endregion

		#region AcceptsTab
		private bool _acceptsTab = true;

		[DefaultValue(true), Category("Behavior")]
		public bool AcceptsTab
		{
			get
			{
				return _acceptsTab;
			}
			set
			{
				_acceptsTab = value;
			}
		}
		#endregion

		#region AllowDrop
		private bool _allowDrop = false;
		public override bool AllowDrop
		{
			get
			{
				return _allowDrop;
			}
			set
			{
				NativeMethods.DragAcceptFiles(Handle, value);
				_allowDrop = value;
			}
		} 
		#endregion		

		#region BackColor

		public override Color BackColor
		{
			get
			{
				if (_colorBag.ContainsKey("BackColor"))
					return _colorBag["BackColor"];

				return SystemColors.Window;
			}

			set
			{
				Color currentColor = BackColor;

				if (value == SystemColors.Window)
					_colorBag.Remove("BackColor");
				else
					_colorBag["BackColor"] = value;

				

				if (_useBackColor)
				{
					for (int i = 0; i < 128; i++)
						if (i != (int)StylesCommon.LineNumber)
							Styles[i].BackColor = value;
				}
				

				
			}
		}


		private bool ShouldSerializeBackColor()
		{
			return BackColor != SystemColors.Window;
		}

		public override void ResetBackColor()
		{
			BackColor = SystemColors.Window;
		}
		#endregion

		#region CallTip
		private CallTip _callTip;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public CallTip CallTip
		{
			get
			{
				return _callTip;
			}
			set
			{
				_callTip = value;
			}
		}

		private bool ShouldSerializeCallTip()
		{
			return _callTip.ShouldSerialize();
		} 
		#endregion

		#region Caret
		private CaretInfo _caret;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public CaretInfo Caret
		{
			get
			{
				return _caret;
			}
		}

		private bool ShouldSerializeCaret()
		{
			return _caret.ShouldSerialize();
		}

		#endregion

		#region Clipboard
		private Clipboard _clipboard;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public Clipboard Clipboard
		{
			get
			{
				return _clipboard;
			}
		}

		private bool ShouldSerializeClipboard()
		{
			return _clipboard.ShouldSerialize();
		} 
		#endregion

		#region CurrentPos
		[Browsable(false)]
		public int CurrentPos
		{
			get
			{
				return NativeInterface.GetCurrentPos();
			}
			set
			{
				NativeInterface.GotoPos(value);
			}
		}
		#endregion

		#region Commands
		private Commands _commands;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public Commands Commands
		{
			get
			{
				return _commands;
			}
			set
			{
				_commands = value;
			}
		}

		private bool ShouldSerializeCommands()
		{
			return _commands.ShouldSerialize();
		}
		#endregion

		#region ConfigurationManager
		private Configuration.ConfigurationManager _configurationManager;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public Configuration.ConfigurationManager ConfigurationManager
		{
			get
			{
				return _configurationManager;
			}
			set
			{
				_configurationManager = value;
			}
		}

		private bool ShouldSerializeConfigurationManager()
		{
			return _configurationManager.ShouldSerialize();
		}
		#endregion

		#region DocumentHandler
		private DocumentHandler _documentHandler;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DocumentHandler DocumentHandler
		{
			get
			{
				return _documentHandler;
			}
			set
			{
				_documentHandler = value;
			}
		} 
		#endregion

		#region DocumentNavigation
		private DocumentNavigation _documentNavigation;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public DocumentNavigation DocumentNavigation
		{
			get
			{
				return _documentNavigation;
			}
			set
			{
				_documentNavigation = value;
			}
		}

		private bool ShouldSerializeDocumentNavigation()
		{
			return _documentNavigation.ShouldSerialize();
		}

		#endregion

		#region DropMarkers
		private DropMarkers _dropMarkers;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public DropMarkers DropMarkers
		{
			get
			{
				return _dropMarkers;
			}
		}

		private bool ShouldSerializeDropMarkers()
		{
			return _dropMarkers.ShouldSerialize();
		}
		#endregion

		#region EndOfLine
		private EndOfLine _endOfLine;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public EndOfLine EndOfLine
		{
			get
			{
				return _endOfLine;
			}
			set
			{
				_endOfLine = value;
			}
		}

		private bool ShouldSerializeEndOfLine()
		{
			return _endOfLine.ShouldSerialize();
		}

		#endregion

		#region Encoding
		private Encoding _encoding;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Encoding Encoding
		{
			get
			{
				return _encoding;
			}
			set
			{
				_encoding = value;
			}
		} 
		#endregion				

		#region Font
		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				if (value == null)
					value = Parent.Font;

				Font currentFont = base.Font;

				if(_useFont)
				{
					for (int i = 0; i < 32; i++)
						if (i != (int)StylesCommon.CallTip)
							Styles[i].Font = value;
				}

				if(!base.Font.Equals(value))
					base.Font = value;
			}
		}

		protected override void OnFontChanged(EventArgs e)
		{
			//	We're doing this becuase when the Ambient font
			//	changes this method is called but the property
			//	setter for font isn't and we need to set the
			//	Default Style on Scintilla
			Font = Font;
			base.OnFontChanged(e);
		}

		public override void ResetFont()
		{
			Font = Parent.Font;
		}

		private bool ShouldSerializeFont()
		{
			return Font != Parent.Font;
		}
		#endregion

		#region ForeColor
		public override Color ForeColor
		{
			get
			{
				if (_colorBag.ContainsKey("ForeColor"))
					return _colorBag["ForeColor"];

				return base.ForeColor;
			}
			set
			{
				Color currentForeColor = ForeColor;

				if (value == SystemColors.WindowText)
					_colorBag.Remove("ForeColor");
				else
					_colorBag["ForeColor"] = value;

				if(_useForeColor)
				{
					for (int i = 0; i < 32; i++)
						if (i != (int)StylesCommon.LineNumber)
							Styles[i].ForeColor = value;
				}

				base.ForeColor = value;
			}
		}

		public virtual bool ShouldSerializeForeColor()
		{
			return ForeColor != SystemColors.WindowText;
		}

		public override void  ResetForeColor()
		{
			ForeColor = SystemColors.WindowText;
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			ForeColor = base.ForeColor;
			base.OnForeColorChanged(e);
		}

		#endregion

		#region GoTo
		private GoTo _goto;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GoTo GoTo
		{
			get
			{
				return _goto;
			}
			set
			{
				_goto = value;
			}
		}
		#endregion

		#region HotspotStyle
		private HotspotStyle _hotspotStyle;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public HotspotStyle HotspotStyle
		{
			get
			{
				return _hotspotStyle;
			}
			set
			{
				_hotspotStyle = value;
			}
		}

		private bool ShouldSerializeHotspotStyle()
		{
			return _hotspotStyle.ShouldSerialize();
		} 
		#endregion

		#region Indicators
		private IndicatorCollection _indicators;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IndicatorCollection Indicators
		{
			get { return _indicators; }
		} 
		#endregion

		#region Indentation
		private Indentation _indentation;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public Indentation Indentation
		{
			get
			{
				return _indentation;
			}
			set
			{
				_indentation = value;
			}
		}

		private bool ShouldSerializeIndentation()
		{
			return _indentation.ShouldSerialize();
		} 
		#endregion

		#region IsBraceMatching
		/// <summary>
		/// Enables the brace matching from current position.
		/// </summary>
		private bool _isBraceMatching = false;
		[DefaultValue(false), Category("Behavior")]
		public bool IsBraceMatching
		{
			get { return _isBraceMatching; }
			set { _isBraceMatching = value; }
		}



		/// <summary>
		/// Custom way to find the matching brace when BraceMatch() does not work
		/// </summary>
		internal int SafeBraceMatch(int position)
		{
			int match = this.CharAt(position);
			int toMatch = 0;
			int length = TextLength;
			int ch;
			int sub = 0;
			Lexer lexer = _lexing.Lexer;
			_lexing.Colorize(0, -1);
			bool comment = PositionIsOnComment(position, lexer);
			switch (match)
			{
				case '{':
					toMatch = '}';
					goto down;
				case '(':
					toMatch = ')';
					goto down;
				case '[':
					toMatch = ']';
					goto down;
				case '}':
					toMatch = '{';
					goto up;
				case ')':
					toMatch = '(';
					goto up;
				case ']':
					toMatch = '[';
					goto up;
			}
			return -1;
		// search up
		up:
			while (position >= 0)
			{
				position--;
				ch = CharAt(position);
				if (ch == match)
				{
					if (comment == PositionIsOnComment(position, lexer)) sub++;
				}
				else if (ch == toMatch && comment == PositionIsOnComment(position, lexer))
				{
					sub--;
					if (sub < 0) return position;
				}
			}
			return -1;
		// search down
		down:
			while (position < length)
			{
				position++;
				ch = CharAt(position);
				if (ch == match)
				{
					if (comment == PositionIsOnComment(position, lexer)) sub++;
				}
				else if (ch == toMatch && comment == PositionIsOnComment(position, lexer))
				{
					sub--;
					if (sub < 0) return position;
				}
			}
			return -1;
		}
		#endregion

		#region IsCustomPaintingEnabled
		private bool _isCustomPaintingEnabled = true;
		[DefaultValue(true), Category("Behavior")]
		public bool IsCustomPaintingEnabled
		{
			get
			{
				return _isCustomPaintingEnabled;
			}
			set
			{
				_isCustomPaintingEnabled = value;
			}
		}

		#endregion

		#region IsDirty
		private bool _isDirty = false;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDirty
		{ 
			get
			{
				return _isDirty;
			}
			set
			{
				if (value == _isDirty)
					return;

				_isDirty = value;

				//	I don't really know of any built in way to force
				//	the SavePointLeft message to fire in Scintilla.
				//	It's also too smart for it's own good, I tried
				//	doing some non-operations like appending an
				//	empty string.
				if (value)
				{
					bool oldIsUndoEnabled = _undoRedo.IsUndoEnabled;
					_undoRedo.IsUndoEnabled = false;
					this.AppendText(" ").Text = "";
					_undoRedo.IsUndoEnabled = oldIsUndoEnabled;
				}
				else
					NativeInterface.SetSavePoint();
			}
		}

		#endregion

		#region IsReadOnly
		[DefaultValue(false), Category("Behavior")]
		public bool IsReadOnly
		{
			get
			{
				return _ns.GetReadOnly();

			}
			set
			{
				_ns.SetReadOnly(value);
			}
		}
		#endregion

		#region Lexing
		private Lexing _lexing;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public Lexing Lexing
		{
			get
			{
				return _lexing;
			}
			set
			{
				_lexing = value;
			}
		}

		private bool ShouldSerializeLexing()
		{
			return _lexing.ShouldSerialize();
		} 
		#endregion

		#region Lines
		private LinesCollection _lines;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LinesCollection Lines
		{
			get
			{

				return _lines;
			}
		}

		#endregion

		#region LineWrap
		private LineWrap _lineWrap;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public LineWrap LineWrap
		{
			get
			{
				return _lineWrap;
			}
			set
			{
				_lineWrap = value;
			}
		}

		private bool ShouldSerializeLineWrap()
		{
			return LineWrap.ShouldSerialize();
		}

		#endregion

		#region LongLines
		private LongLines _longLines;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public LongLines LongLines
		{
			get
			{
				return _longLines;
			}
			set
			{
				_longLines = value;
			}
		}

		private bool ShouldSerializeLongLines()
		{
			return _longLines.ShouldSerialize();
		} 
		#endregion

		#region ManagedRanges
		private List<ManagedRange> _managedRanges = new List<ManagedRange>();
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<ManagedRange> ManagedRanges
		{
			get { return _managedRanges; }
		}
		#endregion

		#region Margins
		private MarginCollection _margins;
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public MarginCollection Margins
		{
			get
			{
				return _margins;
			}
		}

		private bool ShouldSerializeMargins()
		{
			return _margins.ShouldSerialize();
		}

		private void ResetMargins()
		{
			_margins.Reset();
		}
		#endregion

		#region MatchBraces
		private bool _matchBraces = true;
		[DefaultValue(true), Category("Behavior")]
		public bool MatchBraces
		{
			get
			{
				return _matchBraces;
			}
			set
			{
				_matchBraces = value;

				//	Clear any active Brace matching that may exist
				if(!value)
					_ns.BraceHighlight(-1, -1);
			}
		}

		#endregion

		#region Modified
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Modified
		{
			get
			{
				return NativeInterface.GetModify();
			}
		}

		#endregion

		#region MouseDownCaptures
		[DefaultValue(true), Category("Behavior")]
		public bool MouseDownCaptures
		{
			get
			{
				return NativeInterface.GetMouseDownCaptures();
			}
			set
			{
				NativeInterface.SetMouseDownCaptures(value);
			}
		} 
		#endregion

		#region Native Interface
		private INativeScintilla _ns;

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public INativeScintilla NativeInterface
		{
			get
			{
				return this as INativeScintilla;
			}
		}
		#endregion

		#region OverType
		
        [DefaultValue(false), Category("Behavior")]
		public bool OverType
		{
			get
			{
				return _ns.GetOvertype();
			}
			set
			{
				_ns.SetOvertype(value);
			}
		}
		#endregion

		#region Printing
		private Printing _printing;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Layout")]
		public Printing Printing
		{
			get
			{
				return _printing;
			}
			set
			{
				_printing = value;
			}
		}
		private bool ShouldSerializePrinting()
		{
			return _printing.ShouldSerialize();
		} 
		#endregion

		#region RawText
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		unsafe public byte[] RawText
		{
			get
			{
				int length = NativeInterface.GetTextLength() + 1;

				//	May as well avoid all the crap below if we know what the outcome
				//	is going to be :)
				if (length == 1)
					return new byte[] { 0 };

				//  Allocate a buffer the size of the string + 1 for 
				//  the NULL terminator. Scintilla always sets this
				//  regardless of the encoding
				byte[] buffer = new byte[length];

				//  Get a direct pointer to the the head of the buffer
				//  to pass to the message along with the wParam. 
				//  Scintilla will fill the buffer with string data.
				fixed (byte* bp = buffer)
				{
					_ns.SendMessageDirect(Constants.SCI_GETTEXT, (IntPtr)length, (IntPtr)bp);
					return buffer;
				}
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					_ns.ClearAll();
				}
				else
				{
					//	This byte[] HAS to be NULL terminated or who knows how big 
					//	of an overrun we'll have on our hands
					if (value[value.Length - 1] != 0)
					{
						//	I hate to have to do this becuase it can be very inefficient.
						//	It can probably be done much better by the client app
						Array.Resize<byte>(ref value, value.Length + 1);
						value[value.Length - 1] = 0;
					}
					fixed (byte* bp = value)
						_ns.SendMessageDirect(Constants.SCI_SETTEXT, IntPtr.Zero, (IntPtr)bp);
				}
			}
		} 
		#endregion

		#region Scrolling
		private Scrolling _scrolling;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Layout")]
		public Scrolling Scrolling
		{
			get
			{
				return _scrolling;
			}
			set
			{
				_scrolling = value;
			}
		}

		private bool ShouldSerializeScrolling()
		{
			return _scrolling.ShouldSerialize();
		}

		#endregion

		#region Selection
		private Selection _selection;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public Selection Selection
		{
			get
			{
				return _selection;
			}
		}

		private bool ShouldSerializeSelection()
		{
			return _selection.ShouldSerialize();
		}

		#endregion		

		#region Styles
		private StyleCollection _styles;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public StyleCollection Styles
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

		private bool ShouldSerializeStyles()
		{
			return _styles.ShouldSerialize();
		} 
		#endregion

		#region SupressControlCharacters
		private bool _supressControlCharacters = true;

		[DefaultValue(true), Category("Behavior")]
		public bool SupressControlCharacters
		{
			get
			{
				return _supressControlCharacters;
			}
			set
			{
				_supressControlCharacters = value;
			}
		}
		#endregion

		#region Text
		public override string Text
		{
			get
			{
				string s;
				_ns.GetText(_ns.GetLength() + 1, out s);
				return s;
			}

			set
			{
			
				if(string.IsNullOrEmpty(value))
					_ns.ClearAll();
				else
					_ns.SetText(value);
			}
		}
		#endregion

		#region TextLength
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TextLength
		{
			get
			{
				return NativeInterface.GetTextLength();
			}
		}
		#endregion

		#region UndoRedo
		private UndoRedo _undoRedo;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public UndoRedo UndoRedo
		{
			get
			{
				return _undoRedo;
			}
		}

		public bool ShouldSerializeUndoRedo()
		{
			return _undoRedo.ShouldSerialize();
		} 
		#endregion

		#region UseForeColor
		private bool _useForeColor = false;
		[Category("Appearance"), DefaultValue(false)]
		public bool UseForeColor
		{
			get
			{
				return _useForeColor;
			}
			set
			{
				_useForeColor = value;

				if (value)
					ForeColor = ForeColor;
			}
		}

		#endregion

		#region UseFont
		private bool _useFont = false;
		[Category("Appearance"), DefaultValue(false)]
		public bool UseFont
		{
			get
			{
				return _useFont;
			}
			set
			{
				_useFont = value;

				if (value)
					Font = Font;
			}
		} 
		#endregion

		#region UseBackColor
		private bool _useBackColor = false;
		[Category("Appearance"), DefaultValue(false)]
		public bool UseBackColor
		{
			get
			{
				return _useBackColor;
			}
			set
			{
				_useBackColor = value;
				if (value)
					BackColor = BackColor;
			}
		} 
		#endregion

		#region UseWaitCursor
		public new bool UseWaitCursor
		{
			get
			{
				return base.UseWaitCursor;
			}
			set
			{
				base.UseWaitCursor = value;

				if (value)
					NativeInterface.SetCursor(Constants.SC_CURSORWAIT);
				else
					NativeInterface.SetCursor(Constants.SC_CURSORNORMAL);
			}
		} 
		#endregion

		#region WhiteSpace
		private WhiteSpace _whiteSpace;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public WhiteSpace WhiteSpace
		{
			get
			{
				return _whiteSpace;
			}
			set
			{
				_whiteSpace = value;
			}
		}

		private bool ShouldSerializeWhiteSpace()
		{
			return _whiteSpace.ShouldSerialize();
		}

		#endregion

		#region Zoom
		[DefaultValue(0), Category("Appearance")]
		public int Zoom
		{
			get
			{
				return _ns.GetZoom();
			}
			set
			{
				_ns.SetZoom(value);
			}
		}
		#endregion

		#endregion

		#region Private Methods

		private unsafe void handleFileDrop(IntPtr hDrop)
		{
			int nfiles = NativeMethods.DragQueryFileA(hDrop, 0xffffffff, (IntPtr)null, 0);
			byte[] buffer = new byte[1024];

			List<string> files = new List<string>();
			for (uint i = 0; i < nfiles; i++)
			{
				fixed (byte* b = buffer)
				{
					NativeMethods.DragQueryFileA(hDrop, i, (IntPtr)b, 1024);
					files.Add(Utilities.MarshalStr((IntPtr)b));
				}
			}
			NativeMethods.DragFinish(hDrop);

			OnFileDrop(new FileDropEventArgs(files.ToArray()));
		}


		private List<ManagedRange> managedRangesInRange(int firstPos, int lastPos)
		{
			//	TODO: look into optimizing this so that it isn't a linear
			//	search. This is fine for a few markers per document but
			//	can be greatly improved if there are a large # of markers
			List<ManagedRange> ret = new List<ManagedRange>();
			foreach(ManagedRange mr in _managedRanges)
				if(mr.Start >= firstPos && mr.Start <= lastPos)
					ret.Add(mr);

			return ret;
		}


		private void paintRanges(Graphics g)
		{
			//	First we want to get the range (in positions) of what
			//	will be painted so that we know which markers to paint
			int firstLine	= _ns.GetFirstVisibleLine();
			int lastLine	= firstLine + _ns.LinesOnScreen();
			int firstPos	= _ns.PositionFromLine(firstLine);
			int lastPos		= _ns.PositionFromLine(lastLine + 1) - 1;

			//	If the lastLine was outside the defined document range it will
			//	contain -1, defualt it to the last doc position
			if(lastPos < 0)
				lastPos = _ns.GetLength();

			List<ManagedRange> mrs = managedRangesInRange(firstPos, lastPos);

		
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			foreach(ManagedRange mr in mrs)
			{
				mr.Paint(g);
			}
		}

		#endregion

		#region Events
		private static readonly object _loadEventKey = new object();
		private static readonly object _textInsertedEventKey = new object();
		private static readonly object _textDeletedEventKey = new object();
		private static readonly object _beforeTextInsertEventKey = new object();
		private static readonly object _beforeTextDeleteEventKey = new object();
		private static readonly object _documentChangeEventKey = new object();
		private static readonly object _foldChangedEventKey = new object();
		private static readonly object _markerChangedEventKey = new object();
		private static readonly object _styleNeededEventKey = new object();
		private static readonly object _charAddedEventKey = new object();
		private static readonly object _savePointReachedEventKey = new object();
		private static readonly object _savePointLeftEventKey = new object();
		private static readonly object _readOnlyModifyAttemptEventKey = new object();
		private static readonly object _selectionChangedEventKey = new object();
		private static readonly object _linesNeedShownEventKey = new object();
		private static readonly object _uriDroppedEventKey = new object();		
		private static readonly object _dwellStartEventKey = new object();
		private static readonly object _dwellEndEventKey = new object();
		private static readonly object _zoomChangedEventKey = new object();
		private static readonly object _hotspotClickedEventKey = new object();
		private static readonly object _hotspotDoubleClickedEventKey = new object();
		private static readonly object _dropMarkerCollectEventKey  = new object();
		private static readonly object _callTipClickEventKey = new object();
		private static readonly object _marginClickEventKey = new object();
		private static readonly object _indicatorClickEventKey = new object();
		private static readonly object _scrollEventKey = new object();
		private static readonly object _macroRecordEventKey = new object();
		private static readonly object _userListEventKey = new object();
		private static readonly object _fileDropEventKey = new object();


		#region Load
		[Category("Behavior")]
		public event EventHandler Load
		{
			add { Events.AddHandler(_loadEventKey, value); }
			remove { Events.RemoveHandler(_loadEventKey, value); }
		}
		protected virtual void OnLoad(EventArgs e)
		{
			if (Events[_loadEventKey] != null)
				((EventHandler)Events[_loadEventKey])(this, e);
		}
		#endregion

		#region DocumentChange
		[Category("Scintilla")]
		public event EventHandler<NativeScintillaEventArgs> DocumentChange
		{
			add { Events.AddHandler(_documentChangeEventKey, value); }
			remove { Events.RemoveHandler(_documentChangeEventKey, value); }
		}
		protected virtual void OnDocumentChange(NativeScintillaEventArgs e)
		{
			if (Events[_documentChangeEventKey] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_documentChangeEventKey])(this, e);
		}
		#endregion

		#region CallTipClick
		[Category("Scintilla")]
		public event EventHandler<CallTipClickEventArgs> CallTipClick
		{
			add { Events.AddHandler(_callTipClickEventKey, value); }
			remove { Events.RemoveHandler(_callTipClickEventKey, value); }
		}

		internal void FireCallTipClick(int arrow)
		{
			CallTipArrow a = (CallTipArrow)arrow;
			OverloadList ol = CallTip.OverloadList;
			CallTipClickEventArgs e;

			if (ol == null)
			{
				e = new CallTipClickEventArgs(a, -1, -1, null, CallTip.HighlightStart, CallTip.HighlightEnd);
			}
			else
			{
				int newIndex = ol.CurrentIndex;

				if (a == CallTipArrow.Down)
				{
					if (ol.CurrentIndex == ol.Count - 1)
						newIndex = 0;
					else
						newIndex++;
				}
				else if (a == CallTipArrow.Up)
				{
					if (ol.CurrentIndex == 0)
						newIndex = ol.Count - 1;
					else
						newIndex--;
				}

				e = new CallTipClickEventArgs(a, ol.CurrentIndex, newIndex, ol, CallTip.HighlightStart, CallTip.HighlightEnd);
			}

			OnCallTipClick(e);

			if (e.Cancel)
			{
				CallTip.Cancel();
			}
			else
			{
				if (ol != null)
				{
					//	We allow them to alse replace the list entirely or just
					//	manipulate the New Index
					CallTip.OverloadList = e.OverloadList;
					CallTip.OverloadList.CurrentIndex = e.NewIndex;
					CallTip.ShowOverloadInternal();
				}

				CallTip.HighlightStart = e.HighlightStart;
				CallTip.HighlightEnd = e.HighlightEnd;
			}
		}

		protected virtual void OnCallTipClick(CallTipClickEventArgs e)
		{
			if (Events[_callTipClickEventKey] != null)
				((EventHandler<CallTipClickEventArgs>)Events[_callTipClickEventKey])(this, e);
		}
		#endregion				

		#region TextInserted
		[Category("Scintilla")]
		public event EventHandler<TextModifiedEventArgs> TextInserted
		{
			add { Events.AddHandler(_textInsertedEventKey, value); }
			remove { Events.RemoveHandler(_textInsertedEventKey, value); }
		}

		protected virtual void OnTextInserted(TextModifiedEventArgs e)
		{
			if (Events[_textInsertedEventKey] != null)
				((EventHandler<TextModifiedEventArgs>)Events[_textInsertedEventKey])(this, e);
		}
		#endregion

		#region TextDeleted
		[Category("Scintilla")]
		public event EventHandler<TextModifiedEventArgs> TextDeleted
		{
			add { Events.AddHandler(_textDeletedEventKey, value); }
			remove { Events.RemoveHandler(_textDeletedEventKey, value); }
		}
		protected virtual void OnTextDeleted(TextModifiedEventArgs e)
		{
			if (Events[_textDeletedEventKey] != null)
				((EventHandler<TextModifiedEventArgs>)Events[_textDeletedEventKey])(this, e);
		}
		#endregion

		#region BeforeTextInsert
		[Category("Scintilla")]
		public event EventHandler<TextModifiedEventArgs> BeforeTextInsert
		{
			add { Events.AddHandler(_beforeTextInsertEventKey, value); }
			remove { Events.RemoveHandler(_beforeTextInsertEventKey, value); }
		}
		protected virtual void OnBeforeTextInsert(TextModifiedEventArgs e)
		{
			List<ManagedRange> offsetRanges = new List<ManagedRange>();
			foreach (ManagedRange mr in _managedRanges)
			{
				if (mr.Start == e.Position && mr.PendingDeletion)
				{
					mr.PendingDeletion = false;
					ManagedRange lmr = mr;
					BeginInvoke(new MethodInvoker(delegate() { lmr.Change(e.Position, e.Position + e.Length); }));
				}

				//	If the Range is a single point we treat it slightly 
				//	differently than a spanned range
				if (mr.IsPoint)
				{
					//	Unlike a spanned range, if the insertion point of
					//	the new text == the start of the range (and thus
					//	the end as well) we offset the entire point.
					if (mr.Start >= e.Position)
						mr.Change(mr.Start + e.Length, mr.End + e.Length);
					else if (mr.End >= e.Position)
						mr.Change(mr.Start, mr.End + e.Length);
				}
				else
				{
					//	We offset a spanned range entirely only if the
					//	start occurs after the insertion point of the new
					//	text.
					if (mr.Start > e.Position)
						mr.Change(mr.Start + e.Length, mr.End + e.Length);
					else if (mr.End >= e.Position)
					{
						//	However it the start of the range == the insertion
						//	point of the new text instead of offestting the 
						//	range we expand it.
						mr.Change(mr.Start, mr.End + e.Length);
					}
				}

			}

			if (Events[_beforeTextInsertEventKey] != null)
				((EventHandler<TextModifiedEventArgs>)Events[_beforeTextInsertEventKey])(this, e);
		}

		#endregion

		#region BeforeTextDelete
		[Category("Scintilla")]
		public event EventHandler<TextModifiedEventArgs> BeforeTextDelete
		{
			add { Events.AddHandler(_beforeTextDeleteEventKey, value); }
			remove { Events.RemoveHandler(_beforeTextDeleteEventKey, value); }
		}
		protected virtual void OnBeforeTextDelete(TextModifiedEventArgs e)
		{
			int firstPos = e.Position;
			int lastPos = firstPos + e.Length;

			List<ManagedRange> deletedRanges = new List<ManagedRange>();
			foreach (ManagedRange mr in _managedRanges)
			{

				//	These ranges lie within the deleted range so
				//	the ranges themselves need to be deleted
				if (mr.Start >= firstPos && mr.End <= lastPos)
				{

					//	If the entire range is being delete and NOT a superset of the range,
					//	don't delete it, only collapse it.
					if (!mr.IsPoint && e.Position == mr.Start && (e.Position + e.Length == mr.End))
					{
						mr.Change(mr.Start, mr.Start);
					}
					else
					{
						//	Notify the virtual Range that it needs to cleanup
						mr.Change(-1, -1);

						//	Mark for deletion after this foreach:
						deletedRanges.Add(mr);

					}
				}
				else if (mr.Start >= lastPos)
				{
					//	These ranges are merely offset by the deleted range
					mr.Change(mr.Start - e.Length, mr.End - e.Length);
				}
				else if (mr.Start >= firstPos && mr.Start <= lastPos)
				{
					//	The left side of the managed range is getting
					//	cut off
					mr.Change(firstPos, mr.End - e.Length);
				}
				else if (mr.Start < firstPos && mr.End >= firstPos && mr.End >= lastPos)
				{
					mr.Change(mr.Start, mr.End - e.Length);
				}
				else if (mr.Start < firstPos && mr.End >= firstPos && mr.End < lastPos)
				{
					mr.Change(mr.Start, firstPos);
				}

			}

			foreach (ManagedRange mr in deletedRanges)
				mr.Dispose();


			if (Events[_beforeTextDeleteEventKey] != null)
				((EventHandler<TextModifiedEventArgs>)Events[_beforeTextDeleteEventKey])(this, e);
		} 
		#endregion

		#region FoldChanged
		[Category("Scintilla")]
		public event EventHandler<FoldChangedEventArgs> FoldChanged
		{
			add { Events.AddHandler(_foldChangedEventKey, value); }
			remove { Events.RemoveHandler(_foldChangedEventKey, value); }
		}
		protected virtual void OnFoldChanged(FoldChangedEventArgs e)
		{
			if (Events[_foldChangedEventKey] != null)
				((EventHandler<FoldChangedEventArgs>)Events[_foldChangedEventKey])(this, e);
		} 
		#endregion

		#region MarkerChanged
		[Category("Scintilla")]
		public event EventHandler<MarkerChangedEventArgs> MarkerChanged
		{
			add { Events.AddHandler(_markerChangedEventKey, value); }
			remove { Events.RemoveHandler(_markerChangedEventKey, value); }
		}
		protected virtual void OnMarkerChanged(MarkerChangedEventArgs e)
		{
			if (Events[_markerChangedEventKey] != null)
				((EventHandler<MarkerChangedEventArgs>)Events[_markerChangedEventKey])(this, e);
		}

		#endregion

		#region IndicatorClick
		[Category("Scintilla")]
		public event EventHandler<ScintillaMouseEventArgs> IndicatorClick
		{
			add { Events.AddHandler(_indicatorClickEventKey, value); }
			remove { Events.RemoveHandler(_indicatorClickEventKey, value); }
		}
		protected virtual void OnIndicatorClick(ScintillaMouseEventArgs e)
		{
			if (Events[_indicatorClickEventKey] != null)
				((EventHandler<ScintillaMouseEventArgs>)Events[_indicatorClickEventKey])(this, e);
		}

		#endregion

		#region MarginClick
		[Category("Scintilla")]
		public event EventHandler<MarginClickEventArgs> MarginClick
		{
			add { Events.AddHandler(_marginClickEventKey, value); }
			remove { Events.RemoveHandler(_marginClickEventKey, value); }
		}
		
        protected virtual void OnMarginClick(MarginClickEventArgs e)
		{
			if (Events[_marginClickEventKey] != null)
				((EventHandler<MarginClickEventArgs>)Events[_marginClickEventKey])(this, e);
		}

		internal void FireMarginClick(SCNotification n)
		{
			Margin m = Margins[n.margin];
			Keys k = Keys.None;

			if ((n.modifiers & (int)KeyMod.Alt) == (int)KeyMod.Alt)
				k |= Keys.Alt;

			if ((n.modifiers & (int)KeyMod.Ctrl) == (int)KeyMod.Ctrl)
				k |= Keys.Control;

			if ((n.modifiers & (int)KeyMod.Shift) == (int)KeyMod.Shift)
				k |= Keys.Shift;

			OnMarginClick(new MarginClickEventArgs(k, n.position, Lines.FromPosition(n.position), m, m.AutoToggleMarkerNumber, m.IsFoldMargin));
		}
		#endregion

		#region StyleNeeded
		[Category("Scintilla")]
		public event EventHandler<StyleNeededEventArgs> StyleNeeded
		{
			add { Events.AddHandler(_styleNeededEventKey, value); }
			remove { Events.RemoveHandler(_styleNeededEventKey, value); }
		}
		protected virtual void OnStyleNeeded(StyleNeededEventArgs e)
		{
			if (Events[_styleNeededEventKey] != null)
				((EventHandler<StyleNeededEventArgs>)Events[_styleNeededEventKey])(this, e);
		} 
		#endregion

		#region CharAdded
		[Category("Scintilla")]
		public event EventHandler<CharAddedEventArgs> CharAdded
		{
			add { Events.AddHandler(_charAddedEventKey, value); }
			remove { Events.RemoveHandler(_charAddedEventKey, value); }
		}
		protected virtual void OnCharAdded(CharAddedEventArgs e)
		{
			if (Events[_charAddedEventKey] != null)
				((EventHandler<CharAddedEventArgs>)Events[_charAddedEventKey])(this, e);

			if (_indentation.SmartIndentType != SmartIndent.None)
				_indentation.CheckSmartIndent(e.Ch);
		} 
		#endregion

		#region SavePointReached
		[Category("Scintilla")]
		public event EventHandler SavePointReached
		{
			add { Events.AddHandler(_savePointReachedEventKey, value); }
			remove { Events.RemoveHandler(_savePointReachedEventKey, value); }
		}
		protected virtual void OnSavePointReached(EventArgs e)
		{
			_isDirty = false;
			if (Events[_savePointReachedEventKey] != null)
				((EventHandler)Events[_savePointReachedEventKey])(this, e);
		}

		#endregion

		#region SavePointLeft
		[Category("Scintilla")]
		public event EventHandler SavePointLeft
		{
			add { Events.AddHandler(_savePointLeftEventKey, value); }
			remove { Events.RemoveHandler(_savePointLeftEventKey, value); }
		}
		protected virtual void OnSavePointLeft(EventArgs e)
		{
			_isDirty = true;
			if (Events[_savePointLeftEventKey] != null)
				((EventHandler)Events[_savePointLeftEventKey])(this, e);
		} 
		#endregion

		#region ReadOnlyModifyAttempt
		[Category("Scintilla")]
		public event EventHandler ReadOnlyModifyAttempt
		{
			add { Events.AddHandler(_readOnlyModifyAttemptEventKey, value); }
			remove { Events.RemoveHandler(_readOnlyModifyAttemptEventKey, value); }
		}
		protected virtual void OnReadOnlyModifyAttempt(EventArgs e)
		{
			if (Events[_readOnlyModifyAttemptEventKey] != null)
				((EventHandler)Events[_readOnlyModifyAttemptEventKey])(this, e);
		} 
		#endregion

		#region SelectionChanged
		[Category("Scintilla")]
		public event EventHandler SelectionChanged
		{
			add { Events.AddHandler(_selectionChangedEventKey, value); }
			remove { Events.RemoveHandler(_selectionChangedEventKey, value); }
		}
		protected virtual void OnSelectionChanged(EventArgs e)
		{
			if (Events[_selectionChangedEventKey] != null)
				((EventHandler)Events[_selectionChangedEventKey])(this, e);

			if (_isBraceMatching && (_selection.Length == 0))
			{
				int position = CurrentPos - 1,
					bracePosStart = -1,
					bracePosEnd = -1;

				char character = (char)CharAt(position);

				switch (character)
				{
					case '{':
					case '}':
					case '(':
					case ')':
					case '[':
					case ']':
						if (!this.PositionIsOnComment(position))
						{
							bracePosStart = position;
							bracePosEnd = _ns.BraceMatch(position,0);
						}
						break;
					default:
						position = CurrentPos;
						character = (char)CharAt(position);
						break;
				}

				_ns.BraceHighlight(bracePosStart, bracePosEnd);
			}
		} 
		#endregion

		#region LinesNeedShown
		[Category("Scintilla")]
		public event EventHandler<LinesNeedShownEventArgs> LinesNeedShown
		{
			add { Events.AddHandler(_linesNeedShownEventKey, value); }
			remove { Events.RemoveHandler(_linesNeedShownEventKey, value); }
		}
		protected virtual void OnLinesNeedShown(LinesNeedShownEventArgs e)
		{
			if (Events[_linesNeedShownEventKey] != null)
				((EventHandler<LinesNeedShownEventArgs>)Events[_linesNeedShownEventKey])(this, e);
		}

		#endregion

		#region UriDropped
		[Category("Scintilla")]
		public event EventHandler<UriDroppedEventArgs> UriDropped
		{
			add { Events.AddHandler(_uriDroppedEventKey, value); }
			remove { Events.RemoveHandler(_uriDroppedEventKey, value); }
		}
		protected virtual void OnUriDropped(UriDroppedEventArgs e)
		{
			if (Events[_uriDroppedEventKey] != null)
				((EventHandler<UriDroppedEventArgs>)Events[_uriDroppedEventKey])(this, e);
		} 
		#endregion

		#region DwellStart
		[Category("Scintilla")]
		public event EventHandler<ScintillaMouseEventArgs> DwellStart
		{
			add { Events.AddHandler(_dwellStartEventKey, value); }
			remove { Events.RemoveHandler(_dwellStartEventKey, value); }
		}
		protected virtual void OnDwellStart(ScintillaMouseEventArgs e)
		{
			if (Events[_dwellStartEventKey] != null)
				((EventHandler<ScintillaMouseEventArgs>)Events[_dwellStartEventKey])(this, e);
		}

		#endregion

		#region DwellEnd
		[Category("Scintilla")]
		public event EventHandler DwellEnd
		{
			add { Events.AddHandler(_dwellEndEventKey, value); }
			remove { Events.RemoveHandler(_dwellEndEventKey, value); }
		}
		protected virtual void OnDwellEnd(EventArgs e)
		{
			if (Events[_dwellEndEventKey] != null)
				((EventHandler)Events[_dwellEndEventKey])(this, e);
		}

		#endregion

		#region ZoomChanged
		[Category("Scintilla")]
		public event EventHandler ZoomChanged
		{
			add { Events.AddHandler(_zoomChangedEventKey, value); }
			remove { Events.RemoveHandler(_zoomChangedEventKey, value); }
		}
		protected virtual void OnZoomChanged(EventArgs e)
		{
			if (Events[_zoomChangedEventKey] != null)
				((EventHandler)Events[_zoomChangedEventKey])(this, e);
		} 
		#endregion

		#region HotspotClick
		[Category("Scintilla")]
		public event EventHandler<ScintillaMouseEventArgs> HotspotClick
		{
			add { Events.AddHandler(_hotSpotClickEventKey, value); }
			remove { Events.RemoveHandler(_hotSpotClickEventKey, value); }
		}
		protected virtual void OnHotspotClick(ScintillaMouseEventArgs e)
		{
			if (Events[_hotSpotClickEventKey] != null)
				((EventHandler<ScintillaMouseEventArgs>)Events[_hotSpotClickEventKey])(this, e);
		} 
		#endregion

		#region HotspotDoubleClick
		[Category("Scintilla")]
		public event EventHandler<ScintillaMouseEventArgs> HotspotDoubleClick
		{
			add { Events.AddHandler(_hotSpotDoubleClickEventKey, value); }
			remove { Events.RemoveHandler(_hotSpotDoubleClickEventKey, value); }
		}
		protected virtual void OnHotspotDoubleClick(ScintillaMouseEventArgs e)
		{
			if (Events[_hotSpotDoubleClickEventKey] != null)
				((EventHandler<ScintillaMouseEventArgs>)Events[_hotSpotDoubleClickEventKey])(this, e);
		}

		#endregion

		#region DropMarkerCollect
		[Category("Scintilla")]
		public event EventHandler<DropMarkerCollectEventArgs> DropMarkerCollect
		{
			add { Events.AddHandler(_dropMarkerCollectEventKey, value); }
			remove { Events.RemoveHandler(_dropMarkerCollectEventKey, value); }
		}
		protected internal virtual void OnDropMarkerCollect(DropMarkerCollectEventArgs e)
		{
			if (Events[_dropMarkerCollectEventKey] != null)
				((EventHandler<DropMarkerCollectEventArgs>)Events[_dropMarkerCollectEventKey])(this, e);
		}

		#endregion

		#region Scroll
		[Category("Action")]
		public event EventHandler<ScrollEventArgs> Scroll
		{
			add { Events.AddHandler(_scrollEventKey, value); }
			remove { Events.RemoveHandler(_scrollEventKey, value); }
		}

		internal void FireScroll(ref Message m)
		{
			ScrollOrientation so = ScrollOrientation.VerticalScroll;
			int oldScroll = 0, newScroll = 0;
			ScrollEventType set = (ScrollEventType)(Utilities.SignedLoWord(m.WParam));
			if (m.Msg == NativeMethods.WM_HSCROLL)
			{
				so = ScrollOrientation.HorizontalScroll;
				oldScroll = _ns.GetXOffset();

				//	Let Scintilla Handle the scroll Message to actually perform scrolling
				base.WndProc(ref m);
				newScroll = _ns.GetXOffset();
			}
			else
			{
				so = ScrollOrientation.VerticalScroll;
				oldScroll = _ns.GetFirstVisibleLine();
				base.WndProc(ref m);
				newScroll = _ns.GetFirstVisibleLine();
			}

			OnScroll(new ScrollEventArgs(set, oldScroll, newScroll, so));
		}

		protected virtual void OnScroll(ScrollEventArgs e)
		{
			if (Events[_scrollEventKey] != null)
				((EventHandler<ScrollEventArgs>)Events[_scrollEventKey])(this, e);
		}

		#endregion	
		
		#region MacroRecord
		[Category("Scintilla")]
		public event EventHandler<MacroRecordEventArgs> MacroRecord
		{
			add { Events.AddHandler(_macroRecordEventKey, value); }
			remove { Events.RemoveHandler(_macroRecordEventKey, value); }
		}
		protected virtual void OnMacroRecord(MacroRecordEventArgs e)
		{
			if (Events[_macroRecordEventKey] != null)
				((EventHandler<MacroRecordEventArgs>)Events[_macroRecordEventKey])(this, e);
		}
		#endregion

		#region FileDrop
		[Category("Scintilla")]
		public event EventHandler<FileDropEventArgs> FileDrop
		{
			add { Events.AddHandler(_fileDropEventKey, value); }
			remove { Events.RemoveHandler(_fileDropEventKey, value); }
		}
		protected virtual void OnFileDrop(FileDropEventArgs e)
		{
			if (Events[_fileDropEventKey] != null)
				((EventHandler<FileDropEventArgs>)Events[_fileDropEventKey])(this, e);
		}
		#endregion

		#endregion

		#region Public Methods
		public Range AppendText(string text)
		{
			int oldLength = TextLength;
			NativeInterface.AppendText(text.Length, text);
			return GetRange(oldLength, TextLength);
		}

		/// <summary>
		/// Inserts text at the current cursor position
		/// </summary>
		/// <param name="text">Text to insert</param>
		/// <returns>The range inserted</returns>
		public Range InsertText(string text)
		{
			NativeInterface.AddText(text.Length, text);
			return GetRange(_caret.Position, text.Length);
		}

		/// <summary>
		/// Inserts text at the given position
		/// </summary>
		/// <param name="position">The position to insert text in</param>
		/// <param name="text">Text to insert</param>
		/// <returns>The text range inserted</returns>
		public Range InsertText(int position, string text)
		{
			NativeInterface.InsertText(position, text);
			return GetRange(position, text.Length);
		}

		public char CharAt(int position)
		{
			return _ns.GetCharAt(position);
		}

		public Range GetRange(int startPosition, int endPosition)
		{
			return new Range(startPosition, endPosition, this);
		}

		public Range GetRange()
		{
			return new Range(0, _ns.GetTextLength(), this);
		}

		public int GetColumn(int position)
		{
			return _ns.GetColumn(position);
		}

		public int FindColumn(int line, int column)
		{
			return _ns.FindColumn(line, column);
		}

		public int PositionFromPoint(int x, int y)
		{
			return _ns.PositionFromPoint(x, y);
		}

		public int PositionFromPointClose(int x, int y)
		{
			return _ns.PositionFromPointClose(x, y);
		}

		public int PointXFromPosition(int position)
		{
			return _ns.PointXFromPosition(position);
		}

		public int PointYFromPosition(int position)
		{
			return _ns.PointYFromPosition(position);
		}

		/// <summary>
		/// Checks that if the specified position is on comment.
		/// </summary>
		public bool PositionIsOnComment(int position)
		{
			//this.Colorize(0, -1);
			return PositionIsOnComment(position, _lexing.Lexer);
		}

		/// <summary>
		/// Checks that if the specified position is on comment.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="lexer"></param>
		/// <returns></returns>
		public bool PositionIsOnComment(int position, Lexer lexer)
		{
			int style = _styles.GetStyleAt(position);
			if ((lexer == Lexer.Python || lexer == Lexer.Lisp)
				&& style == 1
				|| style == 12)
			{
				return true; // python or lisp
			}
			else if ((lexer == Lexer.Cpp || lexer == Lexer.Pascal || lexer == Lexer.Tcl || lexer == Lexer.Bullant)
				&& style == 1
				|| style == 2
				|| style == 3
				|| style == 15
				|| style == 17
				|| style == 18)
			{
				return true; // cpp, tcl, bullant or pascal
			}
			else if ((lexer == Lexer.Hypertext || lexer == Lexer.Xml)
				&& style == 9
				|| style == 20
				|| style == 29
				|| style == 30
				|| style == 42
				|| style == 43
				|| style == 44
				|| style == 57
				|| style == 58
				|| style == 59
				|| style == 72
				|| style == 82
				|| style == 92
				|| style == 107
				|| style == 124
				|| style == 125)
			{
				return true; // html or xml
			}
			else if ((lexer == Lexer.Perl || lexer == Lexer.Ruby || lexer == Lexer.Clw || lexer == Lexer.Bash)
				&& style == 2)
			{
				return true; // perl, bash, clarion/clw or ruby
			}
			else if ((lexer == Lexer.Sql)
				&& style == 1
				|| style == 2
				|| style == 3
				|| style == 13
				|| style == 15
				|| style == 17
				|| style == 18)
			{
				return true; // sql
			}
			else if ((lexer == Lexer.VB || lexer == Lexer.Properties || lexer == Lexer.MakeFile || lexer == Lexer.Batch || lexer == Lexer.Diff || lexer == Lexer.Conf || lexer == Lexer.Ave || lexer == Lexer.Eiffel || lexer == Lexer.EiffelKw || lexer == Lexer.Tcl || lexer == Lexer.VBScript || lexer == Lexer.MatLab || lexer == Lexer.Fortran || lexer == Lexer.F77 || lexer == Lexer.Lout || lexer == Lexer.Mmixal || lexer == Lexer.Yaml || lexer == Lexer.PowerBasic || lexer == Lexer.ErLang || lexer == Lexer.Octave || lexer == Lexer.Kix || lexer == Lexer.Asn1)
				&& style == 1)
			{
				return true; // asn1, vb, diff, batch, makefile, avenue, eiffel, eiffelkw, vbscript, matlab, crontab, fortran, f77, lout, mmixal, yaml, powerbasic, erlang, octave, kix or properties
			}
			else if ((lexer == Lexer.Latex)
				&& style == 4)
			{
				return true; // latex
			}
			else if ((lexer == Lexer.Lua || lexer == Lexer.EScript || lexer == Lexer.Verilog)
				&& style == 1
				|| style == 2
				|| style == 3)
			{
				return true; // lua, verilog or escript
			}
			else if ((lexer == Lexer.Ada)
				&& style == 10)
			{
				return true; // ada
			}
			else if ((lexer == Lexer.Baan || lexer == Lexer.Pov || lexer == Lexer.Ps || lexer == Lexer.Forth || lexer == Lexer.MsSql || lexer == Lexer.Gui4Cli || lexer == Lexer.Au3 || lexer == Lexer.Apdl || lexer == Lexer.Vhdl || lexer == Lexer.Rebol)
				&& style == 1
				|| style == 2)
			{
				return true; // au3, apdl, baan, ps, mssql, rebol, forth, gui4cli, vhdl or pov
			}
			else if ((lexer == Lexer.Asm)
				&& style == 1
				|| style == 11)
			{
				return true; // asm
			}
			else if ((lexer == Lexer.Nsis)
				&& style == 1
				|| style == 18)
			{
				return true; // nsis
			}
			else if ((lexer == Lexer.Specman)
				&& style == 2
				|| style == 3)
			{
				return true; // specman
			}
			else if ((lexer == Lexer.Tads3)
				&& style == 3
				|| style == 4)
			{
				return true; // tads3
			}
			else if ((lexer == Lexer.CSound)
				&& style == 1
				|| style == 9)
			{
				return true; // csound
			}
			else if ((lexer == Lexer.Caml)
				&& style == 12
				|| style == 13
				|| style == 14
				|| style == 15)
			{
				return true; // caml
			}
			else if ((lexer == Lexer.Haskell)
				&& style == 13
				|| style == 14
				|| style == 15
				|| style == 16)
			{
				return true; // haskell
			}
			else if ((lexer == Lexer.Flagship)
				&& style == 1
				|| style == 2
				|| style == 3
				|| style == 4
				|| style == 5
				|| style == 6)
			{
				return true; // flagship
			}
			else if ((lexer == Lexer.Smalltalk)
				&& style == 3)
			{
				return true; // smalltalk
			}
			else if ((lexer == Lexer.Css)
				&& style == 9)
			{
				return true; // css
			}
			return false;
		}

		/// <summary>
		/// Adds a line end marker to the end of the document
		/// </summary>
		public void AddLastLineEnd()
		{
			EndOfLineMode eolMode = _endOfLine.Mode;
			string eolMarker = "\r\n";

			if (eolMode == EndOfLineMode.CR)
				eolMarker = "\r";
			else if (eolMode == EndOfLineMode.LF)
				eolMarker = "\n";

			int tl = TextLength;
			int start = tl - eolMarker.Length;

			if (start < 0 || GetRange(start, start + eolMarker.Length).Text != eolMarker)
				AppendText(eolMarker);
		}

		/// <summary>
		/// Gets a word from the specified position
		/// </summary>
		public string GetWordFromPosition(int position)
		{
			try
			{
				int startPosition = NativeInterface.WordStartPosition(position - 1, true);
				int endPosition = NativeInterface.WordEndPosition(position - 1, true);
				string keyword = GetRange(startPosition, endPosition - startPosition).Text;
				if (keyword == "" || keyword == " ")
					return null;
				return keyword.Trim();
			}
			catch
			{
				return null;
			}
		}

		#endregion

		#region other stuff
		internal bool IsDesignMode
		{
			get
			{
				return DesignMode;
			}
		}

		private List<ScintillaHelperBase> _helpers = new List<ScintillaHelperBase>();
		protected internal List<ScintillaHelperBase> Helpers
		{
			get
			{
				return _helpers;
			}
			set
			{
				_helpers = value;
			}
		}
		#endregion

		#region ISupportInitialize Members
		private bool _isInitializing = false;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool IsInitializing
		{
			get
			{
				return _isInitializing;
			}
			set
			{
				_isInitializing = value;
			}
		}

		public void BeginInit()
		{
			_isInitializing = true;
		}

		public void EndInit()
		{
			_isInitializing = false;
			foreach(ScintillaHelperBase helper in _helpers)
			{
				helper.Initialize();
			}			
		}

		#endregion
	}
}