using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Editor;

namespace Manipulator.SubWidgets;

public class BindInput : Widget
{
	public PropertyInfo Property { get; private set; }

	public bool Invalid { get; set; }

	private BindsWindow ParentWindow;
	private KeyCode LastValue;
	private bool EnteringKey = false;

	private string Text;

	public BindInput( PropertyInfo prop, BindsWindow parent ) : base( parent )
	{
		Property = prop;
		ParentWindow = parent;

		FocusMode = FocusMode.Click;

		LastValue = (KeyCode)prop.GetValue( parent.UnsavedBinds );
		SetText( LastValue );
	}

	private void SetText( KeyCode key )
	{
		Text = key.ToString();
	}

	protected override void OnMousePress( MouseEvent e )
	{
		if ( e.LeftMouseButton )
		{
			EnteringKey = true;
			Cursor = CursorShape.IBeam;
			Text = "";

			e.Accepted = true;
			return;
		}

		base.OnMousePress( e );
		e.Accepted = true;
	}

	public void Reset()
	{
		EnteringKey = false;
		Cursor = CursorShape.None;
		LastValue = (KeyCode)Property.GetValue( ParentWindow.UnsavedBinds );
		SetText( LastValue );
	}

	protected override void OnKeyPress( KeyEvent e )
	{
		if ( EnteringKey )
		{
			ParentWindow.ChangeBind( Property, e.Key );
			Reset();
		}
	}

	protected override void OnMouseLeave()
	{
		if ( EnteringKey )
		{
			// Nothing has been entered yet, but we're leaving.
			// Take this as a cancelling event, reset
			Reset();
		}
	}

	protected override void OnPaint()
	{
		base.OnPaint();

		var col = Invalid ? Color.Red : Theme.ControlText;

		var r = LocalRect;

		if ( !Enabled )
		{
			col = col.WithAlpha( 0.3f );
		}
		else if ( Paint.HasMouseOver )
		{
			Paint.ClearPen();
			Paint.SetBrush( col.WithAlpha( 0.25f ) );
			Paint.DrawRect( r, ManipulatorWidget.UIRounding );

			col = Theme.White;
		}
		else
		{
			Paint.ClearPen();
			Paint.SetBrush( col.WithAlpha( 0.1f ) );
			Paint.DrawRect( r, ManipulatorWidget.UIRounding );
		}

		if ( Paint.HasFocus )
		{
			Paint.ClearPen();
			Paint.SetBrush( Theme.Primary.WithAlpha( 0.75f ) );
			Paint.DrawRect( r, ManipulatorWidget.UIRounding );
		}

		Paint.SetPen( col );
		Paint.DrawText( r, Text );
	}
}
