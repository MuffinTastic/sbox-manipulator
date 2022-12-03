using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manipulator.Gizmo;
using Sandbox;
using Tools;

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
		foreach ( var button in Buttons )
		{
			var gizmoButton = (GizmoSelectorButton) button;
			var buttonType = gizmoButton.Attribute.Type;
			var gizmoType = Manipulator.Session?.Gizmo.GetType();

			button.Checked = buttonType == gizmoType;
		}
	}

	public override void OnButtonClicked( SelectorButton button )
	{
		var gizmoButton = (GizmoSelectorButton) button;

		base.OnButtonClicked( gizmoButton );

		Manipulator.Session?.SetGizmo( gizmoButton.Attribute );
	}

	public void OnGizmoSet( int index )
	{
		SetButtonActiveIndex( index );
	}
}
