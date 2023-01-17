using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manipulator.Gizmo;
using Sandbox;
using Editor;

namespace Manipulator.SubWidgets;

public class GizmoSelector : Selector
{
	private ManipulatorWidget Manipulator { get; set; }

	public GizmoSelector( Widget parent, ManipulatorWidget manipulator ) : base( parent )
	{
		Manipulator = manipulator;

		Log.Info( Manipulator );

		BuildUI();
	}

	protected override void BuildButtons()
	{
		foreach ( var attribute in GizmoUIAttribute.All )
		{
			var button = new GizmoSelectorButton( this, attribute );

			AddButton( button );
		}
	}

	public void OnActivate()
	{
		SetButtonActiveFunc( ( SelectorButton button ) => (button as GizmoSelectorButton).Attribute.Type == Manipulator.Session?.Gizmo.GetType() );
	}

	public override void OnButtonClicked( SelectorButton button )
	{
		var gizmoButton = (GizmoSelectorButton) button;

		base.OnButtonClicked( gizmoButton );

		Manipulator.Session?.SetGizmo( gizmoButton.Attribute );
	}

	public void OnGizmoSet( int index )
	{
		SetButtonActiveFunc( ( SelectorButton button ) => (button as GizmoSelectorButton).Attribute.Index == index );
	}
}
