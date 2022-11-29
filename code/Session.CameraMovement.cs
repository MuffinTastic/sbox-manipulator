using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Tools;

namespace Manipulator;
public partial class Session
{
	private const float CameraDefaultSpeed = 200.0f;
	private const float CameraBoostSpeed = 1600.0f;

	Vector2 cameraMovePreviousCursorPosition;
	bool cameraRotating = false;
	float cameraPitch = 0.0f;
	float cameraYaw = 0.0f;

	bool cameraForwardPressed;
	bool cameraBackwardPressed;
	bool cameraLeftPressed;
	bool cameraRightPressed;
	bool cameraUpPressed;
	bool cameraDownPressed;
	bool cameraBoostPressed;

	private void StartCameraRotation()
	{
		cameraRotating = true;
		Widget.Cursor = CursorShape.Blank;
		cameraMovePreviousCursorPosition = Application.CursorPosition;
	}

	private void StopCameraRotation( bool resetMouse = true )
	{
		if ( resetMouse )
			Application.CursorPosition = cameraMovePreviousCursorPosition;

		cameraRotating = false;
		Widget.Cursor = CursorShape.None;
	}

	private void UpdateCamera()
	{
		var backward = cameraBackwardPressed ? 1.0f : 0.0f;
		var left = cameraLeftPressed ? 1.0f : 0.0f;
		var right = cameraRightPressed ? 1.0f : 0.0f;
		var up = cameraUpPressed ? 1.0f : 0.0f;
		var down = cameraDownPressed ? 1.0f : 0.0f;
		var forward = cameraForwardPressed ? 1.0f : 0.0f;

		var velocity = new Vector3( forward - backward, left - right, up - down ).Normal;
		velocity *= cameraBoostPressed ? CameraBoostSpeed : CameraDefaultSpeed;
		velocity *= RealTime.Delta;

		MainCamera.Position += MainCamera.Rotation * velocity;
		OverlayCamera.Position = MainCamera.Position;

		if ( !Widget.IsFocused || !cameraRotating )
		{
			if ( cameraRotating )
				StopCameraRotation( resetMouse: false );

			return;
		}

		var angle = new Angles( cameraPitch, cameraYaw, 0.0f );
		MainCamera.Rotation = angle.ToRotation();
		OverlayCamera.Rotation = MainCamera.Rotation;
	}

	private void ApplyCameraMoveMouse( Vector2 delta )
	{
		delta /= 2.0f;
		cameraYaw += delta.x;
		cameraPitch -= delta.y;

		if ( cameraYaw > 360.0f )
			cameraYaw -= 360.0f;
		else if ( cameraYaw < 0.0f )
			cameraYaw += 360.0f;

		cameraPitch = MathF.Max( -90.0f, MathF.Min( cameraPitch, 90.0f ) );
	}

	public void ApplyCameraScrollWheel( WheelEvent e )
	{
		MainCamera.Position += MainCamera.Rotation.Forward * e.Delta / 2.0f;
		OverlayCamera.Position = MainCamera.Position;
	}

	private void ApplyCameraKeyPress( KeyEvent e )
	{
		if ( e.Key == KeyCode.W )
			cameraForwardPressed = true;
		if ( e.Key == KeyCode.S )
			cameraBackwardPressed = true;
		if ( e.Key == KeyCode.A )
			cameraLeftPressed = true;
		if ( e.Key == KeyCode.D )
			cameraRightPressed = true;
		if ( e.Key == KeyCode.E )
			cameraUpPressed = true;
		if ( e.Key == KeyCode.Q )
			cameraDownPressed = true;
		if ( e.Key == KeyCode.Shift )
			cameraBoostPressed = true;
	}

	private void ApplyCameraKeyRelease( KeyEvent e )
	{
		if ( e.Key == KeyCode.W )
			cameraForwardPressed = false;
		if ( e.Key == KeyCode.S )
			cameraBackwardPressed = false;
		if ( e.Key == KeyCode.A )
			cameraLeftPressed = false;
		if ( e.Key == KeyCode.D )
			cameraRightPressed = false;
		if ( e.Key == KeyCode.E )
			cameraUpPressed = false;
		if ( e.Key == KeyCode.Q )
			cameraDownPressed = false;
		if ( e.Key == KeyCode.Shift )
			cameraBoostPressed = false;
	}
}
