using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Tools;

namespace Manipulator;

public partial class Session
{
	bool pressIsClick;
	bool pressIsLeftClick;
	bool pressIsRightClick;
	private const float pressFuzz = 5.0f;
	Vector2 pressStart;
	Vector2 lastMousePosition;

	public void OnMousePress( MouseEvent e )
	{
		if ( true /* todo: not interacting with gizmos */ )
		{
			pressStart = e.LocalPosition;
			pressIsClick = true; // assume it's a click
			pressIsLeftClick = e.LeftMouseButton;
			pressIsRightClick = e.RightMouseButton;
		}
	}

	public void OnMouseMove( MouseEvent e )
	{
		var delta = lastMousePosition - e.LocalPosition;

		if ( pressIsClick )
		{
			// we went out of range, it's not a click
			if ( e.LocalPosition.Distance( pressStart ) > pressFuzz )
			{
				pressIsClick = false;

				if ( pressIsLeftClick )
					StartEditDrag( e.LocalPosition );
				else if ( pressIsRightClick )
					StartCameraRotation();
			}

			// we don't want to do anything until we're sure it's not a click
			return;
		}
		else
		{
			if ( pressIsLeftClick )
			{
				EditDragMove( e.LocalPosition, delta );
			}
			else if ( pressIsRightClick && cameraRotating )
			{
				ApplyCameraMoveMouse( delta );
			}
		}

		lastMousePosition = e.LocalPosition;
	}

	public void OnMouseReleased( MouseEvent e )
	{
		if ( pressIsClick )
		{
			EditMouseClick( e );
		}
		else
		{
			if ( e.LeftMouseButton )
			{
				StopEditDrag( e.LocalPosition );
			}
			else if ( e.RightMouseButton )
			{
				if ( cameraRotating )
					StopCameraRotation();
			}
		}
	}

	public void OnWheel( WheelEvent e )
	{
		ApplyCameraScrollWheel( e );
	}

	public void OnKeyPress( KeyEvent e )
	{
		ApplyCameraKeyPress( e );
	}

	public void OnKeyRelease( KeyEvent e )
	{
		ApplyCameraKeyRelease( e );
	}

	public Ray GetCursorRay()
	{
		var direction = Screen.GetDirection( Application.CursorPosition - Widget.ScreenPosition, FOV, MainCamera.Rotation, Widget.ContentRect.Size );
		return new Ray( MainCamera.Position, direction );
	}

	public Vector2? Project( Vector3 position )
	{
		var screenPos = Screen.ProjectFromWorld( position, FOV, new Transform( MainCamera.Position, MainCamera.Rotation ), Widget.ContentRect.Size );

		// validation?

		return screenPos;
	}
}
