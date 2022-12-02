﻿using System;
using System.Collections.Generic;
using Sandbox;
using Tools;
using Manipulator.Gizmo;
using System.Linq;

namespace Manipulator;

public partial class Session : RenderHook, IDisposable, IValid
{
	public const float FOV = 90.0f;

	public ManipulatorWidget ParentWidget { get; init; }
	
	public SceneWorld SceneWorld { get; init; }
	public SceneCamera Camera { get; private set; }

	private List<SceneModel> testmodels = new();

	public bool IsValid => Global.InGame;

	public Session( ManipulatorWidget parent, SceneWorld sceneWorld )
	{
		ParentWidget = parent;
		SceneWorld = sceneWorld;

		Camera = new SceneCamera( "Manipulator Main Camera" );
		Camera.World = SceneWorld;

		Camera.ZFar = 10000;
		Camera.AntiAliasing = true;
		Camera.EnablePostProcessing = true;
		Camera.BackgroundColor = Color.Black;

		// Session
		Camera.AddHook( this );

		OnResize();
	}

	// Anything that could be listened to by UI should be done here
	// Bit of a hack but it works
	public void PostUISetup()
	{
		SetGizmo( GizmoUIAttribute.All.First() );
	}

	public void Dispose()
	{
		Camera?.RemoveAllHooks();
		Camera = null;
		OnGizmoUpdated = null;

		foreach ( var model in testmodels )
		{
			model.Delete();
		}
	}

	public void OnResize()
	{
		var Aspect = ParentWidget.ContentRect.Size.x / ParentWidget.ContentRect.Size.y;
		var fov = MathF.Atan( MathF.Tan( FOV.DegreeToRadian() * 0.5f ) * (Aspect * 0.75f) ).RadianToDegree() * 2.0f;

		Camera.FieldOfView = fov;
	}

	public void Update()
	{
		if ( !IsValid )
			return;

		UpdateCamera();
		UpdateEdit();
	}
}
