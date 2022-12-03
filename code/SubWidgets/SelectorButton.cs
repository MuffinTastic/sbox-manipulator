using System;
using Manipulator.Gizmo;
using Tools;

namespace Manipulator.SubWidgets;

/// <summary>
/// A button that shows as an icon and tries to keep itself square
/// </summary>
public class SelectorButton : Widget
{
	private Selector Selector;
	public string Icon { get; set; }
	public int Index { get; set; }

	public bool IsToggle { get; set; } = false;

	bool _checked;
	public bool Checked
	{
		get => _checked;
		set
		{
			if ( _checked == value ) return;
			_checked = value;
			Update();
		}
	}


	public SelectorButton( Selector selector ) : base( selector )
	{
		Selector = selector;

		MinimumSize = 30;
		Cursor = CursorShape.Finger;
	}

	protected override void OnMousePress( MouseEvent e )
	{
		if ( e.LeftMouseButton )
		{
			Selector.OnButtonClicked( this );
			e.Accepted = true;
			return;
		}

		base.OnMousePress( e );
		e.Accepted = true;
	}

	protected override void DoLayout()
	{
		base.DoLayout();
	}

	protected override void OnPaint()
	{
		var col = Theme.ControlText;

		var r = LocalRect;

		if ( !Enabled )
		{
			col = col.WithAlpha( 0.3f );
		}
		else if ( Paint.HasMouseOver )
		{
			Paint.ClearPen();
			Paint.SetBrush( col.WithAlpha( 0.1f ) );
			Paint.DrawRect( r, ManipulatorWidget.UIRounding );

			col = Theme.White;
		}

		Paint.SetPen( col );
		Paint.DrawIcon( r, Icon, 20, TextFlag.Center );
	}
}
