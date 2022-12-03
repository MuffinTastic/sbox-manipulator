using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

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

		Dragged = GetHovered( ray );

		if ( IsDragging )
		{
			DragStartTransform = GetSelectionTransform();

			Dragged.StartDrag( ray );
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

	public void ResetDragStartTransform()
	{
		if ( IsDragging )
		{
			DragStartTransform = GetSelectionTransform();
		}
	}

	public bool IsHovering( Ray ray )
	{
		if ( !Session.ShouldInteract() )
			return false;

		if ( !Selection.IsValid() )
			return false;

		return SubGizmos.Any( g => g.Intersects( ray, out var _ ) );
	}

	public bool IsHovering( Ray ray, SubGizmo subGizmo )
	{
		if ( !Session.ShouldInteract() )
			return false;

		if ( !Selection.IsValid() )
			return false;

		var hovered = GetHovered( ray );
		return subGizmo == hovered;
	}

	private SubGizmo GetHovered( Ray ray )
	{
		var gizmos = new List<(SubGizmo, float)>();

		foreach ( var gizmo in SubGizmos )
		{
			if ( gizmo.Intersects( ray, out var distance ) )
			{
				gizmos.Add( ( gizmo, distance ) );
			}
		}

		return gizmos
			.OrderBy( t => t.Item2 )
			.Select( t => t.Item1 )
			.FirstOrDefault();
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

	public abstract void Render();

	public void UpdateSelectionTransform( Vector3 newPosition, Rotation newRotation, float newScale )
	{
		if ( !Selection.IsValid() )
			return;

		Selection.Position = newPosition;

		if ( Session.LocalManipulation )
		{
			var transform = GetDragTransform();
			newRotation = newRotation * transform.Rotation.Inverse;
		}

		Selection.Rotation = newRotation;

		Selection.Scale = newScale;
	}

	public float GetCameraAdjustedScale()
	{
		var pos = Selection.Position;
		var transform = new Transform( Session.Camera.Position, Session.Camera.Rotation );
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
			Session.Camera.Rotation : transform.Rotation;
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
			case Axis.Camera: return Session.Camera.Rotation.Forward;
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
		if ( Session.LocalManipulation )
		{
			return new Transform( Selection.Position, Selection.SelectedEntities.First().Transform.Rotation, Selection.Scale );
		}
		else
		{
			return new Transform( Selection.Position, Rotation.Identity, Selection.Scale );
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
