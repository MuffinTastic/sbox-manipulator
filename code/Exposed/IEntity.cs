using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Internal;
using Sandbox;

public static class IEntityExtensions
{
	public static SceneObject TryGetSceneObject( this IEntity ent )
	{
		if ( ent is null )
			return null;

		var type = ent.GetType();
		var sceneObjectProp = type.GetProperty( "SceneObject", BindingFlags.Instance | BindingFlags.Public );

		if ( sceneObjectProp is null )
			return null;

		var sceneObject = sceneObjectProp.GetValue( ent );
		return (SceneObject) sceneObject;
	}

	public static bool IsWorld( this IEntity ent )
	{
		if ( ent is null )
			return false;

		var className = ent.GetType().GetTypeInfo().Name;

		return className == "WorldEntity";
	}

	public static void TryResetInterpolation( this IEntity ent )
	{
		if ( ent is null )
			return;

		var type = ent.GetType();
		var resetMethod = type.GetMethod( "ResetInterpolation", BindingFlags.Instance | BindingFlags.Public );

		if ( resetMethod is null )
			return;

		try
		{
			resetMethod.Invoke( ent, null );
		}
		catch ( NullReferenceException nre )
		{
			Log.Error( nre );
		}
	}

	public static bool? TryGetPhysicsEnabled( this IEntity ent )
	{
		if ( ent is null )
			return null;

		var type = ent.GetType();
		var physicsEnabledProp = type.GetProperty( "PhysicsEnabled", BindingFlags.Instance | BindingFlags.Public );

		if ( physicsEnabledProp is null )
			return null;

		Log.Info( physicsEnabledProp );

		var physicsEnabled = physicsEnabledProp.GetValue( ent );
		return (bool) physicsEnabled;
	}

	public static void SetPhysicsEnabled( this IEntity ent, bool enabled )
	{
		if ( ent is null )
			return;

		var type = ent.GetType();
		var physicsEnabledProp = type.GetProperty( "PhysicsEnabled", BindingFlags.Instance | BindingFlags.Public );

		if ( physicsEnabledProp is null )
			return;

		try
		{
			physicsEnabledProp.SetValue( ent, enabled );
		}
		catch ( NullReferenceException nre )
		{
			Log.Error( nre );
		}
	}

	private static Dictionary<IEntity, IEntity> ServerEntities = new();

	public static IEntity GetServerEntity( this IEntity entity )
	{
		if ( ServerEntities.TryGetValue( entity, out IEntity serverEntity ) )
		{
			return serverEntity;
		}
		else
		{
			serverEntity = EntityEntry.All.Where( e => e.Client == entity || e.Server == entity ).FirstOrDefault()?.Server;
			ServerEntities.Add( entity, serverEntity );
			return serverEntity;
		}
	}
}
