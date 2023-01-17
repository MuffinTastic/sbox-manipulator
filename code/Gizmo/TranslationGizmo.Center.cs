using Sandbox;
using Manipulator.Extensions;

namespace Manipulator.Gizmo;

public partial class TranslationGizmo
{
	private class CenterGizmo : SubGizmo
	{
		const float CenterGizmoScale = 0.075f;

		BBox bbox;

		public CenterGizmo( Gizmo parent, Axis axis ) : base( parent, axis, "models/gizmo_center.vmdl" )
		{

		}

		public override void Update()
		{
			Vector3 corner1, corner2;

			corner1 = new Vector3( -ModelSizeScaleAdjuster );
			corner2 = new Vector3( ModelSizeScaleAdjuster );

			var scale = Parent.GetCameraAdjustedScale();
			corner1 *= CenterGizmoScale * scale;
			corner2 *= CenterGizmoScale * scale;

			Vector3.Sort( ref corner1, ref corner2 );
			bbox = new BBox( corner1, corner2 );
		}

		public override void StartDrag( Ray ray )
		{
			var origin = Parent.Selection.Position;
			plane = GetAppropriatePlaneForAxis( origin );

			Parent.Session.SnapMouseToWorld( Parent.GetSelectionTransform().Position );
		}

		public override void UpdateDrag( Ray ray )
		{
			var transform = Parent.GetDragTransform();

			Vector3 point = plane.Trace( ray, twosided: true ).GetValueOrDefault();

			var newPosition = transform.PointToWorld( point );
			Parent.UpdateSelectionTransform( point, transform.Rotation, 1.0f );
		}

		public override void Render( Session session )
		{
			var color = GetGizmoColor();
			var transform = Parent.GetSelectionTransform();
			var scale = Parent.GetCameraAdjustedScale();

			var renderTransform = new Transform(
				transform.Position,
				transform.Rotation,
				ModelSizeHalfScaleAdjuster * CenterGizmoScale * scale
			);

			sceneModel.Transform = renderTransform;

			Graphics.Render( sceneModel, null, color, Override );
		}

		public override bool Intersects( Ray ray, out float distance )
		{
			var transform = Parent.GetSelectionTransform();
			ray = transform.RayToLocal( ray );
			var intersects = bbox.Intersection( ray, out var point1, out var _ );
			distance = Vector3.DistanceBetween( point1, ray.Position );
			return intersects;
		}

		public override Plane GetAppropriatePlaneForAxis( Vector3 origin )
		{
			return new Plane( origin, Parent.AxisToVector( Axis.Camera ) );
		}
	}
}
