using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manipulator;
using Sandbox;
using Sandbox.Internal;
using static Sandbox.Event;

namespace Exposed;

public struct GameTraceResult
{
	//
	// Summary:
	//     Whether the trace hit something or not
	public bool Hit;

	//
	// Summary:
	//     Whether the trace started in a solid
	public bool StartedSolid;

	//
	// Summary:
	//     The start position of the trace
	public Vector3 StartPosition;

	//
	// Summary:
	//     The end or hit position of the trace
	public Vector3 EndPosition;

	//
	// Summary:
	//     The hit position of the trace
	public Vector3 HitPosition;

	//
	// Summary:
	//     The hit surface normal (direction vector)
	public Vector3 Normal;

	//
	// Summary:
	//     A fraction [0..1] of where the trace hit between the start and the original end
	//     positions
	public float Fraction;

	//
	// Summary:
	//     The entity that was hit, if any
	public IEntity Entity;

	//
	// Summary:
	//     The physics object that was hit, if any
	public PhysicsBody Body;

	//
	// Summary:
	//     The physics shape that was hit, if any
	public PhysicsShape Shape;

	//
	// Summary:
	//     The physical properties of the hit surface
	public Surface Surface;

	//
	// Summary:
	//     The id of the hit bone (either from hitbox or physics shape)
	public int Bone;

	//
	// Summary:
	//     The direction of the trace ray
	public Vector3 Direction;

	//
	// Summary:
	//     The triangle index hit, if we hit a mesh physics shape
	public int Triangle;

	//
	// Summary:
	//     The tags that the hit shape had
	public string[] Tags;

	//
	// Summary:
	//     The hitbox hit, if at all. Requires Trace.UseHitboxes to work.
	public Hitbox Hitbox;

	// --- //

	private static readonly Type TraceResultType = Util.ClientTypes.Where( t => t.FullName == "Sandbox.TraceResult" ).FirstOrDefault();

	public GameTraceResult( object traceResult )
	{
		Hit = (bool) TraceResultType.GetField( "Hit" ).GetValue( traceResult );
		StartedSolid = (bool)TraceResultType.GetField( "StartedSolid" ).GetValue( traceResult );
		StartPosition = (Vector3)TraceResultType.GetField( "StartPosition" ).GetValue( traceResult );
		EndPosition = (Vector3)TraceResultType.GetField( "EndPosition" ).GetValue( traceResult );
		HitPosition = (Vector3)TraceResultType.GetField( "HitPosition" ).GetValue( traceResult );
		Normal = (Vector3)TraceResultType.GetField( "Normal" ).GetValue( traceResult );
		Fraction = (float)TraceResultType.GetField( "Fraction" ).GetValue( traceResult );
		Entity = (IEntity)TraceResultType.GetField( "Entity" ).GetValue( traceResult );
		Body = (PhysicsBody)TraceResultType.GetField( "Body" ).GetValue( traceResult );
		Shape = (PhysicsShape)TraceResultType.GetField( "Shape" ).GetValue( traceResult );
		Surface = (Surface)TraceResultType.GetField( "Surface" ).GetValue( traceResult );
		Bone = (int)TraceResultType.GetField( "Bone" ).GetValue( traceResult );
		Direction = (Vector3)TraceResultType.GetField( "Direction" ).GetValue( traceResult );
		Triangle = (int)TraceResultType.GetField( "Triangle" ).GetValue( traceResult );
		Tags = (string[])TraceResultType.GetField( "Tags" ).GetValue( traceResult );
		Hitbox = (Hitbox)TraceResultType.GetField( "Hitbox" ).GetValue( traceResult );
	}
}
