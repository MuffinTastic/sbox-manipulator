using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Editor;
using Sandbox.Internal;

namespace Manipulator;

/// 
/// This class lets us move the entire selection as one big group
/// 
public class Selection : IValid
{
	private Session Session;

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

	private List<ValueTuple<IEntity, Transform>> LocalTransforms = new();

	private struct PreDragState
	{
		public bool? PhysicsEnabled;
	}
	
	private List<ValueTuple<IEntity, PreDragState>> PreDragStates = new();

	public Selection( Session session )
	{
		Session = session;
	}

	public void Add( IEntity entity )
	{
		if ( entity.IsWorld() )
			return;

		if ( !SelectedEntities.Contains( entity ) )
		{
			SelectedEntities.Add( entity );

			RebuildTransforms( resetPivot: true );

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
			
			RebuildTransforms( resetPivot: true );

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
		PreDragStates.Clear();

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
			RebuildTransforms( resetPivot: true );

		PreDragStates.RemoveAll( pair => !pair.Item1.IsValid() );
	}

	private Vector3 _pivotOffsetInit = 0.0f;
	private Vector3 _pivotOffset = 0.0f;

	public void RebuildTransforms( bool resetPivot = false )
	{
		LocalTransforms.Clear();

		if ( !SelectedEntities.Any() )
		{
			return;
		}

		var resetCenter = Vector3.Zero;

		foreach ( var entity in SelectedEntities )
		{
			resetCenter += entity.Transform.Position;
		}

		resetCenter /= SelectedEntities.Count;
		
		if ( resetPivot )
		{
			_position = resetCenter;
		}
		else if ( !Session.PivotManipulation )
		{
			_position = resetCenter + _pivotOffset;
		}

		_rotation = Rotation.Identity;
		_scale = 1.0f;

		_pivotOffsetInit = _pivotOffset = _position - resetCenter;

		var selectionTransform = new Transform( _position, _rotation, _scale );

		foreach ( var entity in SelectedEntities )
		{
			LocalTransforms.Add( ( entity.GetServerEntity(), selectionTransform.ToLocal( entity.Transform ).WithScale( entity.Transform.Scale ) ) );
		}
	}

	private void OnTransformChanged()
	{
		if ( Session.PivotManipulation )
		{
			RebuildTransforms();
		}
		else
		{
			UpdateSelected();
		}
	}

	public void UpdateSelected()
	{
		var selectionTransform = new Transform( _position, _rotation, _scale );

		foreach ( var pair in LocalTransforms )
		{
			var entity = pair.Item1;
			var localTransform = pair.Item2;
			localTransform.Position *= selectionTransform.Scale;
			
			var worldTransform = selectionTransform.ToWorld( localTransform ).WithScale( localTransform.Scale * selectionTransform.Scale );
			entity.Position = worldTransform.Position;
			entity.Rotation = worldTransform.Rotation;
			entity.Velocity = 0.0f;
			entity.TryResetInterpolation();
		}

		_pivotOffset = _rotation * (_pivotOffsetInit * _scale);
	}

	public void StartDrag()
	{
		PreDragStates.Clear();

		foreach ( var entity in SelectedEntities )
		{
			var serverEntity = entity.GetServerEntity();
			var isPawn = serverEntity.Client?.Pawn == serverEntity;

			bool? physicsEnabled = null;

			// setting physics on a pawn fucks everything up, at least in the sandbox gamemode
			// let's avoid that
			if ( !isPawn && serverEntity.TryGetPhysicsEnabled() is bool enabled )
			{
				physicsEnabled = enabled;
				serverEntity.SetPhysicsEnabled( false );
			}

			var state = new PreDragState
			{
				PhysicsEnabled = physicsEnabled
			};

			PreDragStates.Add( (serverEntity, state) );
		}
	}

	public void StopDrag()
	{
		foreach ( var pair in PreDragStates )
		{
			var serverEntity = pair.Item1;
			var state = pair.Item2;

			if ( state.PhysicsEnabled is bool physicsEnabled )
			{
				serverEntity.SetPhysicsEnabled( physicsEnabled );
			}
		}
	}
}
