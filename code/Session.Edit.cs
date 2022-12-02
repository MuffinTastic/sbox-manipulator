using System.Threading.Tasks;
using Exposed;
using Sandbox;
using Sandbox.Internal;
using Tools;

namespace Manipulator;

public partial class Session
{
	public IEntity HoverEntity { get; private set; }

	public Selection Selection = new();

	public Gizmo.Gizmo Gizmo;

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
		if ( e.Key == KeyCode.R )
		{
			Gizmo.ToggleLocal();

			Gizmo.UpdateDrag( GetCursorRay() );
		}
	}

	private GameTraceResult RunTrace( Ray ray )
	{
		var tr = GameTrace.Ray( ray.Origin, ray.Origin + ray.Direction * 1000.0f, client: true )
			.WorldAndEntities()
			.Run();
		return tr;
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
