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
	public static SceneObject ExposedGetSceneObject( this IEntity ent )
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

	public static void SetTransform( this IEntity ent, Transform transform )
	{
		if ( ent is null )
			return;

		var type = ent.GetType();
		var transformProp = type.GetProperty( "Transform", BindingFlags.Instance | BindingFlags.Public );

		if ( transformProp is null )
			return;

		try
		{
			transformProp.SetValue( ent, transform );
		}
		catch ( NullReferenceException nre )
		{
			Log.Error( nre );
		}
	}

	public static void SetVelocity( this IEntity ent, Vector3 velocity )
	{
		if ( ent is null )
			return;

		var type = ent.GetType();
		var velocityProp = type.GetProperty( "Velocity", BindingFlags.Instance | BindingFlags.Public );

		if ( velocityProp is null )
			return;

		try
		{
			velocityProp.SetValue( ent, velocity );
		}
		catch ( NullReferenceException nre )
		{
			Log.Error( nre );
		}
	}

	public static void ResetInterpolation( this IEntity ent )
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

	public static bool IsValid( this IEntity ent )
	{
		if ( ent is null )
			return false;

		var type = ent.GetType();
		var isValidProperty = type.GetProperty( "IsValid", BindingFlags.Instance | BindingFlags.Public );

		if ( isValidProperty is null )
			return false;

		try
		{
			return (bool) isValidProperty.GetValue( ent, null );
		}
		catch ( NullReferenceException nre )
		{
			Log.Error( nre );
			return false;
		}
	}

	private static Dictionary<IEntity, IEntity> ClientToServer = new();

	public static IEntity GetServerEntity( this IEntity clientEntity )
	{
		if ( ClientToServer.TryGetValue( clientEntity, out IEntity serverEntity ) )
		{
			return serverEntity;
		}
		else
		{
			serverEntity = EntityEntry.All.Where( e => e.Client == clientEntity ).FirstOrDefault()?.Server;
			ClientToServer.Add( clientEntity, serverEntity );
			return serverEntity;
		}
	}
}
