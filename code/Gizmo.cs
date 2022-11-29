using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace Manipulator;

public abstract class Gizmo : IValid
{
	public const float MaxSize = 32.0f;

	public enum Axis
	{
		X, Y, Z
	}

	public Transform Transform { get; set; }

	public bool IsValid => Selection.IsValid();

	public Session Session { get; init; }
	public Selection Selection { get; init; }

	public Gizmo( Session session, Selection selection )
	{
		Session = session;
		Selection = selection;
	}

	public abstract void StartDrag( Ray ray );
	public abstract void UpdateDrag( Ray ray );
	public abstract void StopDrag( Ray ray );

	public abstract void Update();
	public abstract void Render();

	public abstract Plane GetAppropriatePlaneForAxis( Vector3 origin, Axis axis );

	public Plane GetCameraAlignedPlane( Vector3 origin )
	{
		return new Plane( origin, Session.MainCamera.Rotation.Forward );
	}

	protected static Vector3 AxisToVector( Axis axis )
	{
		switch ( axis )
		{
			default:
			case Axis.X: return new Vector3( 1.0f, 0.0f, 0.0f );
			case Axis.Y: return new Vector3( 0.0f, 1.0f, 0.0f );
			case Axis.Z: return new Vector3( 0.0f, 0.0f, 1.0f );
		}
	}

	protected static Color AxisToColor( Axis axis )
	{
		switch ( axis )
		{
			default:
			case Axis.X: return new Vector3( 1.0f, 0.0f, 0.0f );
			case Axis.Y: return new Vector3( 0.0f, 1.0f, 0.0f );
			case Axis.Z: return new Vector3( 0.0f, 0.0f, 1.0f );
		}
	}
}
