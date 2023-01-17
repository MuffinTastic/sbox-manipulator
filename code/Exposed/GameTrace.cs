using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace Exposed;

public struct GameTrace
{
	private static readonly Type TraceType = Util.ClientTypes.Where( t => t.FullName == "Sandbox.Trace" ).FirstOrDefault();

	private Type type;
	private object trace;

	public static GameTrace Ray( Vector3 start, Vector3 end, bool client )
	{
		var types = client ? Util.ClientTypes : Util.ServerTypes;
		var newType = types.Where( t => t.FullName == "Sandbox.Trace" ).FirstOrDefault();
		var newTrace = TraceType.InvokeMember( "Ray", BindingFlags.InvokeMethod, Type.DefaultBinder, null, new object[] { start, end } );

		return new GameTrace
		{
			type = newType,
			trace = newTrace
		};
	}

	public readonly GameTrace WorldOnly()
	{
		var newTrace = type.InvokeMember( "WorldOnly", BindingFlags.InvokeMethod, Type.DefaultBinder, trace, new object[] { } );
		
		return new GameTrace
		{
			type = type,
			trace = newTrace
		};
	}

	public readonly GameTrace EntitiesOnly()
	{
		var newTrace = type.InvokeMember( "EntitiesOnly", BindingFlags.InvokeMethod, Type.DefaultBinder, trace, new object[] { } );

		return new GameTrace
		{
			type = type,
			trace = newTrace
		};
	}

	public readonly GameTrace WorldAndEntities()
	{
		var newTrace = type.InvokeMember( "WorldAndEntities", BindingFlags.InvokeMethod, Type.DefaultBinder, trace, new object[] { } );

		return new GameTrace
		{
			type = type,
			trace = newTrace
		};
	}

	public readonly GameTrace Radius( float radius )
	{
		var newTrace = type.InvokeMember( "Radius", BindingFlags.InvokeMethod, Type.DefaultBinder, trace, new object[] { radius } );

		return new GameTrace
		{
			type = type,
			trace = newTrace
		};
	}

	public readonly GameTrace Ignore( in IEntity ent, in bool hierarchy = true )
	{
		var newTrace = type.InvokeMember( "Ignore", BindingFlags.InvokeMethod, Type.DefaultBinder, trace, new object[] { ent, hierarchy } );

		return new GameTrace
		{
			type = type,
			trace = newTrace
		};
	}

	public readonly GameTrace WithTag( string tag )
	{
		var newTrace = type.InvokeMember( "WithTag", BindingFlags.InvokeMethod, Type.DefaultBinder, trace, new object[] { tag } );

		return new GameTrace
		{
			type = type,
			trace = newTrace
		};
	}

	public readonly GameTrace WithAllTags( params string[] tags )
	{
		var newTrace = type.InvokeMember( "WithAllTags", BindingFlags.InvokeMethod, Type.DefaultBinder, trace, new object[] { tags } );

		return new GameTrace
		{
			type = type,
			trace = newTrace
		};
	}

	public readonly GameTrace WithAnyTags( params string[] tags )
	{
		var newTrace = type.InvokeMember( "WithAnyTags", BindingFlags.InvokeMethod, Type.DefaultBinder, trace, new object[] { tags } );

		return new GameTrace
		{
			type = type,
			trace = newTrace
		};
	}

	public readonly GameTrace WithoutTags( params string[] tags )
	{
		var newTrace = type.InvokeMember( "WithoutTags", BindingFlags.InvokeMethod, Type.DefaultBinder, trace, new object[] { tags } );

		return new GameTrace
		{
			type = type,
			trace = newTrace
		};
	}

	public readonly GameTraceResult Run()
	{
		var result = type.InvokeMember( "Run", BindingFlags.InvokeMethod, Type.DefaultBinder, trace, new object[] { } );

		return new GameTraceResult( result );
	}
}
