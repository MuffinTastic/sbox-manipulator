using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manipulator.Extensions;
using Sandbox;

namespace Manipulator.Gizmo;

public partial class PositionGizmo
{
	private class PlaneGizmo : SubGizmo
	{
		const float PlaneGizmoSize = 12.0f;

		public PlaneGizmo( Gizmo parent, Axis axis ) : base( parent, axis, "models/gizmo_plane.vmdl" )
		{

		}

		public override void Update()
		{
			Vector3 corner1, corner2;

			var direction = Parent.AxisToVector( Axis );
			var up = direction.z.AlmostEqual( 0.0f ) ? Vector3.Forward : Vector3.Up;
			var rot = Rotation.LookAt( direction, up );

			var transform = Parent.GetSelectionTransform();

			var camera = Parent.Session.MainCamera.Position;
			var offset = SnapInQuadrant( camera );

			var center = offset * (1.0f / PlaneGizmoSize);

			corner1 = center + (rot.Left + rot.Up) * Gap;
			corner2 = center + (rot.Right + rot.Down) * Gap;

			var scale = Parent.GetCameraAdjustedScale();
			corner1 *= scale;
			corner2 *= scale;

			Vector3.Sort( ref corner1, ref corner2 );
			bbox = new BBox( corner1, corner2 );
		}

		public override void StartDrag()
		{
			base.StartDrag();

			Parent.Session.SnapMouseToWorld( Parent.GetSelectionTransform().Position );
		}

		public override void UpdateDrag( Ray ray )
		{
			var transform = Parent.GetDragTransform();
			ray = transform.RayToLocal( ray );

			Vector3 point = plane.Trace( ray, twosided: true ).GetValueOrDefault();

			var newPosition = transform.PointToWorld( point );
			Parent.Move( newPosition, transform.Rotation );
		}

		public override void Render( Session session )
		{
			var color = GetGizmoColor();

			var direction = Parent.AxisToVector( Axis );
			var up = direction.z.AlmostEqual( 0.0f ) ? Vector3.Forward : Vector3.Up;
			var rot = Rotation.LookAt( direction, up );

			var transform = Parent.GetSelectionTransform();
			var scale = Parent.GetCameraAdjustedScale();

			var camera = Parent.Session.MainCamera.Position;
			var offset = SnapInQuadrant( camera );

			var renderTransform = new Transform(
				transform.Position + transform.Rotation * offset * (1.0f / PlaneGizmoSize) * scale,
				transform.Rotation * rot,
				RenderSize * scale
			);

			sceneModel.Transform = renderTransform;

			Graphics.Render( sceneModel, null, color, OverrideNoCull );
		}

		public override bool Intersects( Ray ray )
		{
			var transform = Parent.GetSelectionTransform();
			ray = transform.RayToLocal( ray );
			return bbox.Intersection( ray, out var _, out var _ );
		}

		public override Plane GetAppropriatePlaneForAxis( Vector3 origin )
		{
			return new Plane( Vector3.Zero, Parent.AxisToVector( Axis ) );
		}
	}
}
