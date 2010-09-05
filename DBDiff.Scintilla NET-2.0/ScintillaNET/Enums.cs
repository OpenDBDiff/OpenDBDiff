using System;
using System.Collections.Generic;
using System.Text;

namespace ScintillaNet
{
	public enum VOID
	{
		NULL
	}

	public enum CaretStyle
	{
		Invisible = 0,
		Line = 1,
		Block = 2
	}

	public enum Lexer
	{
		Container=0,
		Null=1,
		Python=2,
		Cpp=3,
		Hypertext=4,
		Xml=5,
		Perl=6,
		Sql=7,
		VB=8,
		Properties=9,
		ErrorList=10,
		MakeFile=11,
		Batch=12,
		XCode=13,
		Latex=14,
		Lua=15,
		Diff=16,
		Conf=17,
		Pascal=18,
		Ave=19,
		Ada=20,
		Lisp=21,
		Ruby=22,
		Eiffel=23,
		EiffelKw=24,
		Tcl=25,
		NnCronTab=26,
		Bullant=27,
		VBScript=28,
		Asp=29,
		Php=30,
		Baan=31,
		MatLab=32,
			Scriptol=33,
		Asm=34,
		CppNoCase=35,
		Fortran=36,
		F77=37,
		Css=38,
		Pov=39,
		Lout=40,
		EScript=41,
		Ps=42,
		Nsis=43,
		Mmixal=44,
		Clw=45,
		ClwNoCase=46,
		Lot=47,
		Yaml=48,
		Tex=49,
		MetaPost=50,
		PowerBasic=51,
		Forth=52,
		ErLang=53,
		Octave=54,
		MsSql=55,
		Verilog=56,
		Kix=57,
		Gui4Cli=58,
		Specman=59,
		Au3=60,
		Apdl=61,
		Bash=62,
		Asn1 = 63,
		Vhdl = 64,
		Caml = 65,
		BlitzBasic = 66,
		PureBasic = 67,
		Haskell = 68,
		PhpScript = 69,
		Tads3 = 70,
		Rebol = 71,
		Smalltalk = 72,
		Flagship = 73,
		CSound = 74,
		FreeBasic = 75,
		InnoSetup = 76,
		Opal = 77,
		Spice = 78,
		D = 79,
		CMake = 80,
		Gap = 81,
		Plm = 82,
		Progress = 83,
		Automatic=1000
	}

	public enum WhiteSpaceMode
	{
		Invisible=0,
		VisibleAlways=1,
		VisibleAfterIndent=2,
	}
	public enum EndOfLineMode
	{
		Crlf=0,
		CR=1,
		LF=2,
	}
	public enum MarkerSymbol
	{
		Circle=0,
		RoundRectangle=1,
		Arrow=2,
		SmallRect=3,
		ShortArrow=4,
		Empty=5,
		ArrowDown=6,
		Minus=7,
		Plus=8,
		VLine=9,
		LCorner=10,
		TCorner=11,
		BoxPlus=12,
		BoxPlusConnected=13,
		BoxMinus=14,
		BoxMinusConnected=15,
		LCornerCurve=16,
		TCornerCurve=17,
		CirclePlus=18,
		CirclePlusConnected=19,
		CircleMinus=20,
		CircleMinusConnected=21,
		Background=22,
		Ellipsis=23,
		Arrows=24,
		PixMap=25,
		FullRectangle=26,
		Character=10000,
	}
	public enum MarkerOutline
	{
		FolderEnd=25,
		FolderOpenMid=26,
		FolderMidTail=27,
		FolderTail=28,
		FolderSub=29,
		Folder=30,
		FolderOpen=31,
	}
	public enum MarginType
	{
		Symbol=0,
		Number=1,
		Back=2,
		Fore=3,
	}
	public enum StylesCommon
	{
		Default=32,
		LineNumber=33,
		BraceLight=34,
		BraceBad=35,
		ControlChar=36,
		IndentGuide=37,
		CallTip=38,
		LastPredefined=39,
		Max=127,
	}
	public enum CharacterSet
	{
		Ansi=0,
		Default=1,
		Baltic=186,
		Chinesebig5=136,
		EastEurope=238,
		Gb2312=134,
		Greek=161,
		Hangul=129,
		Mac=77,
		Oem=255,
		Russian=204,
		Cyrillic=1251,
		ShiftJis=128,
		Symbol=2,
		Turkish=162,
		Johab=130,
		Hebrew=177,
		Arabic=178,
		Vietnamese=163,
		Thai=222,
		CharSet885915=1000,
	}
	public enum StyleCase
	{
		Mixed=0,
		Upper=1,
		Lower=2,
	}
	public enum IndicatorStyle
	{
		Plain=0,
		Squiggle=1,
		TT=2,
		Diagonal=3,
		Strike=4,
		Hidden=5,
		Box=6,
		RoundBox=7,
	}
	public enum PrintColorMode
	{
		Normal=0,
		InvertLight=1,
		BlackOnWhite=2,
		ColorOnWhite=3,
		ColorOnWhiteDefaultBackground=4,
	}

	public enum WrapMode
	{
		None=0,
		Word=1,
		Char=2,
	}

	[Flags]
	public enum WrapVisualFlag
	{
		None=0x0000,
		End=0x0001,
		Start=0x0002,
	}

	[Flags]
	public enum WrapVisualLocation
	{
		Default=0x0000,
		EndByText=0x0001,
		StartByText=0x0002,
	}
	public enum LineCache
	{
		None=0,
		Caret=1,
		Page=2,
		Document=3,
	}
	public enum EdgeMode
	{
		None=0,
		Line=1,
		Background=2,
	}
	public enum CursorShape
	{
		Normal=-1,
		Wait=4,
	}
	public enum CaretPolicy
	{
		Slop=0x01,
		Strict=0x04,
		Jumps=0x10,
		Even=0x08,
	}
	public enum SelectionMode
	{
		Stream=0,
		Rectangle=1,
		Lines=2,
	}
	public enum ModificationFlags
	{
		InsertText=0x1,
		DeleteText=0x2,
		ChangeStyle=0x4,
		ChangeFold=0x8,
		User=0x10,
		Undo=0x20,
		Redo=0x40,
		StepInUndoRedo=0x100,
		ChangeMarker=0x200,
		BeforeInsert=0x400,
		BeforeDelete=0x800,
	}
	//public enum Keys
	//{
	//    Down=300,
	//    Up=301,
	//    Left=302,
	//    Right=303,
	//    Home=304,
	//    End=305,
	//    Prior=306,
	//    Next=307,
	//    Delete=308,
	//    Insert=309,
	//    Escape=7,
	//    Back=8,
	//    Tab=9,
	//    Return=13,
	//    Add=310,
	//    Subtract=311,
	//    Divide=312,
	//}
	public enum KeyMod
	{
		Norm=0,
		Shift=1,
		Ctrl=2,
		Alt=4,
	}
	public enum Events : uint
	{
		StyleNeeded=2000,
		CharAdded=2001,
		SavePointReached=2002,
		SavePointLeft=2003,
		ModifyAttemptRO=2004,
		SCKey=2005,
		SCDoubleClick=2006,
		UpdateUI=2007,
		Modified=2008,
		MacroRecord=2009,
		MarginClick=2010,
		NeedShown=2011,
		Painted=2013,
		UserListSelection=2014,
		UriDropped=2015,
		DwellStart=2016,
		DwellEnd=2017,
		SCZoom=2018,
		HotspotClick=2019,
		HotspotDoubleClick=2020,
		CallTipClick=2021
	}

	public enum CopyFormat
	{
		Text,
		Rtf,
		Html
	}

	public enum CallTipArrow
	{
		None = 0,
		Up = 1,
		Down = 2
	}

	//	Next = 10025
	public enum BindableCommand
	{
		BackTab = 2328,
		BeginUndoAction = 2078,
		CallTipCancel = 2201,
		Cancel = 2325,
		CharLeft = 2304,
		CharLeftExtend = 2305,
		CharLeftRectExtend = 2428,
		CharRight = 2306,
		CharRightExtend = 2307,
		CharRightRectExtend = 2429,
		ChooseCaretX = 2399,
		Clear = 2180,
		ClearAll = 2004,
		ClearAllCmdKeys = 2072,
		ClearDocumentStyle = 2005,
		ClearRegisteredImages = 2408,
		Copy = 2178,
		Cut = 2177,
		DeleteBack = 2326,
		DeleteBackNotLine = 2344,
		DelLineLeft = 2395,
		DelLineRight = 2396,
		DelWordLeft = 2335,
		DelWordRight = 2336,
		DocumentEnd = 2318,
		DocumentEndExtend = 2319,
		DocumentNavigateBackward = 10018,
		DocumentNavigateForward = 10019,
		DocumentStart = 2316,
		DocumentStartExtend = 2317,
		DropMarkerCollect = 10008,
		DropMarkerDrop = 10007,
		EditToggleOvertype = 2324,
		EmptyUndoBuffer = 2175,
		EndUndoAction = 2079,
		FormFeed = 2330,
		GrabFocus = 2400,
		Home = 2312,
		HomeDisplay = 2345,
		HomeDisplayExtend = 2346,
		HomeExtend = 2313,
		HomeRectExtend = 2430,
		HomeWrap = 2349,
		HomeWrapExtend = 2450,
		IncrementalSearch = 10015,
		LineCopy = 2455,
		LineComment = 10016,
		LineCut = 2337,
		LineDelete = 2338,
		LineDown = 2300,
		LineDownExtend = 2301,
		LineDownRectExtend = 2426,
		LineDuplicate = 2404,
		LineEnd = 2314,
		LineEndDisplay = 2347,
		LineEndDisplayExtend = 2348,
		LineEndExtend = 2315,
		LineEndRectExtend = 2432,
		LineEndWrap = 2451,
		LineEndWrapExtend = 2452,
		LineScrollDown = 2342,
		LineScrollUp = 2343,
		LinesJoin = 2288,
		LineTranspose = 2339,
		LineUncomment = 10017,
		LineUp = 2302,
		LineUpExtend = 2303,
		LineUpRectExtend = 2427,
		LowerCase = 2340,
		MoveCaretInsideView = 2401,
		NewLine = 2329,
		Null = 2172,
		PageDown = 2322,
		PageDownExtend = 2323,
		PageDownRectExtend = 2434,
		PageUp = 2320,
		PageUpExtend = 2321,
		PageUpRectExtend = 2433,
		ParaDown = 2413,
		ParaDownExtend = 2414,
		ParaUp = 2415,
		ParaUpExtend = 2416,
		Print = 10009,
		PrintPreview = 10010,
		Paste = 2179,
		Redo = 2011,
		ScrollCaret = 2169,
		SearchAnchor = 2366,
		SelectAll = 2013,
		SelectionDuplicate = 2469,
		SetCharsDefault = 2444,
		SetSavePoint = 2014,
		SetZoom = 2373,
		ShowSurroundWithList = 10023,
		ShowGoTo = 10024,
		StartRecord = 3001,
		StreamComment=10021,
		StopRecord = 3002,
		StutteredPageDown = 2437,
		StutteredPageDownExtend = 2438,
		StutteredPageUp = 2435,
		StutteredPageUpExtend = 2436,
		StyleClearAll = 2050,
		StyleResetDefault = 2058,
		Tab = 2327,
		TargetFromSelection = 2287,
		ToggleCaretSticky = 2459,
		ToggleLineComment = 10020,
		Undo = 2176,
		UpperCase = 2341,
		VCHome = 2331,
		VCHomeExtend = 2332,
		VCHomeRectExtend = 2431,
		VCHomeWrap = 2453,
		VCHomeWrapExtend = 2454,
		WordLeft = 2308,
		WordLeftEnd = 2439,
		WordLeftEndExtend = 2440,
		WordLeftExtend = 2309,
		WordPartLeft = 2390,
		WordPartLeftExtend = 2391,
		WordPartRight = 2392,
		WordPartRightExtend = 2393,
		WordRight = 2310,
		WordRightEnd = 2441,
		WordRightEndExtend = 2442,
		WordRightExtend = 2311
	}

	public enum SmartIndent
	{
		None = 0,
		CPP = 1,
		CPP2 = 4,
		Simple = 2
	}
	
}
