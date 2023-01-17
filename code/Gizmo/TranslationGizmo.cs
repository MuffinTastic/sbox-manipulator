using Sandbox;

namespace Manipulator.Gizmo;

[GizmoUI( Name = "Translation", Icon = "open_with", Order = 1 )]
public partial class TranslationGizmo : Gizmo
{
	public TranslationGizmo( Session session, Selection selection ) : base( session, selection )
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

	public override void Render()
	{
		if ( !Selection.IsValid() )
			return;

		if ( Dragged is not null )
		{
			Dragged.PreRender( Session );
			Dragged.Render();
		}
		else
		{
			foreach ( var gizmo in SubGizmos )
			{
				gizmo.PreRender( Session );
				gizmo.Render();
			}
		}
	}
}
