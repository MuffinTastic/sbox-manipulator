using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;
using Tools;

namespace Manipulator;

[Dock( "Editor", "Manipulator", "pinch" )]
public class ManipulatorWidget : Widget
{
	private bool connecting = false;

	// use Session.IsValid()
	public Session Session { get; private set; }

	Pixmap renderTarget;
	bool renderTargetDirty = false;

	public ManipulatorWidget( Widget parent ) : base( parent )
	{
		MinimumSize = 300;
		MouseTracking = true;
		FocusMode = FocusMode.Click;
	}

	[Event.Frame]
	protected void Frame()
	{
		// Edge checks
		if ( Session is null && Global.InGame )
		{
			Activate();
		}
		else if ( Session is not null & !Global.InGame )
		{
			Deactivate();
		}

		if ( Session.IsValid() )
		{
			try
			{
				Session.Update();
			}
			catch ( Exception e )
			{
				Log.Error( e );
			}

			base.Update();
		}
	}

	protected override void OnPaint()
	{
		base.OnPaint();

		if ( !Session.IsValid() )
		{
			DrawDisabledMessage();
			return;
		}

		SetRenderTarget();

		Session.MainCamera.RenderToPixmap( renderTarget );
		Paint.Draw( new Rect( Vector2.Zero, renderTarget.Size ), renderTarget );

		//Session.OverlayCamera.RenderToPixmap( renderTarget );
		//Paint.Draw( new Rect( Vector2.Zero, renderTarget.Size ), renderTarget );

		Session.OnPaint();
	}

	private void DrawDisabledMessage()
	{
		Paint.SetPenEmpty();

		Paint.SetBrush( Theme.WindowBackground );
		Paint.DrawRect( ContentRect );

		var size = new Vector2( 260, 115 );
		var r = new Rect( Size / 2 - size / 2, size );
		Paint.SetBrush( Theme.WidgetBackground );
		Paint.DrawRect( r, 10.0f );

		Paint.SetPen( Theme.ControlText );
		Paint.SetDefaultFont( size: 12.0f, weight: 400 );

		var text = connecting ?
			"Game started\nConnecting..." :
			"No game is running\nManipulator disabled";
		Paint.DrawText( r.Shrink( 5.0f ), text );
	}

	private void Activate()
	{
		CreateSession();
	}

	private void Deactivate()
	{
		Session.Dispose();
		Session = null;

		// Force an update so it shows the "disabled" message immediately
		Update();
	}

	private async void CreateSession()
	{
		connecting = true;
		SceneWorld mainSceneWorld;

		do
		{
			if ( !Global.InGame )
				return;

			mainSceneWorld = Exposed.SceneWorld.GetMainSceneWorld();

			await Task.Delay( 100 );
		}
		while ( mainSceneWorld is null );

		SetRenderTarget( force: true );
		Session = new Session( this, mainSceneWorld );

		connecting = false;
	}

	protected override void OnResize()
	{
		renderTargetDirty = true;
		Session?.OnResize();
	}

	private void SetRenderTarget( bool force = false )
	{
		if ( renderTargetDirty || force )
		{
			renderTarget = new Pixmap( (int) Size.x, (int) Size.y );
			renderTargetDirty = false;
		}
	}

	#region Input events

	protected override void OnMousePress( MouseEvent e )
	{
		base.OnMousePress( e );

		Session?.OnMousePress( e );
	}

	protected override void OnMouseReleased( MouseEvent e )
	{
		base.OnMouseReleased( e );

		Session?.OnMouseReleased( e );
	}

	protected override void OnMouseMove( MouseEvent e )
	{
		base.OnMouseMove( e );

		Session?.OnMouseMove( e );
	}

	protected override void OnKeyPress( KeyEvent e )
	{
		base.OnKeyPress( e );

		Session?.OnKeyPress( e );
	}

	protected override void OnKeyRelease( KeyEvent e )
	{
		base.OnKeyRelease( e );

		Session?.OnKeyRelease( e );
	}

	protected override void OnWheel( WheelEvent e )
	{
		base.OnWheel( e );

		Session?.OnWheel( e );
	}

	#endregion
}
