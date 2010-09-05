using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class DropMarkers : ScintillaHelperBase
	{
		internal DropMarkers(Scintilla scintilla) : base(scintilla) { }

		private Stack<DropMarker> _markerStack = new Stack<DropMarker>();
		private static Dictionary<string, Stack<DropMarker>> _sharedStack = new Dictionary<string, Stack<DropMarker>>();

		internal bool ShouldSerialize()
		{
			return ShouldSerializeSharedStackName();
		}

		#region SharedStackName
		private string _sharedStackName = string.Empty;
		public string SharedStackName
		{
			get
			{
				return _sharedStackName;
			}
			set
			{
				if (value == null)
					value = string.Empty;

				if (_sharedStackName == value)
					return;

				
				if (value == string.Empty)
				{
					//	If we had a shared stack name but are now clearing it
					//	we need to create our own private DropMarkerStack again
					_markerStack = new Stack<DropMarker>();

					//	If this was the last subscriber of a shared stack
					//	remove the name to free up resources
					if (_sharedStack.ContainsKey(_sharedStackName) && _sharedStack[_sharedStackName].Count == 1)
						_sharedStack.Remove(_sharedStackName);
				}
				else
				{
					//	We're using one of the shared stacks. Of course if it hasn't 
					//	already been registered with the list we need to create it.
					if (!_sharedStack.ContainsKey(_sharedStackName))
						_sharedStack[_sharedStackName] = new Stack<DropMarker>();

					_markerStack = _sharedStack[_sharedStackName];
				}

				_sharedStackName = value;
			}
		}

		private bool ShouldSerializeSharedStackName()
		{
			return _sharedStackName != string.Empty;
		}

		private void ResetSharedStackName()
		{
			_sharedStackName = string.Empty;
		} 
		#endregion

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Stack<DropMarker> MarkerStack
		{
			get
			{
				return _markerStack;
			}

			//	That's right kids you can actually provide your own MarkerStack. This
			//	is really useful for MDI applications where you want a single master
			//	MarkerStack that will automatically switch documents (a la CodeRush).
			//	Of course you can let the control do this for you automatically by 
			//	setting the SharedStackName property of multiple instances.
			set
            {
            	_markerStack = value;
            }
		}

		private DropMarkerList _allDocumentDropMarkers = new DropMarkerList();
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DropMarkerList AllDocumentDropMarkers
		{
			get
			{
				return _allDocumentDropMarkers;
			}
			set
			{
				_allDocumentDropMarkers = value;
			}
		}

		public DropMarker Drop()
		{
			return Drop(NativeScintilla.GetCurrentPos());
		}

		public DropMarker Drop(int position)
		{
			DropMarker dm = new DropMarker(position, position, getCurrentTopOffset(), Scintilla);
			_allDocumentDropMarkers.Add(dm);
			_markerStack.Push(dm);
			Scintilla.ManagedRanges.Add(dm);

			//	Force the Drop Marker to paint
			Scintilla.Invalidate(dm.GetClientRectangle());
			return dm;
		}

		public void Collect()
		{
			while (_markerStack.Count > 0)
			{
				DropMarker dm = _markerStack.Pop();
				
				//	If the Drop Marker was deleted in the document by
				//	a user action it will be disposed but not removed
				//	from the marker stack. In this case just pretend
				//	like it doesn't exist and go on to the next one
				if (dm.IsDisposed)
					continue;

				//	The MarkerCollection fires a cancellable event.
				//	If it is canclled the Collect() method will return
				//	false. In this case we need to push the marker back
				//	on the stack so that it will still be collected in
				//	the future.
				if (!dm.Collect())
					_markerStack.Push(dm);

				return;
			}
		}

		private int getCurrentTopOffset()
		{
			return -1;
		}

	}

	public class DropMarkerList : System.Collections.ObjectModel.KeyedCollection<Guid, DropMarker>
	{
		protected override Guid GetKeyForItem(DropMarker item)
		{
			return item.Key;
		}
	}

	public class DropMarker : ManagedRange
	{
		private int _topOffset;
		public int TopOffset
		{
			get
			{
				return _topOffset;
			}
			set
			{
				_topOffset = value;
			}
		}

		private Guid _key = Guid.NewGuid();
		public Guid Key
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
			}
		}


		public DropMarker(int start, int end, int topOffset, Scintilla scintilla)
			: base(start, end, scintilla)
		{
			base.Start		= start;
			base.End		= end;
			this._topOffset = topOffset;
		}

		public override void Change(int newStart, int newEnd)
		{
			Invalidate();			
			//	This actually changes Start and End
			base.Change(newStart, newEnd);
		}

		public void Invalidate()
		{
			if(Scintilla != null && Start > 0)
			{
				//	Invalidate the old Marker Location so that we don't get "Ghosts"
				Scintilla.Invalidate(GetClientRectangle());
			}
		}

		//	Drop Markers are points, not a spanned range. Though this could change in the future.
		public override bool IsPoint
		{
			get
			{
				return Start == End;
			}
		}

		protected internal override void Paint(Graphics g)
		{
			base.Paint(g);

			if (IsDisposed)
				return;

			int x = NativeScintilla.PointXFromPosition(Start);
			int y = NativeScintilla.PointYFromPosition(Start) + NativeScintilla.TextHeight(0) - 2;

			//	Draw a red Triangle with a dark red border at the marker position
			g.FillPolygon(Brushes.Red, new Point[] { new Point(x-2, y+4), new Point(x, y), new Point(x+2, y+4) });
			g.DrawPolygon(Pens.DarkRed, new Point[] { new Point(x-2, y+4), new Point(x, y), new Point(x+2, y+4) });
		}

		public bool Collect()
		{
			return Collect(true);
		}

		internal bool Collect(bool dispose)
		{
			DropMarkerCollectEventArgs e = new DropMarkerCollectEventArgs(this);
			Scintilla.OnDropMarkerCollect(e);

			if (e.Cancel)
				return false;

			GotoStart();

			if (dispose)
				Dispose();

			return true;
		}

		public override void Dispose()
		{
			Scintilla.DropMarkers.AllDocumentDropMarkers.Remove(this);
			Invalidate();
			base.Dispose();
		}		

		public Rectangle GetClientRectangle()
		{
			int x = NativeScintilla.PointXFromPosition(Start);
			int y = NativeScintilla.PointYFromPosition(Start) + NativeScintilla.TextHeight(0) - 2;

			//	Invalidate the old Marker Location so that we don't get "Ghosts"
			return new Rectangle(x - 2, y, 5, 5);
		}
	}
}
