using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.Internal;
using Tools;

namespace Manipulator;

/// 
/// This class lets us move the entire selection as one big group
/// 
public class Selection : IValid
{
	private Vector3 _position;
	private Rotation _rotation;
	private float _scale;

	public Vector3 Position
	{
		get => _position;
		set
		{
			_position = value;
			OnTransformChanged();
		}
	}

	public Rotation Rotation
	{
		get => _rotation;
		set
		{
			_rotation = value;
			OnTransformChanged();
		}
	}

	public float Scale
	{
		get => _scale;
		set
		{
			_scale = value;
			OnTransformChanged();
		}
	}

	public bool IsValid => SelectedEntities.Any();

	public event Action OnUpdated;

	public HashSet<IEntity> SelectedEntities = new();
	public List<ValueTuple<IEntity, Transform>> LocalTransforms = new();

	public void Add( IEntity entity )
	{
		if ( entity.IsWorld() )
			return;

		if ( !SelectedEntities.Contains( entity ) )
		{
			SelectedEntities.Add( entity );

			RebuildTransforms();

			OnUpdated?.Invoke();
		}
	}

	public void Remove( IEntity entity )
	{
		if ( entity.IsWorld() )
			return;

		if ( SelectedEntities.Contains( entity ) )
		{
			SelectedEntities.Remove( entity );
			
			RebuildTransforms();

			OnUpdated?.Invoke();
		}
	}

	public void Set( IEntity entity )
	{
		Clear( suppress: true );

		Add( entity );

		OnUpdated?.Invoke();
	}

	public void Clear( bool suppress = false )
	{
		SelectedEntities.Clear();
		LocalTransforms.Clear();

		if ( !suppress )
			OnUpdated?.Invoke();
	}

	public void SetEditorInspector()
	{
		if ( !SelectedEntities.Any() )
		{
			Utility.InspectorObject = null;
			return;
		}

		if ( SelectedEntities.Count == 1 )
		{
			Utility.InspectorObject = SelectedEntities.First();
		}
		else
		{
			Utility.InspectorObject = this;
		}
	}

	public void CullInvalid()
	{
		var removed = SelectedEntities.RemoveWhere( entity => !entity.IsValid() );
		if ( removed != 0 )
			RebuildTransforms();
	}

	public void RebuildTransforms()
	{
		LocalTransforms.Clear();

		if ( !SelectedEntities.Any() )
		{
			return;
		}

		var averagePosition = Vector3.Zero;

		foreach ( var entity in SelectedEntities )
		{
			averagePosition += entity.Transform.Position;
		}

		averagePosition /= SelectedEntities.Count;
		
		_position = averagePosition;
		_rotation = Rotation.Identity;
		_scale = 1.0f;
		var selectionTransform = new Transform( _position, _rotation, _scale );

		foreach ( var entity in SelectedEntities )
		{
			LocalTransforms.Add( (entity.GetServerEntity(), selectionTransform.ToLocal( entity.Transform ).WithScale( entity.Transform.Scale ) ) );
		}
	}

	private void OnTransformChanged()
	{
		var selectionTransform = new Transform( _position, _rotation, _scale );

		foreach ( var pair in LocalTransforms )
		{
			var entity = pair.Item1;
			var localTransform = pair.Item2;
			localTransform.Position *= selectionTransform.Scale;
			
			var worldTransform = selectionTransform.ToWorld( localTransform ).WithScale( localTransform.Scale * selectionTransform.Scale );
			entity.SetTransform( worldTransform );
			entity.SetVelocity( 0.0f );
			entity.ResetInterpolation();
		}
	}
}
