using Sandbox;
using Manipulator.Extensions;

namespace Manipulator.Gizmo;

public partial class TranslationGizmo
{
	private class PlaneGizmo : SubGizmo
	{
		const float PlaneGizmoOffsetScale = 0.55f;
		const float PlaneGizmoScale = 0.1f;

		BBox bbox;

		public PlaneGizmo( Gizmo parent, Axis axis ) : base( parent, axis, "models/gizmo_plane.vmdl" )
		{
			plane = GetAppropriatePlaneForAxis();
		}

		public override void Update()
		{
			Vector3 corner1, corner2;

			var direction = Parent.AxisToVector( Axis );
			var up = direction.z.AlmostEqual( 0.0f ) ? Vector3.Forward : Vector3.Up;
			var rot = Rotation.LookAt( direction, up );

			var camera = Parent.Session.Camera.Position;
			var offset = SnapInQuadrant( camera );

			var center = offset * PlaneGizmoOffsetScale;

			corner1 = center + (rot.Left + rot.Up) * 0.25f;
			corner2 = center + (rot.Right + rot.Down) * 0.25f;

			var scale = Parent.GetCameraAdjustedScale();
			corner1 *= ModelSizeScaleAdjuster * scale;
			corner2 *= ModelSizeScaleAdjuster * scale;

			Vector3.Sort( ref corner1, ref corner2 );
			bbox = new BBox( corner1, corner2 );
		}

		public override void StartDrag( Ray ray )
		{
			Parent.Session.SnapMouseToWorld( Parent.GetSelectionTransform().Position );
		}

		public override void UpdateDrag( Ray ray )
		{
			var transform = Parent.GetDragTransform();
			ray = transform.RayToLocal( ray );

			Vector3 point = plane.Trace( ray, twosided: true ).GetValueOrDefault();

			var newPosition = transform.PointToWorld( point );
			Parent.UpdateSelectionTransform( newPosition, transform.Rotation, 1.0f );
		}

		public override void Render( Session session )
		{
			var color = GetGizmoColor();

			var direction = Parent.AxisToVector( Axis );
			var up = direction.z.AlmostEqual( 0.0f ) ? Vector3.Forward : Vector3.Up;
			var rot = Rotation.LookAt( direction, up );

			var transform = Parent.GetSelectionTransform();
			var scale = Parent.GetCameraAdjustedScale();

			var camera = Parent.Session.Camera.Position;
			var offset = SnapInQuadrant( camera );

			var renderTransform = new Transform(
				transform.Position + transform.Rotation * offset * PlaneGizmoOffsetScale * ModelSizeScaleAdjuster * scale,
				transform.Rotation * rot,
				ModelSizeHalfScaleAdjuster * PlaneGizmoScale * scale
			);

			sceneModel.Transform = renderTransform;

			Graphics.Render( sceneModel, null, color, OverrideNoCull );
		}

		public override bool Intersects( Ray ray, out float distance )
		{
			var transform = Parent.GetSelectionTransform();
			ray = transform.RayToLocal( ray );
			var intersects = bbox.Intersection( ray, out var point1, out var _ );
			distance = Vector3.DistanceBetween( point1, ray.Position );
			return intersects;
		}

		public override Plane GetAppropriatePlaneForAxis( Vector3 origin = default )
		{
			return new Plane( Vector3.Zero, Parent.AxisToVector( Axis ) );
		}
	}
}
