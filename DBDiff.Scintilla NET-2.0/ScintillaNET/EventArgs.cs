using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
namespace ScintillaNet
{
	#region DropMarkerCollectEventArgs
	public class DropMarkerCollectEventArgs : CancelEventArgs
	{
		private DropMarker _dropMarker;
		public DropMarker DropMarker
		{
			get
			{
				return _dropMarker;
			}
			set
			{
				_dropMarker = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the DropMarkerCollectEventArgs class.
		/// </summary>
		/// <param name="dropMarker"></param>
		public DropMarkerCollectEventArgs(DropMarker dropMarker)
		{
			_dropMarker = dropMarker;
		}
	} 
	#endregion

	#region CharAddedEventArgs

	public class CallTipClickEventArgs : EventArgs
	{
		private CallTipArrow _callTipArrow;
		public CallTipArrow CallTipArrow
		{
			get
			{
				return _callTipArrow;
			}
		}

		private int _currentIndex;
		public int CurrentIndex
		{
			get
			{
				return _currentIndex;
			}
		}
		private int _newIndex;
		public int NewIndex
		{
			get
			{
				return _newIndex;
			}
			set
			{
				_newIndex = value;
			}
		}

		private OverloadList _overloadList;
		public OverloadList OverloadList
		{
			get
			{
				return _overloadList;
			}
			set
			{
				_overloadList = value;
			}
		}

		private bool _cancel = false;
		public bool Cancel
		{
			get
			{
				return _cancel;
			}
			set
			{
				_cancel = value;
			}
		}

		private int _highlightStart;
		public int HighlightStart
		{
			get
			{
				return _highlightStart;
			}
			set
			{
				_highlightStart = value;
			}
		}

		private int _highlightEnd;
		public int HighlightEnd
		{
			get
			{
				return _highlightEnd;
			}
			set
			{
				_highlightEnd = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the CallTipClickEventArgs class.
		/// </summary>
		/// <param name="callTipArrow"></param>
		/// <param name="currentIndex"></param>
		/// <param name="newIndex"></param>
		/// <param name="overloadList"></param>
		/// <param name="highlightStart"></param>
		/// <param name="highlightEnd"></param>
		public CallTipClickEventArgs(CallTipArrow callTipArrow, int currentIndex, int newIndex, OverloadList overloadList, int highlightStart, int highlightEnd)
		{
			_callTipArrow = callTipArrow;
			_currentIndex = currentIndex;
			_newIndex = newIndex;
			_overloadList = overloadList;
			_highlightStart = highlightStart;
			_highlightEnd = highlightEnd;
		}
	}
	#endregion

	#region CharAddedEventArgs

	public class CharAddedEventArgs : EventArgs
	{
		private char _ch;
		public char Ch
		{
			get
			{
				return _ch;
			}
		}

		public CharAddedEventArgs(char ch)
		{
			_ch = ch;
		}
	}
	#endregion

	#region FoldChangedEventArgs
	public class FoldChangedEventArgs : ModifiedEventArgs
	{
		private int _line;
		private int _newFoldLevel;
		private int _previousFoldLevel;

		public int Line
		{
			get
			{
				return _line;
			}
			set
			{
				_line = value;
			}
		}

		public int NewFoldLevel
		{
			get
			{
				return _newFoldLevel;
			}
			set
			{
				_newFoldLevel = value;
			}
		}

		public int PreviousFoldLevel
		{
			get
			{
				return _previousFoldLevel;
			}
			set
			{
				_previousFoldLevel = value;
			}
		}

		public FoldChangedEventArgs(int line, int newFoldLevel, int previousFoldLevel, int modificationType) : base(modificationType)
		{
			_line				= line;
			_newFoldLevel		= newFoldLevel;
			_previousFoldLevel	= previousFoldLevel;
		}
	}
	#endregion 

	#region LinesNeedShownEventArgs
	public class LinesNeedShownEventArgs : EventArgs
	{
		private int _firstLine;
		private int _lastLine;

		public int FirstLine
		{
			get { return _firstLine; }
			set { _firstLine = value; }
		}
		
		public int LastLine
		{
			get { return _lastLine; }
			set { _lastLine = value; }
		}

		public LinesNeedShownEventArgs(int startLine, int endLine)
		{
			_firstLine	= startLine;
			_lastLine	= endLine;
		}
	}

	#endregion

	#region MarkerChangedEventArgs
	public class MarkerChangedEventArgs : ModifiedEventArgs
	{
		private int _line;
		public int Line
		{
			get
			{
				return _line;
			}
			set
			{
				_line = value;
			}
		}

		public MarkerChangedEventArgs(int line, int modificationType)
			: base(modificationType)
		{
			_line = line;
		}

	}
	#endregion

	#region ModifiedEventArgs

	//	ModifiedEventArgs is the base class for all events that are fired 
	//	in response to an SCN_MODIFY notification message. They all have 
	//	the Undo/Redo flags in common and I'm also including the raw 
	//	modificationType integer value for convenience purposes.
	public abstract class ModifiedEventArgs : EventArgs
	{
		private UndoRedoFlags _undoRedoFlags;
		private int _modificationType;

		public int ModificationType
		{
			get
			{
				return _modificationType;
			}
			set
			{
				_modificationType = value;
			}
		}

		public UndoRedoFlags UndoRedoFlags
		{
			get
			{
				return _undoRedoFlags;
			}
			set
			{
				_undoRedoFlags = value;
			}
		}

		public ModifiedEventArgs(int modificationType)
		{
			_modificationType	= modificationType;
			_undoRedoFlags		= new UndoRedoFlags(modificationType);
		}
	}
	#endregion

	#region NativeScintillaEventArgs
	
	//	All events fired from the INativeScintilla Interface uses
	//	NativeScintillaEventArgs. Msg is a copy
	//	of the Notification Message sent to Scintilla's Parent WndProc
	//	and SCNotification is the SCNotification Struct pointed to by 
	//	Msg's lParam. 
	
	public class NativeScintillaEventArgs : EventArgs
	{
		private Message _msg;
		private SCNotification _notification;

		public Message Msg
		{
			get
			{
				return _msg;
			}
		}

		public SCNotification SCNotification
		{
			get
			{
				return _notification;
			}
		}

		public NativeScintillaEventArgs(Message Msg, SCNotification notification)
		{
			_msg			= Msg;
			_notification	= notification;
		}
	}
	#endregion
		
	#region ScintillaMouseEventArgs

	public class ScintillaMouseEventArgs : EventArgs
	{
		private int _x;
		private int _y;
		private int _position;

		public int X
		{
			get { return _x; }
			set { _x = value; }
		}

		public int Y
		{
			get { return _y; }
			set { _y = value; }
		}

		public int Position
		{
			get { return _position; }
			set { _position = value; }
		}

		public ScintillaMouseEventArgs(int x, int y, int position)
		{
			_x			= x;
			_y			= y;
			_position	= position;
		}
	}

	#endregion
	
	#region StyleChangedEventArgs
	//	StyleChangedEventHandler is used for the StyleChanged Event which is also used as 
	//	a more specific abstraction around the SCN_MODIFIED notification message.
	public class StyleChangedEventArgs : ModifiedEventArgs
	{
		private int _position;
		private int _length;

		public int Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
			}
		}

		public int Length
		{
			get
			{
				return _length;
			}
			set
			{
				_length = value;
			}
		}

		public StyleChangedEventArgs(int position, int length, int modificationType) : base(modificationType)
		{
			_position	= position;
			_length		= length;
		}
	}
	#endregion

	#region StyleNeededEventArgs

	public class StyleNeededEventArgs : EventArgs
	{
		private Range _range;
		public Range Range
		{
			get { return _range; }
			set { _range = value; }
		}

		public StyleNeededEventArgs(Range range)
		{
			_range = range;
		}
	}
	


	#endregion

	#region TextModifiedEventArgs

	//	TextModifiedEventHandler is used as an abstracted subset of the
	//	SCN_MODIFIED notification message. It's used whenever the SCNotification's
	//	modificationType flags are SC_MOD_INSERTTEXT ,SC_MOD_DELETETEXT, 
	//	SC_MOD_BEFOREINSERT and SC_MOD_BEFORE_DELETE. They all use a 
	//	TextModifiedEventArgs which corresponds to a subset of the 
	//	SCNotification struct having to do with these modification types.


	public class TextModifiedEventArgs : ModifiedEventArgs
	{
		private int _position;
		private int _length;
		private int _linesAddedCount;
		private string _text;
		private bool _isUserChange;
		private int _markerChangedLine;

		private const string STRING_FORMAT = "ModificationTypeFlags\t:{0}\r\nPosition\t\t\t:{1}\r\nLength\t\t\t\t:{2}\r\nLinesAddedCount\t\t:{3}\r\nText\t\t\t\t:{4}\r\nIsUserChange\t\t\t:{5}\r\nMarkerChangeLine\t\t:{6}";

		public override string ToString()
		{
			return string.Format(STRING_FORMAT, ModificationType, _position, _length, _linesAddedCount, _text, _isUserChange, _markerChangedLine) + Environment.NewLine + UndoRedoFlags.ToString();
		}

		public bool IsUserChange
		{
			get
			{
				return _isUserChange;
			}
			set
			{
				_isUserChange = value;
			}
		}

		public int MarkerChangedLine
		{
			get
			{
				return _markerChangedLine;
			}
			set
			{
				_markerChangedLine = value;
			}
		}

		public int Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
			}
		}

		public int Length
		{
			get
			{
				return _length;
			}
			set
			{
				_length = value;
			}
		}

		public int LinesAddedCount
		{
			get
			{
				return _linesAddedCount;
			}
			set
			{
				_linesAddedCount = value;
			}
		}


		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		public TextModifiedEventArgs(int modificationType, bool isUserChange, int markerChangedLine, int position, int length, int linesAddedCount, string text) : base(modificationType)
		{
			_isUserChange			= isUserChange;
			_markerChangedLine		= markerChangedLine;
			_position				= position;
			_length					= length;
			_linesAddedCount		= linesAddedCount;
			_text					= text;
		}
	}
	#endregion

	#region UndoRedoFlags
	//	Used by TextModifiedEventArgs, StyeChangedEventArgs and FoldChangedEventArgs
	//	this provides a friendly wrapper around the SCNotification's modificationType
	//	flags having to do with Undo and Redo
	public struct UndoRedoFlags
	{
		public bool IsUndo;
		public bool IsRedo;
		public bool IsMultiStep;
		public bool IsLastStep;
		public bool IsMultiLine;

		private const string STRING_FORMAT = "IsUndo\t\t\t\t:{0}\r\nIsRedo\t\t\t\t:{1}\r\nIsMultiStep\t\t\t:{2}\r\nIsLastStep\t\t\t:{3}\r\nIsMultiLine\t\t\t:{4}";

		public override string ToString()
		{
			return string.Format(STRING_FORMAT, IsUndo, IsRedo, IsMultiStep, IsLastStep, IsMultiLine);
		}

		public UndoRedoFlags(int modificationType)
		{
			IsLastStep		= (modificationType & Constants.SC_LASTSTEPINUNDOREDO) > 0;
			IsMultiLine		= (modificationType & Constants.SC_MULTILINEUNDOREDO) > 0;
			IsMultiStep		= (modificationType & Constants.SC_MULTISTEPUNDOREDO) > 0;
			IsRedo			= (modificationType & Constants.SC_PERFORMED_REDO) > 0;
			IsUndo			= (modificationType & Constants.SC_PERFORMED_UNDO) > 0;
		}
	}
	#endregion

	#region UriDroppedEventArgs

	public class UriDroppedEventArgs : EventArgs
	{
		//	I decided to leave it a string because I can't really
		//	be sure it is a Uri.
		private string _uriText;
		public string UriText
		{
			get { return _uriText; }
			set { _uriText = value; }
		}

		public UriDroppedEventArgs(string uriText)
		{
			_uriText = uriText;
		}
	}
	#endregion	

	#region MarginClickEventArgs
	public class MarginClickEventArgs : EventArgs
	{
		private Keys _modifiers;
		public Keys Modifiers
		{
			get
			{
				return _modifiers;
			}
		}

		private int _position;
		public int Position
		{
			get
			{
				return _position;
			}
		}

		private Line _line;
		public Line Line
		{
			get
			{
				return _line;
			}
		}

		private Margin _margin;
		public Margin Margin
		{
			get
			{
				return _margin;
			}
		}

		private int _toggleMarkerNumber;
		public int ToggleMarkerNumber
		{
			get
			{
				return _toggleMarkerNumber;
			}
			set
			{
				_toggleMarkerNumber = value;
			}
		}

		private bool _toggleFold;
		public bool ToggleFold
		{
			get
			{
				return _toggleFold;
			}
			set
			{
				_toggleFold = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the MarginClickEventArgs class.
		/// </summary>
		/// <param name="modifiers"></param>
		/// <param name="position"></param>
		/// <param name="line"></param>
		/// <param name="margin"></param>
		/// <param name="toggleMarkerNumber"></param>
		/// <param name="toggleFold"></param>
		public MarginClickEventArgs(Keys modifiers, int position, Line line, Margin margin, int toggleMarkerNumber, bool toggleFold)
		{
			_modifiers = modifiers;
			_position = position;
			_line = line;
			_margin = margin;
			_toggleMarkerNumber = toggleMarkerNumber;
			_toggleFold = toggleFold;
		}
	} 
	#endregion

	#region MacroRecordEventArgs
	public class MacroRecordEventArgs : EventArgs
	{
		private Message _recordedMessage;
		public Message RecordedMessage
		{
			get
			{
				return _recordedMessage;
			}
			set
			{
				_recordedMessage = value;
			}
		}
		/// <summary>
		/// Initializes a new instance of the MacroRecordEventArgs class.
		/// </summary>
		/// <param name="recordedMessage"></param>
		public MacroRecordEventArgs(Message recordedMessage)
		{
			_recordedMessage = recordedMessage;
		}

		public MacroRecordEventArgs(NativeScintillaEventArgs ea)
		{
			_recordedMessage = ea.Msg;
			_recordedMessage.LParam = ea.SCNotification.lParam;
			_recordedMessage.WParam = ea.SCNotification.wParam;
		}
	} 
	#endregion

	#region UserListItemSelected
	public class UserListItemSelected : EventArgs
	{
		private string _text;

		public string Text
		{
			get { return _text; }
		}

		public UserListItemSelected(string text)
		{
			_text = text;
		}

		private bool _cancel = false;
		public bool Cancel
		{
			get
			{
				return _cancel;
			}
			set
			{
				_cancel = value;
			}
		}

		private int _listType;
		public int ListType
		{
			get
			{
				return _listType;
			}
			set
			{
				_listType = value;
			}
		}

		internal UserListItemSelected(SCNotification eventSource)
		{
			_listType = (int)eventSource.wParam;
			_text = Utilities.MarshalStr(eventSource.text);
		}
	}
	#endregion

	public class FileDropEventArgs : EventArgs
	{
		private string[] _fileNames;
		public string[] FileNames
		{
			get
			{
				return _fileNames;
			}
		}

		/// <summary>
		/// Initializes a new instance of the FileDropEventArgs class.
		/// </summary>
		/// <param name="fileNames"></param>
		public FileDropEventArgs(string[] fileNames)
		{
			_fileNames = fileNames;
		}
	}
}