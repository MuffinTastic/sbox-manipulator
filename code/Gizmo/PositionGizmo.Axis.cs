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
	private class AxisGizmo : SubGizmo
	{
		public AxisGizmo( Gizmo parent, Axis axis ) : base( parent, axis, "models/gizmo_arrow.vmdl" )
		{

		}

		public override void Update()
		{
			Vector3 corner1, corner2;

			var direction = GetAppropriateAxisDirection();
			var up = direction.z.AlmostEqual( 0.0f ) ? Vector3.Left : Vector3.Up;
			var rot = Rotation.LookAt( direction, up );

			corner1 = rot.Forward * (Width + Gap) + (rot.Right + rot.Down) * Width;
			corner2 = rot.Forward * Length + (rot.Left + rot.Up) * Width;

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

			Vector3 delta = plane.Trace( ray, twosided: true ).GetValueOrDefault();

			var axisVector = Parent.AxisToVector( Axis );
			delta = axisVector.Dot( delta ) * axisVector;

			var newPosition = transform.PointToWorld( delta );
			Parent.Move( newPosition, transform.Rotation );
		}

		public override void Render( Session session )
		{
			var color = GetGizmoColor();

			var direction = GetAppropriateAxisDirection();
			var up = direction.z.AlmostEqual( 0.0f ) ? Vector3.Left : Vector3.Up;
			var rot = Rotation.LookAt( direction, up );

			var transform = Parent.GetSelectionTransform();
			var scale = Parent.GetCameraAdjustedScale();

			var renderTransform = new Transform(
				transform.Position + transform.Rotation * (rot.Forward * Gap * scale),
				transform.Rotation * rot,
				RenderSize * scale
			);

			sceneModel.Transform = renderTransform;

			Graphics.Render( sceneModel, null, color, Override );
		}

		public override bool Intersects( Ray ray )
		{
			var transform = Parent.GetSelectionTransform();
			ray = transform.RayToLocal( ray );
			return bbox.Intersection( ray, out var _, out var _ );
		}

		public Vector3 GetAppropriateAxisDirection()
		{
			var transform = Parent.GetSelectionTransform();

			var axisVector = Parent.AxisToVector( Axis );
			var camDiff = transform.PointToLocal( Parent.Session.MainCamera.Position );
			var diffNormal = camDiff.Normal;

			if ( diffNormal.Dot( axisVector ) < 0 )
				axisVector *= -1.0f;

			return axisVector;
		}

		public override Plane GetAppropriatePlaneForAxis( Vector3 origin )
		{
			var camDiff = origin - Parent.Session.MainCamera.Position;
			var diffNormal = camDiff.Normal;
			var diffNormalAbs = diffNormal.Abs();

			var xAxisVec = Parent.AxisToVector( Axis.X );
			var yAxisVec = Parent.AxisToVector( Axis.Y );
			var zAxisVec = Parent.AxisToVector( Axis.Z );

			Vector3 planeNormal = Vector3.Zero;

			if ( Axis == Axis.X )
			{
				if ( diffNormalAbs.DistanceSquared( yAxisVec ) < diffNormalAbs.DistanceSquared( zAxisVec ) )
				{
					planeNormal = yAxisVec;
				}
				else
				{
					planeNormal = zAxisVec;
				}
			}
			else if ( Axis == Axis.Y )
			{
				if ( diffNormalAbs.DistanceSquared( xAxisVec ) < diffNormalAbs.DistanceSquared( zAxisVec ) )
				{
					planeNormal = xAxisVec;
				}
				else
				{
					planeNormal = zAxisVec;
				}
			}
			else if ( Axis == Axis.Z )
			{
				if ( diffNormalAbs.DistanceSquared( xAxisVec ) < diffNormalAbs.DistanceSquared( yAxisVec ) )
				{
					planeNormal = xAxisVec;
				}
				else
				{
					planeNormal = yAxisVec;
				}
			}

			// because we're doing our calculations in abs space we lose some directionality
			// we can get it back like this
			if ( diffNormal.Dot( planeNormal ) > 0 )
				planeNormal *= -1.0f;

			return new Plane( Vector3.Zero, planeNormal );
		}
	}
}
