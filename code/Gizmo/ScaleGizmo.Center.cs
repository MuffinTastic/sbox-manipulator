using Sandbox;
using Manipulator.Extensions;

namespace Manipulator.Gizmo;

public partial class ScaleGizmo
{
	private class CenterGizmo : SubGizmo
	{
		const float CenterGizmoScale = 0.075f;

		BBox bbox;

		Rotation startRot;

		public CenterGizmo( Gizmo parent, Axis axis ) : base( parent, axis, "models/gizmo_scalar.vmdl" )
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

		Vector3 startPoint;

		public override void StartDrag( Ray ray )
		{
			var origin = Parent.Selection.Position;
			plane = GetAppropriatePlaneForAxis( origin );

			Vector3 point = plane.Trace( ray, twosided: true ).GetValueOrDefault();
			startPoint = Parent.Session.Camera.Rotation.Inverse * point;
		}

		public override void UpdateDrag( Ray ray )
		{
			var transform = Parent.GetDragTransform();

			Vector3 point = plane.Trace( ray, twosided: true ).GetValueOrDefault();
			point = Parent.Session.Camera.Rotation.Inverse * point;

			var newScaleOffset = -(point.y - startPoint.y) / 250.0f;
			var newScale = 1.0f + newScaleOffset.Clamp( -0.99f, float.MaxValue );

			Parent.UpdateSelectionTransform( transform.Position, transform.Rotation, transform.Scale * newScale );
		}

		public override void PreRender( Session session )
		{
			var transform = Parent.GetSelectionTransform();
			var scale = Parent.GetCameraAdjustedScale();

			var renderTransform = new Transform(
				transform.Position,
				transform.Rotation,
				ModelSizeHalfScaleAdjuster * CenterGizmoScale * scale
			);

			sceneModel.Transform = renderTransform;
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
		public override Color GetGizmoColor()
		{
			var ray = Parent.Session.GetCursorRay();
			var intersects = Parent.IsHovering( ray, this );

			if ( Parent.Dragged == this || (Parent.Dragged is null && intersects) )
				return Color.Yellow;

			return Color.Magenta.Desaturate( 0.1f );
		}
	}
}
