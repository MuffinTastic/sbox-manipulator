using Sandbox;
using Sandbox.Internal;
using Tools;

namespace Manipulator;
public partial class Session
{
	public override void OnFrame( SceneCamera _ )
	{
		foreach ( var ent in EntityEntry.All )
		{
			var so = ent.Client?.ExposedGetSceneObject();

			if ( so.IsValid() )
			{
				if ( so.Flags.ViewModelLayer )
				{
					so.RenderingEnabled = false;
				}
			}
		}
	}

	///
	/// For rendering things within the overlay scene 
	///
	public override void OnStage( SceneCamera _, Stage renderStage )
	{
		if ( renderStage == Stage.AfterOpaque )
		{
			foreach ( var sm in testmodels )
			{
				Graphics.Render( sm );
			}
		}

		if ( renderStage == Stage.AfterPostProcess )
		{
			if ( HoverOutlines.IsValid() )
			{
				HoverOutlines.Render();
			}

			if ( SelectionOutlines.IsValid() )
			{
				SelectionOutlines.Render();
			}

			//if ( Gizmo.IsValid() )
			{
				Gizmo.Render();
			}
		}
	}

	///
	/// For rendering UI on top of the framebuffer with Qt
	/// 
	public void OnPaint()
	{
		if ( pressIsClick )
		{
			Paint.SetBrushEmpty();
			Paint.SetPen( Color.Red );
			Paint.DrawCircle( pressStart, pressFuzz );

		}


		/*if ( Selection.SelectedEntities.FirstOrDefault() is IEntity entitty )
		{
			var pos = Project( entitty.Transform.Position );

			if ( pos is not null )
			{
				Paint.SetBrushEmpty();
				Paint.SetPen( Color.Cyan );
				Paint.SetBrush( Color.Cyan );
				Paint.DrawCircle( pos.Value, 10.0f );
			}
		}*/

		//var rect = new Rect( new Vector2( 10, 10 ), new Vector2( 175, 35 ) );

		//Paint.SetBrush( new Color( 0.1f, 0.1f, 0.1f, 0.5f ) );
		//Paint.SetPenEmpty();
		//Paint.DrawRect( rect, 10.0f );

		if ( false )
		{
			Paint.SetPen( Color.White );
			Paint.SetFont( "Consolas", 12 );
			var text = $"dragButton {dragButton},\n" +
						$"pressIsClick {pressIsClick},\n" +
						$"pressIsLeftClick {pressIsLeftClick},\n" +
						$"pressIsRightClick {pressIsRightClick}";

			Paint.DrawText( ParentWidget.ContentRect.Shrink( 8.0f ), text, flags: TextFlag.LeftTop );
		}
	}
}
