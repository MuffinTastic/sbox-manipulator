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

		// Session
		MainCamera.AddHook( this );

		Gizmo = new PositionGizmo( this, Selection );

		OnResize();
	}

	public void Dispose()
	{
		MainCamera?.RemoveAllHooks();
		MainCamera = null;

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
	}

	public void Update()
	{
		if ( !IsValid )
			return;

		UpdateCamera();
		UpdateEdit();
	}
}
