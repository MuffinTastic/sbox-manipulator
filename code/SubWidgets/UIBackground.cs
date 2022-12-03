using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manipulator.Gizmo;
using Tools;

namespace Manipulator.SubWidgets;
public class UIBackground : Widget
{
	public UIBackground( Widget parent ) : base( parent )
	{
		SetLayout( LayoutMode.LeftToRight );

		Layout.Margin = ManipulatorWidget.UIMargin;
	}

	protected override void OnPaint()
	{
		var col = Theme.ControlBackground;

		var r = LocalRect;

		Paint.ClearPen();
		Paint.SetBrush( col.WithAlpha( 0.5f ) );
		Paint.DrawRect( r, ManipulatorWidget.UIRounding );

	}
}
