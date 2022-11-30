using System.Text;
using Sandbox;
using Tools.Inspectors;
using Tools;

namespace Manipulator;

[CanEdit( typeof( Selection ) )]
public partial class SelectionInspector : ScrollArea
{
	const float HeaderHeight = 64 + 8 + 8;

	DisplayInfo displayInfo;

	public Selection Selection
	{
		get;
		set;
	}

	public SelectionInspector( Widget parent, Selection target ) : base( parent )
	{
		Selection = target;
		displayInfo = DisplayInfo.For( target );
		SetLayout( LayoutMode.TopToBottom );

		Canvas = new Widget( this );
		Canvas.SetLayout( LayoutMode.TopToBottom );
		Layout.Add( Canvas );

		Canvas.Layout.Margin = new( 0, 0, 0, 0 );
		Canvas.SetSizeMode( SizeMode.CanGrow, SizeMode.CanGrow );

		// Main row
		{
			var icon = new IconWidget( "pinch", this );
			icon.Padding = 12;
			icon.BackgroundColor = Theme.Primary;
			icon.ForegroundColor = Theme.White;
			icon.MinimumSize = 48;

			var head = Canvas.Layout.AddRow();
			head.Spacing = 16;
			head.Add( icon );
			head.Margin = new Sandbox.UI.Margin( 12, 12, 16, 12 );

			var col = head.AddColumn( 0 );
			AddHeaderColumn( col );
		}

		Canvas.Layout.AddSeparator();

		// Transform Row
		{
			var head = Canvas.Layout.AddColumn();
			head.Spacing = 16;
			head.Margin = new Sandbox.UI.Margin( 12, 12, 16, 12 );

			var sheet = new Tools.Copy.PropertySheet( this );

			head.Add( sheet );

			sheet.AddProperty( Selection, "Position" );
			sheet.AddProperty( Selection, "Rotation" );
		}

		/*
		Canvas.Layout.AddSeparator();

		var publish = new PropertySheet( Canvas );
		Canvas.Layout.Add( publish );
		Canvas.Layout.AddStretchCell();
		publish.Target = target;*/


		Enabled = true;
	}

	void AddHeaderColumn( Layout column )
	{
		column.Spacing = 4;

		{
			var row = column.AddRow();
			row.Spacing = 4;

			var header = new SelectionHeaderText( Selection );

			Selection.OnUpdated += header.UpdateCount;

			row.Add( header, 0 );
		}
	}
}

internal class SelectionHeaderText : Widget
{
	readonly Selection Selection;
	string Text;

	public SelectionHeaderText( Selection selection, Widget parent = null ) : base( parent )
	{
		Selection = selection;
		UpdateCount();
	}

	protected override void OnPaint()
	{
		Paint.SetPen( Theme.ControlText );
		Paint.SetDefaultFont( 12 );

		Paint.DrawText( ContentRect.Shrink( 8.0f ), Text, TextFlag.LeftCenter );
	}

	public void UpdateCount()
	{
		if ( !IsValid )
		{
			Selection.OnUpdated -= UpdateCount;
			return;
		}

		var count = Selection.SelectedEntities.Count;

		var ending = count == 1 ? "y" : "ies";
		var sb = new StringBuilder();
		sb.Append( "Selection" );
		sb.Append( " - " );
		sb.Append( count );
		sb.Append( " entit" );
		sb.Append( ending );

		Text = sb.ToString();

		Update();
	}
}
