using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sandbox.Internal;

namespace Exposed
{
	public static class SceneWorld
	{
		public static HashSet<Sandbox.SceneWorld> All
		{
			get
			{
				var allField = typeof( Sandbox.SceneWorld )
					.GetField( "All", BindingFlags.Static | BindingFlags.NonPublic );

				return (HashSet<Sandbox.SceneWorld>) allField.GetValue( null );
			}
		}

		internal static Sandbox.SceneWorld GetMainSceneWorld()
		{
			// Valid internal sceneworlds
			var all = All.Where( w => !w.ExposedIsTransient() && w.ExposedHandleValid() ).ToList();

			// A valid sceneobject pointer from the game sceneworld
			var testPointer = EntityEntry.All.Select( e => e.Client.ExposedGetSceneObject().ExposedGetPointer() ).Where( e => e is not null ).FirstOrDefault();

			if ( testPointer is not null )
			{
				foreach ( var sceneWorld in all )
				{
					var pointers = sceneWorld.SceneObjects.Select( o => o.ExposedGetPointer() );
					if ( pointers.Contains( testPointer ) )
						return sceneWorld;
				}
			}

			// Last ditch effort
			const string Magic = "worldnodes";
			const string Antimagic = "sky";

			return all.FirstOrDefault( w => w.SceneObjects
						.Where( o => o.Model is not null )
						.Select( o => o.Model.Name )
						.Any( n => n.Contains( Magic ) && !n.Contains( Antimagic ) )
					);
		}
	}
}

public static class SceneWorldExtensions
{
	public static bool ExposedIsTransient( this Sandbox.SceneWorld world )
	{
		var transientProperty = typeof( Sandbox.SceneWorld )
			.GetProperty( "IsTransient", BindingFlags.Instance | BindingFlags.NonPublic );

		return (bool) transientProperty.GetValue( world );
	}

	public static bool ExposedHandleValid( this Sandbox.SceneWorld world )
	{
		var nativeField = typeof( Sandbox.SceneWorld )
			.GetField( "native", BindingFlags.Instance | BindingFlags.NonPublic );

		var handleField = nativeField.FieldType
			.GetField( "self", BindingFlags.Instance | BindingFlags.NonPublic );

		var nativeInstance = nativeField.GetValue( world );
		var handleInstance = (IntPtr) handleField.GetValue( nativeInstance );

		return handleInstance != IntPtr.Zero;
	}
}
