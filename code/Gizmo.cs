using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Tools;

namespace Manipulator;

public abstract class Gizmo
{
	public enum Axis
	{
		X, Y, Z,
		Camera
	}

	public Session Session { get; init; }
	public Selection Selection { get; init; }

	public bool IsDragging => Dragged is not null;

	protected SubGizmo[] SubGizmos;

	public Transform DragStartTransform { get; private set; }
	public SubGizmo Dragged { get; private set; }
	public bool Local { get; private set; } = true;

	public Gizmo( Session session, Selection selection )
	{
		Session = session;
		Selection = selection;
	}

	public bool StartDrag( Ray ray )
	{
		if ( !Selection.IsValid() )
			return false;

		Dragged = null;

		Dragged = SubGizmos.FirstOrDefault( g => g.Intersects( ray ) );

		if ( Dragged is not null )
		{
			DragStartTransform = GetSelectionTransform();

			Dragged.StartDrag();
		}

		return Dragged is not null;
	}

	public void UpdateDrag( Ray ray )
	{
		if ( !Selection.IsValid() )
			return;

		Dragged?.UpdateDrag( ray );
	}

	public void StopDrag( Ray ray )
	{
		if ( !Selection.IsValid() )
			return;

		Dragged = null;
	}

	public void ToggleLocal()
	{
		Local = !Local;
	}

	public bool IsHovering( Ray ray )
	{
		if ( !Selection.IsValid() )
			return false;

		return SubGizmos.Any( g => g.Intersects( ray ) );
	}

	public bool IsHovering( Ray ray, SubGizmo subGizmo )
	{
		if ( !Selection.IsValid() )
			return false;

		var hovered = SubGizmos.FirstOrDefault( g => g.Intersects( ray ) );
		return subGizmo == hovered;
	}

	public void Update()
	{
		if ( !Selection.IsValid() )
			return;

		if ( !IsDragging )
		{
			Selection.RebuildTransforms();
		}

		foreach ( var gizmo in SubGizmos )
		{
			gizmo.Update();
		}
	}

	public void Render()
	{
		if ( !Selection.IsValid() )
			return;

		if ( Dragged is not null )
		{
			Dragged.Render( Session );
		}
		else
		{
			foreach ( var gizmo in SubGizmos )
			{
				gizmo.Render( Session );
			}
		}
	}

	public void Move( Vector3 newPosition, Rotation newRotation )
	{
		if ( !Selection.IsValid() )
			return;

		Selection.Position = newPosition;

		if ( Local )
		{
			var transform = GetDragTransform();
			newRotation = transform.Rotation.Inverse * newRotation;
		}

		Selection.Rotation = newRotation;
	}

	public float GetCameraAdjustedScale()
	{
		var pos = Selection.Position;
		var transform = new Transform( Session.MainCamera.Position, Session.MainCamera.Rotation );
		var localPos = transform.PointToLocal( pos );
		return localPos.x;
	}

	public Vector3 AxisToVector( Axis axis )
	{
		switch ( axis )
		{
			case Axis.X: return new Vector3( 1.0f, 0.0f, 0.0f );
			case Axis.Y: return new Vector3( 0.0f, 1.0f, 0.0f );
			case Axis.Z: return new Vector3( 0.0f, 0.0f, 1.0f );
			case Axis.Camera: return Session.MainCamera.Rotation.Forward;
		}

		return Vector3.Zero;
	}

	public Color AxisToColor( Axis axis )
	{
		var color = Color.Black;

		switch ( axis )
		{
			case Axis.X: color = new Vector3( 1.0f, 0.0f, 0.0f ); break;
			case Axis.Y: color = new Vector3( 0.0f, 1.0f, 0.0f ); break;
			case Axis.Z: color = new Vector3( 0.0f, 0.0f, 1.0f ); break;
			case Axis.Camera: color = new Vector3( 1.0f, 1.0f, 1.0f ); break;
		}

		if ( Local )
		{
			if ( axis == Axis.Z )
				color = new Vector3( 0.5f, 0.0f, 1.0f );
			
			color = color.Desaturate( 0.1f );
		}

		return color;
	}

	public Vector2 SnapToPlaneLocal( Axis axis, Vector3 point )
	{
		point -= Selection.Position;

		Vector3 planeNormal = AxisToVector( axis );
		var plane = new Plane( Vector3.Zero, planeNormal );

		var rot = axis == Axis.Camera ?
			Session.MainCamera.Rotation : GetSelectionTransform().Rotation;
		point = rot.Inverse * point;

		point = plane.SnapToPlane( point );

		switch ( axis )
		{
			case Axis.Camera:
			case Axis.X: return new Vector2( -point.z,  point.y );
			case Axis.Y: return new Vector2( -point.z, -point.x );
			case Axis.Z: return new Vector2(  point.x,  point.y );
		}

		return Vector2.Zero;
	}

	public Vector2 TraceToPlaneLocal( Axis axis, Ray ray )
	{
		Vector3 planeNormal = AxisToVector( axis );
		var plane = new Plane( Selection.Position, planeNormal );

		var point = plane.Trace( ray, twosided: true ).GetValueOrDefault();
		point -= Selection.Position;

		var rot = axis == Axis.Camera ?
			Session.MainCamera.Rotation : GetSelectionTransform().Rotation;
		point = rot.Inverse * point;

		switch ( axis )
		{
			case Axis.Camera:
			case Axis.X: return new Vector2( -point.z,  point.y );
			case Axis.Y: return new Vector2( -point.z, -point.x );
			case Axis.Z: return new Vector2(  point.x,  point.y );
		}

		return Vector2.Zero;
	}

	public Transform GetDragTransform()
	{
		if ( IsDragging )
			return DragStartTransform;

		return GetSelectionTransform();
	}

	public Transform GetSelectionTransform()
	{
		if ( Local )
		{
			return new Transform( Selection.Position, Selection.SelectedEntities.First().Transform.Rotation );
		}
		else
		{
			return new Transform( Selection.Position, Rotation.Identity );
		}
	}

	public abstract class SubGizmo : IDisposable
	{
		protected static Material Override = Material.Load( "materials/mnp_override.vmat" );

		protected const float GizmoSize = 16.0f;
		protected const float FinalScale = 0.15f;
		protected const float RenderSize = (1.0f / GizmoSize) * FinalScale;
		protected const float Gap = 0.2f * FinalScale;
		protected const float Width = 0.2f * FinalScale;
		protected const float Length = 1.0f * FinalScale;

		protected readonly Gizmo Parent;
		protected readonly Axis Axis;

		protected BBox bbox;
		protected Plane plane;

		protected SceneModel sceneModel;

		public SubGizmo( Gizmo parent, Axis axis, string modelName )
		{
			Parent = parent;
			Axis = axis;

			sceneModel = new SceneModel( parent.Session.MainWorld, modelName, Transform.Zero );
			sceneModel.RenderingEnabled = false;
		}

		public void Dispose()
		{
			sceneModel.Delete();
		}

		public abstract void Update();
		public abstract void UpdateDrag( Ray ray );
		public abstract void Render( Session session );
		public abstract bool Intersects( Ray ray );

		public virtual void StartDrag()
		{
			var origin = Parent.Selection.Position;
			plane = GetAppropriatePlaneForAxis( origin );
		}

		public abstract Plane GetAppropriatePlaneForAxis( Vector3 origin );
	}
}
