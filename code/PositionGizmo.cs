using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sandbox;

namespace Manipulator;

public class PositionGizmo : Gizmo
{
	public PositionGizmo( Session session, Selection selection ) : base( session, selection )
	{
		SubGizmos = new SubGizmo[]
		{
			new CenterGizmo( this, Axis.Camera ),
			new AxisGizmo( this, Axis.X ),
			new AxisGizmo( this, Axis.Y ),
			new AxisGizmo( this, Axis.Z ),
			//new PlaneGizmo( this, Axis.X ),
			//new PlaneGizmo( this, Axis.Y ),
			//new PlaneGizmo( this, Axis.Z )
		};
	}

	// ------------------------------------------------ //

	private class CenterGizmo : SubGizmo
	{
		public CenterGizmo( Gizmo parent, Axis axis ) : base( parent, axis, "models/gizmo_center.vmdl" )
		{

		}

		public override void Update()
		{
			Vector3 corner1, corner2;

			corner1 = new Vector3( -Width );
			corner2 = new Vector3( Width );

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
			//ray = transform.RayToLocal( ray );

			Vector3 point = plane.Trace( ray, twosided: true ).GetValueOrDefault();

			var newPosition = transform.PointToWorld( point );
			Parent.Move( point, transform.Rotation );
		}

		public override void Render( Session session )
		{
			var ray = session.GetCursorRay();

			var intersects = Parent.IsHovering( ray, this );

			var color = Parent.AxisToColor( Axis );
			if ( Parent.Dragged == this || intersects )
				color = Color.Yellow;

			var transform = Parent.GetSelectionTransform();

			var scale = Parent.GetCameraAdjustedScale();

			var renderTransform = new Transform(
				transform.Position,
				transform.Rotation,
				scale * RenderSize
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

		public override Plane GetAppropriatePlaneForAxis( Vector3 origin )
		{
			return new Plane( origin, Parent.AxisToVector( Axis.Camera ) );
		}
	}

	// ------------------------------------------------ //

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
			var ray = session.GetCursorRay();

			var intersects = Parent.IsHovering( ray, this );

			var color = Parent.AxisToColor( Axis );
			if ( Parent.Dragged == this || intersects )
				color = Color.Yellow;

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

	/*
	private class PlaneGizmo : SubGizmo
	{
		public PlaneGizmo( Gizmo parent, Axis axis ) : base( parent, axis )
		{

		}

		public override void Update()
		{
			Vector3 corner1, corner2;

			if ( Axis == Axis.Camera )
			{
				corner1 = new Vector3( -Width );
				corner2 = new Vector3( Width );
			}
			else
			{
				var direction = GetAppropriateAxisDirection( Parent.Selection.Position );
				var up = direction.z.AlmostEqual( 0.0f ) ? Vector3.Left : Vector3.Up;
				var rot = Rotation.LookAt( direction, up );

				corner1 = rot.Forward * (Width + Gap) + (rot.Right + rot.Down) * Width;
				corner2 = rot.Forward * Length + (rot.Left + rot.Up) * Width;
			}

			var scale = Parent.GetCameraAdjustedScale();
			corner1 *= scale;
			corner2 *= scale;

			corner1 += Parent.Selection.Position;
			corner2 += Parent.Selection.Position;

			Vector3.Sort( ref corner1, ref corner2 );
			bbox = new BBox( corner1, corner2 );
		}

		public override void UpdateDrag( Ray ray )
		{
			Vector3 delta = plane.Trace( ray, twosided: true ).GetValueOrDefault() - Parent.Selection.Position;

			if ( Axis != Axis.Camera )
			{
				var axisVector = Parent.AxisToVector( Axis );
				delta = axisVector.Dot( delta ) * axisVector;
			}

			var newPosition = Parent.Selection.Position + delta;
			Parent.Move( newPosition, Parent.Selection.Rotation );
		}

		public override void Render( Session session )
		{
			var ray = session.GetCursorRay();

			var intersects = Parent.IsHovering( ray, this );

			List<Vertex> bboxVertices = new();
			List<ushort> bboxIndices = new();

			var color = Parent.AxisToColor( Axis );
			if ( Parent.Dragged == this || intersects )
				color = Color.Yellow;

			Outlines.BuildLines( bbox.Corners, ref bboxVertices, ref bboxIndices, color );

			Graphics.Draw( bboxVertices.ToArray(), bboxVertices.Count,
				bboxIndices.ToArray(), bboxIndices.Count,
				Outlines.Material, primitiveType: Graphics.PrimitiveType.Lines );
		}

		public override bool Intersects( Ray ray )
		{
			return bbox.Intersection( ray, out var _, out var _ );
		}

		public Vector3 GetAppropriateGizmoOffset( Vector3 origin )
		{
			var point = plane.SnapToPlane( origin );

			// snap point to a quadrant
			var snapped = 
		}

		public override Plane GetAppropriatePlaneForAxis( Vector3 origin )
		{
			if ( Axis == Axis.Camera )
			{
				return Parent.GetCameraAlignedPlane( origin );
			}

			Vector3 planeNormal = Parent.AxisToVector( Axis );
			return new Plane( origin, planeNormal );
		}
	}*/
}
