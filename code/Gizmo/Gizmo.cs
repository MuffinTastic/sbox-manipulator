using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Tools;

namespace Manipulator.Gizmo;

public abstract class Gizmo : IDisposable
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
	private bool disposedValue;

	public Transform DragStartTransform { get; private set; }
	public SubGizmo Dragged { get; private set; }
	public bool Local { get; private set; } = false;

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

		if ( IsDragging )
		{
			DragStartTransform = GetSelectionTransform();

			Dragged.StartDrag();
		}

		return IsDragging;
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

		if ( IsDragging )
		{
			DragStartTransform = GetSelectionTransform();
		}
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

	public Vector3 SnapToPlaneLocal( Axis axis, Vector3 point )
	{
		var transform = GetSelectionTransform();
		point = transform.PointToLocal( point );

		Vector3 planeNormal = AxisToVector( axis );
		var plane = new Plane( Vector3.Zero, planeNormal );

		return plane.SnapToPlane( point );
	}

	public Vector2 TraceToPlaneLocal( Axis axis, Ray ray )
	{
		var transform = GetSelectionTransform();

		Vector3 planeNormal = AxisToVector( axis );
		var plane = new Plane( transform.Position, planeNormal );

		var point = plane.Trace( ray, twosided: true ).GetValueOrDefault();
		point -= transform.Position;

		var rot = axis == Axis.Camera ?
			Session.MainCamera.Rotation : transform.Rotation;
		point = rot.Inverse * point;

		switch ( axis )
		{
			case Axis.Camera:
			case Axis.X: return new Vector2( -point.z, point.y );
			case Axis.Y: return new Vector2( -point.z, -point.x );
			case Axis.Z: return new Vector2( point.x, point.y );
		}

		return Vector2.Zero;
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

	protected virtual void Dispose( bool disposing )
	{
		if ( !disposedValue )
		{
			if ( disposing )
			{
				foreach ( var gizmo in SubGizmos )
				{
					gizmo.Dispose();
				}
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			disposedValue = true;
		}
	}

	// TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
	~Gizmo()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose( disposing: false );
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose( disposing: true );
		GC.SuppressFinalize( this );
	}
}
