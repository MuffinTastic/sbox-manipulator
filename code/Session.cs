using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.Internal;
using Tools;

namespace Manipulator;

public partial class Session : RenderHook, IDisposable, IValid
{
	public const float FOV = 90.0f;

	public Widget Widget { get; init; }
	
	public SceneWorld MainWorld { get; init; }
	public SceneCamera MainCamera { get; private set; }

	public SceneWorld OverlayWorld { get; private set; }
	public SceneCamera OverlayCamera { get; private set; }

	private List<SceneModel> testmodels = new();

	public bool IsValid => Global.InGame;

	public Session( Widget parent, SceneWorld sceneWorld )
	{
		Widget = parent;
		MainWorld = sceneWorld;


		MainCamera = new SceneCamera( "Manipulator Main Camera" );
		MainCamera.World = MainWorld;

		MainCamera.ZFar = 10000;
		MainCamera.AntiAliasing = true;
		MainCamera.EnablePostProcessing = true;
		MainCamera.BackgroundColor = Color.Black;


		OverlayWorld = new SceneWorld();

		OverlayCamera = new SceneCamera( "Manipulator Overlay Camera" );
		OverlayCamera.World = OverlayWorld;

		OverlayCamera.ZFar = 10000;
		OverlayCamera.AntiAliasing = true;
		OverlayCamera.EnablePostProcessing = true;
		OverlayCamera.BackgroundColor = Color.Transparent;

		MainCamera.AddHook( this );

		Gizmo = new PositionGizmo( this, Selection );

		OnResize();
	}

	public void Dispose()
	{
		MainCamera?.RemoveAllHooks();
		MainCamera = null;

		OverlayWorld?.Delete();
		OverlayWorld = null;

		OverlayCamera?.RemoveAllHooks();
		OverlayCamera = null;

		foreach ( var model in testmodels )
		{
			model.Delete();
		}
	}

	public void OnResize()
	{
		var Aspect = Widget.ContentRect.Size.x / Widget.ContentRect.Size.y;
		var fov = MathF.Atan( MathF.Tan( FOV.DegreeToRadian() * 0.5f ) * (Aspect * 0.75f) ).RadianToDegree() * 2.0f;

		MainCamera.FieldOfView = fov;
		OverlayCamera.FieldOfView = fov;
	}

	public void Update()
	{
		if ( !IsValid )
			return;

		UpdateCamera();
		UpdateEdit();
	}

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


		if ( Selection.SelectedEntities.FirstOrDefault() is IEntity entitty )
		{
			var pos = Project( entitty.Transform.Position );

			if ( pos is not null )
			{
				Paint.SetBrushEmpty();
				Paint.SetPen( Color.Cyan );
				Paint.SetBrush( Color.Cyan );
				Paint.DrawCircle( pos.Value, 10.0f );
			}
		}

		var rect = new Rect( new Vector2( 10, 10 ), new Vector2( 175, 35 ) );

		Paint.SetBrush( new Color( 0.1f, 0.1f, 0.1f, 0.5f ) );
		Paint.SetPenEmpty();
		Paint.DrawRect( rect, 10.0f );

		Paint.SetPen( Color.Cyan.Saturate( 1.0f ).WithAlpha( 1.0f ) );
		Paint.SetFont( "Consolas", 12 );
		Paint.DrawText( rect.Shrink( 8.0f ), "my cool overlay", flags: TextFlag.LeftTop );
	}
}
