using Manipulator.Gizmo;
using Tools;

namespace Manipulator.SubWidgets;

/// <summary>
/// A button that shows as an icon and tries to keep itself square
/// </summary>
public class GizmoSelectorButton : Widget
{
	private GizmoSelector Parent;

	public GizmoUIAttribute Attribute { get; init; }

	public string Icon { get; set; }

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


	public GizmoSelectorButton( GizmoSelector parent, GizmoUIAttribute attribute ) : base( parent )
	{
		Parent = parent;
		Attribute = attribute;

		Icon = attribute.Icon;
		MinimumSize = 30;
		StatusTip = attribute.Name;
		ToolTip = $"Switch to the {attribute.Name} gizmo";
		Cursor = CursorShape.Finger;
	}

	protected override void OnMousePress( MouseEvent e )
	{
		if ( e.LeftMouseButton )
		{
			Parent.OnGizmoButtonClicked( Attribute );
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
		var icon = Icon;

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

		if ( Checked )
		{
			Paint.ClearPen();
			Paint.SetBrush( Theme.Primary.WithAlpha( 0.5f ) );
			Paint.DrawRect( r, ManipulatorWidget.UIRounding );
		}

		Paint.SetPen( col );
		Paint.DrawIcon( r, Icon, 20, TextFlag.Center );
	}
}
