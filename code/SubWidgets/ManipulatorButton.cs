using System;
using Editor;

namespace Manipulator.SubWidgets;

/// <summary>
/// A button that shows as an icon and tries to keep itself square
/// </summary>
public class ManipulatorButton : Widget
{
	public string Icon { get; set; }

	public event Action Clicked;

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


	public ManipulatorButton( Widget parent ) : base( parent )
	{
		MinimumSize = 30;
	}

	protected override void OnMousePress( MouseEvent e )
	{
		if ( e.LeftMouseButton )
		{
			if ( IsToggle )
				Checked = !Checked;
			Clicked?.Invoke();
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

		if ( IsToggle && Checked || Paint.HasPressed )
		{
			Paint.ClearPen();
			Paint.SetBrush( Theme.Primary.WithAlpha( 0.75f ) );
			Paint.DrawRect( r, ManipulatorWidget.UIRounding );
		}

		Paint.SetPen( col );
		Paint.DrawIcon( r, Icon, 20, TextFlag.Center );
	}
}
