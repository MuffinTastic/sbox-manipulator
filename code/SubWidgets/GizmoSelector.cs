using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manipulator.Gizmo;
using Sandbox;
using Tools;

namespace Manipulator.SubWidgets;

public class GizmoSelector : UIBackground
{
	private ManipulatorWidget Manipulator;

	private List<GizmoSelectorButton> buttons = new();

	public GizmoSelector( Widget parent, ManipulatorWidget manipulator ) : base( parent )
	{
		Manipulator = manipulator;

		SetLayout( LayoutMode.LeftToRight );

		Layout.Margin = ManipulatorWidget.UIMargin;
		Layout.Spacing = ManipulatorWidget.UISpacing;

		foreach ( var attribute in GizmoUIAttribute.All )
		{
			var button = new GizmoSelectorButton( this, attribute );

			Layout.Add( button );
			buttons.Add( button );
		}
	}

	public void OnGizmoButtonClicked( GizmoUIAttribute attribute )
	{
		Manipulator.Session?.SetGizmo( attribute );
	}

	public void OnGizmoSet( GizmoUIAttribute attribute )
	{
		foreach ( var button in buttons )
		{
			button.Checked = button.Attribute == attribute;
		}
	}
}
