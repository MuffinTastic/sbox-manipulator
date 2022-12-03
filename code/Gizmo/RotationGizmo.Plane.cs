using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manipulator.Extensions;
using Sandbox;

namespace Manipulator.Gizmo;

public partial class RotationGizmo
{
	private class PlaneGizmo : SubGizmo
	{
		const float PlaneGizmoScale = 0.8f;

		Rotation rot;

		public PlaneGizmo( Gizmo parent, Axis axis ) : base( parent, axis, "models/gizmo_rotate.vmdl" )
		{
			plane = GetAppropriatePlaneForAxis();

			var direction = Parent.AxisToVector( Axis );
			var up = direction.z.AlmostEqual( 0.0f ) ? Vector3.Forward : Vector3.Up;
			rot = Rotation.LookAt( direction, up );
		}

		public override void Update()
		{

		}

		Vector3 startAngle;

		public override void StartDrag( Ray ray )
		{
			var transform = Parent.GetDragTransform();
			ray = transform.RayToLocal( ray );

			var point = plane.Trace( ray, twosided: true ).GetValueOrDefault();

			startAngle = point.Normal;
		}

		public override void UpdateDrag( Ray ray )
		{
			var transform = Parent.GetDragTransform();
			ray = transform.RayToLocal( ray );

			Vector3 point = plane.Trace( ray, twosided: true ).GetValueOrDefault();

			var newAngle = point.Normal;

			var change = Vector3.GetAngle( startAngle, newAngle );
			
			var cross = Vector3.Cross( startAngle, newAngle );
			cross *= plane.Normal;
			var clockwise = (cross.x + cross.y + cross.z) < 0.0f;
			if ( clockwise )
				change *= -1.0f;

			Rotation newRotation;

			if ( Axis == Axis.X )
			{
				newRotation = Rotation.FromRoll( change );
			}
			else if ( Axis == Axis.Y )
			{
				newRotation = Rotation.FromPitch( change );
			}
			else // Axis.Z
			{
				newRotation = Rotation.FromYaw( change );
			}

			newRotation = transform.RotationToWorld( newRotation );

			Parent.UpdateSelectionTransform( transform.Position, newRotation, 1.0f );
		}

		public override void Render( Session session )
		{
			var color = GetGizmoColor();

			var transform = Parent.GetSelectionTransform();
			var scale = Parent.GetCameraAdjustedScale();

			var renderTransform = new Transform(
				transform.Position,
				transform.Rotation * rot,
				ModelSizeHalfScaleAdjuster * PlaneGizmoScale * scale
			);

			sceneModel.Transform = renderTransform;

			Graphics.Render( sceneModel, null, color, Override );
		}

		public override bool Intersects( Ray ray, out float distance )
		{
			distance = 0.0f;
			var transform = Parent.GetSelectionTransform();
			ray = transform.RayToLocal( ray );

			var scale = Parent.GetCameraAdjustedScale();
			var intersection = plane.Trace( ray, twosided: true );
			
			if ( intersection is null )
				return false;
			
			var maxDist = ModelSizeHalfScaleAdjuster * PlaneGizmoScale * scale * 8.0f;
			var distanceFromCenter = Vector3.DistanceBetween( Vector3.Zero, intersection.Value );
			var intersects = distanceFromCenter < maxDist;

			distance = Vector3.DistanceBetween( ray.Origin, intersection.Value );
			return intersects;
		}

		public override Plane GetAppropriatePlaneForAxis( Vector3 origin = default )
		{
			return new Plane( Vector3.Zero, Parent.AxisToVector( Axis ) );
		}
	}
}
