using System;
using System.Threading.Tasks;
using Exposed;
using Manipulator.Gizmo;
using Sandbox;
using Editor;

namespace Manipulator;

public partial class Session
{
	public IEntity HoverEntity { get; private set; }

	public bool LocalManipulation { get; private set; } = false;
	public bool PivotManipulation { get; private set; } = false;

	public Selection Selection;

	public Gizmo.Gizmo Gizmo { get; private set; }

	public Outlines HoverOutlines = new( Color.Blue, Color.Cyan );
	public Outlines SelectionOutlines = new( Color.Red, Color.Yellow );

	private void EditMouseClick( MouseEvent e )
	{
		if ( !ShouldInteract() )
			return;

		if ( e.LeftMouseButton )
		{
			if ( Gizmo.IsDragging )
			{
				return;
			}

			var tr = RunTrace( GetCursorRay() );
			if ( tr.Hit )
			{
				// SpawnDebugArrow( tr );

				if ( e.HasShift )
				{
					Selection.Add( tr.Entity );
				}
				else if ( e.HasCtrl )
				{
					Selection.Remove( tr.Entity );
				}
				else
				{
					Selection.Set( tr.Entity );
				}

				Selection.SetEditorInspector();

				SetPivotManipulation( false );
			}
		}
	}

	private bool StartEditDrag()
	{
		if ( !ShouldInteract() )
			return false;

		return Gizmo.StartDrag( GetCursorRay() );
	}

	private void EditDragMove()
	{
		if ( !ShouldInteract() )
			return;

		Gizmo.UpdateDrag( GetCursorRay() );
	}

	private void StopEditDrag()
	{
		if ( !ShouldInteract() )
			return;

		Gizmo.StopDrag( GetCursorRay() );
	}

	private void UpdateEdit()
	{
		HoverEntity = null;

		Selection.CullInvalid();

		var ray = GetCursorRay();
		if ( ShouldInteract() && !Gizmo.IsHovering( ray ) )
		{
			var tr = RunTrace( ray );

			if ( tr.Hit )
			{
				HoverEntity = tr.Entity;
			}
		}

		HoverOutlines.UpdateFrom( HoverEntity );
		SelectionOutlines.UpdateFrom( Selection.SelectedEntities );

		Gizmo.Update();
	}

	private void EditHandleKeyPress( KeyEvent e )
	{
		if ( e.Key == ParentWidget.Binds.ToggleLocalManipulation )
		{
			SetLocalManipulation( !LocalManipulation );
			Gizmo.ResetDragStartTransform();
			Gizmo.UpdateDrag( GetCursorRay() );
		}

		if ( e.Key == ParentWidget.Binds.TogglePivotManipulation )
		{
			SetPivotManipulation( !PivotManipulation );
		}

		if ( e.Key == ParentWidget.Binds.SelectTranslationGizmo )
		{
			SetGizmo( GizmoUIAttribute.For( typeof( TranslationGizmo ) ) );
		}

		if ( e.Key == ParentWidget.Binds.SelectRotationGizmo )
		{
			SetGizmo( GizmoUIAttribute.For( typeof( RotationGizmo ) ) );
		}

		if ( e.Key == ParentWidget.Binds.SelectScaleGizmo )
		{
			SetGizmo( GizmoUIAttribute.For( typeof( ScaleGizmo ) ) );
		}
	}

	private GameTraceResult RunTrace( Ray ray )
	{
		var tr = GameTrace.Ray( ray.Position, ray.Position + ray.Forward * 1000.0f, client: true )
			.WorldAndEntities()
			.Run();
		return tr;
	}

	public event Action<int> OnGizmoUpdated;
	public event Action<bool> OnLocalManipulationUpdated;
	public event Action<bool> OnPivotManipulationUpdated;

	public void SetGizmo( GizmoUIAttribute attribute, bool exitPivotManipulation = true )
	{
		Gizmo = attribute.CreateGizmo( this, Selection );
		OnGizmoUpdated?.Invoke( attribute.Index );

		if ( exitPivotManipulation )
			SetPivotManipulation( false );
	}

	public void SetLocalManipulation( bool on )
	{
		LocalManipulation = on;
		OnLocalManipulationUpdated?.Invoke( on );
	}

	public void SetPivotManipulation( bool on )
	{
		PivotManipulation = on;
		OnPivotManipulationUpdated?.Invoke( on );

		if ( PivotManipulation )
		{
			SetGizmo( GizmoUIAttribute.For( typeof( TranslationGizmo ) ), exitPivotManipulation: false );
		}
	}

	public void ResetUIListeners()
	{
		OnGizmoUpdated = null;
		OnLocalManipulationUpdated = null;
	}

	private void SpawnDebugArrow( GameTraceResult tr )
	{
		var obj = new SceneModel( SceneWorld, Model.Load( "models/arrow.vmdl" ), Transform.Zero );
		obj.Position = tr.HitPosition;

		var up = Vector3.Up;

		// We have to use a different "up" to calculate the bitangent if we're
		// on the ceiling or floor.
		// Use the direction of the trace, it goes straight back to the player

		if ( tr.Normal.AlmostEqual( Vector3.Up ) )
			up = tr.Direction;
		else if ( tr.Normal.AlmostEqual( Vector3.Down ) )
			up = -tr.Direction;

		var tangent = up.Cross( tr.Normal ).Normal;
		var bitangent = tr.Normal.Cross( tangent ).Normal;

		obj.Rotation = Rotation.LookAt( bitangent, tr.Normal );

		async void DeleteLater( SceneModel obj )
		{
			await Task.Delay( 1000 );
			obj.Delete();
			testmodels.Remove( obj );
		}

		DeleteLater( obj );
		testmodels.Add( obj );
	}
}
