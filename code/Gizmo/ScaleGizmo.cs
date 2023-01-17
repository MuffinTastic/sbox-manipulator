using Sandbox;

namespace Manipulator.Gizmo;

[GizmoUI( Name = "Scale", Icon = "open_in_full", Order = 3 )]
public partial class ScaleGizmo : Gizmo
{
	public ScaleGizmo( Session session, Selection selection ) : base( session, selection )
	{
		SubGizmos = new SubGizmo[]
		{
			new CenterGizmo( this, Axis.Camera )
		};
	}

	public override void Render()
	{
		if ( !Selection.IsValid() )
			return;

		foreach ( var gizmo in SubGizmos )
		{
			gizmo.PreRender( Session );
			gizmo.Render();
		}
	}
}
