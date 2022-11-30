namespace Manipulator.Gizmo;

public partial class PositionGizmo : Gizmo
{
	public PositionGizmo( Session session, Selection selection ) : base( session, selection )
	{
		SubGizmos = new SubGizmo[]
		{
			new CenterGizmo( this, Axis.Camera ),
			new AxisGizmo( this, Axis.X ),
			new AxisGizmo( this, Axis.Y ),
			new AxisGizmo( this, Axis.Z ),
			new PlaneGizmo( this, Axis.X ),
			new PlaneGizmo( this, Axis.Y ),
			new PlaneGizmo( this, Axis.Z )
		};
	}
}
