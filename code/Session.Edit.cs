using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	public Gizmo Gizmo;

	public Outlines HoverOutlines = new( Color.Blue, Color.Cyan );
	public Outlines SelectionOutlines = new( Color.Red, Color.Yellow );

	private void EditMouseClick( MouseEvent e )
	{
		var tr = RunMouseTrace();
		if ( tr.Hit )
		{
			var obj = new SceneModel( OverlayWorld, Model.Load( "models/arrow.vmdl" ), Transform.Zero );
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

	private void StartEditDrag( Vector2 position )
	{
		//Gizmo.StartDrag( )
	}

	private void EditDragMove( Vector2 position, Vector2 delta )
	{

	}

	private void StopEditDrag( Vector2 position )
	{

	}

	private void UpdateEdit()
	{
		var tr = RunMouseTrace();

		HoverEntity = null;

		if ( tr.Hit && !cameraRotating )
		{
			HoverEntity = tr.Entity;
		}

		HoverOutlines.UpdateFrom( HoverEntity );
		SelectionOutlines.UpdateFrom( Selection.SelectedEntities );

		Gizmo.Update();
	}

	private GameTraceResult RunMouseTrace()
	{
		var ray = GetCursorRay();
		var tr = GameTrace.Ray( ray.Origin, ray.Origin + ray.Direction * 1000.0f, client: true )
			.WorldAndEntities()
			.Run();
		return tr;
	}
}
