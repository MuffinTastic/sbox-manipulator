using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace Manipulator;

public class PositionGizmo : Gizmo
{
	private AxisGizmo XAxisGizmo;
	private AxisGizmo YAxisGizmo;
	private AxisGizmo ZAxisGizmo;

	private AxisGizmo Dragged;

	public PositionGizmo( Session session, Selection selection ) : base( session, selection )
	{
		XAxisGizmo = new AxisGizmo( this, Axis.X );
		YAxisGizmo = new AxisGizmo( this, Axis.Y );
		ZAxisGizmo = new AxisGizmo( this, Axis.Z );
	}
	public override void StartDrag( Ray ray )
	{
		Dragged = null;

		if ( XAxisGizmo.Intersects( ray ) )
		{
			Dragged = XAxisGizmo;
		}
		else if ( YAxisGizmo.Intersects( ray ) )
		{
			Dragged = YAxisGizmo;
		}
		else if ( ZAxisGizmo.Intersects( ray ) )
		{
			Dragged = ZAxisGizmo;
		}

		if ( Dragged is not null )
		{
			//Session.
		}
	}

	public override void UpdateDrag( Ray ray )
	{
		throw new NotImplementedException();
	}

	public override void StopDrag( Ray ray )
	{
		throw new NotImplementedException();
	}

	public override void Update()
	{
		XAxisGizmo.Update();
		YAxisGizmo.Update();
		ZAxisGizmo.Update();
	}

	public override void Render()
	{
		XAxisGizmo.DebugRender( Session );
		YAxisGizmo.DebugRender( Session );
		ZAxisGizmo.DebugRender( Session );
	}

	public override Plane GetAppropriatePlaneForAxis( Vector3 origin, Axis axis )
	{
		var camDiff = origin - Session.MainCamera.Position;
		var diffNormal = camDiff.Normal;
		var diffNormalAbs = diffNormal.Abs();

		var xAxisVec = AxisToVector( Axis.X );
		var yAxisVec = AxisToVector( Axis.Y );
		var zAxisVec = AxisToVector( Axis.Z );

		Vector3 planeNormal = Vector3.Zero;

		if ( axis == Axis.X )
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
		else if ( axis == Axis.Y )
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
		else if ( axis == Axis.Z )
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

		return new Plane( origin, planeNormal );
	}

	private class AxisGizmo
	{
		public const float Width = 4.0f;

		private readonly Gizmo Parent;
		private readonly Axis Axis;
		private Plane plane;

		private BBox bbox;

		public AxisGizmo( Gizmo parent, Axis axis )
		{
			Parent = parent;
			Axis = axis;
		}

		public void Update()
		{
			var direction = GetAppropriateAxisDirection( Parent.Transform.Position );
			var up = direction.z.AlmostEqual( 0.0f ) ? Vector3.Left : Vector3.Up;
			var rot = Rotation.LookAt( direction, up );

			var corner1 = Parent.Transform.Position + rot.Forward * Width + ( rot.Right + rot.Down) * Width;
			var corner2 = Parent.Transform.Position + rot.Forward * Gizmo.MaxSize + (rot.Left + rot.Up) * Width;

			Vector3.Sort( ref corner1, ref corner2 );
			bbox = new BBox( corner1, corner2 );
		}

		public void DebugRender( Session session )
		{
			var ray = session.GetCursorRay();

			var intersects = bbox.Intersection( ray, out Vector3 point1, out Vector3 point2 );

			List<Vertex> bboxVertices = new();
			List<ushort> bboxIndices = new();

			Outlines.BuildLines( bbox.Corners, ref bboxVertices, ref bboxIndices, intersects ? Color.Magenta : AxisToColor( Axis ) );

			Graphics.Draw( bboxVertices.ToArray(), bboxVertices.Count,
				bboxIndices.ToArray(), bboxIndices.Count,
				Outlines.Material, primitiveType: Graphics.PrimitiveType.Lines );
		}

		public Vector3 GetAppropriateAxisDirection( Vector3 origin )
		{
			var axisVector = AxisToVector( Axis );
			var camDiff = origin - Parent.Session.MainCamera.Position;
			var diffNormal = camDiff.Normal;

			if ( diffNormal.Dot( axisVector ) > 0 )
				axisVector *= -1.0f;

			return axisVector;
		}

		internal bool Intersects( Ray ray )
		{
			return bbox.Intersection( ray, out var _, out var _ );
		}
	}
}
