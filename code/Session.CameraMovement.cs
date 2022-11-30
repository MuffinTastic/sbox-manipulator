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
	public bool CameraRotating { get; private set; } = false;
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
		CameraRotating = true;
		Widget.Cursor = CursorShape.Blank;
		cameraMovePreviousCursorPosition = Application.CursorPosition;
	}

	private void StopCameraRotation( bool resetMouse = true )
	{
		if ( resetMouse )
			Application.CursorPosition = cameraMovePreviousCursorPosition;

		CameraRotating = false;
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

		Camera.Position += Camera.Rotation * velocity;

		// we might be dragging something while moving
		if ( !velocity.AlmostEqual( 0.0f ) && !CameraRotating )
		{
			EditDragMove();
		}

		if ( !Widget.IsFocused || !CameraRotating )
		{
			if ( CameraRotating )
				StopCameraRotation( resetMouse: false );

			return;
		}

		var angle = new Angles( cameraPitch, cameraYaw, 0.0f );
		Camera.Rotation = angle.ToRotation();
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
		Camera.Position += Camera.Rotation.Forward * e.Delta / 2.0f;
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
