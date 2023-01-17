using Sandbox;

namespace Manipulator.Gizmo;

[GizmoUI( Name = "Rotation", Icon = "rotate_right", Order = 2 )]
public partial class RotationGizmo : Gizmo
{
	public RotationGizmo( Session session, Selection selection ) : base( session, selection )
	{
		SubGizmos = new SubGizmo[]
		{
			new PlaneGizmo( this, Axis.X ),
			new PlaneGizmo( this, Axis.Y ),
			new PlaneGizmo( this, Axis.Z )
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
