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
}
