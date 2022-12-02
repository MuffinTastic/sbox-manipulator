using Tools;

namespace Manipulator;

public partial class Session
{
	bool leftClickPressed;
	bool rightClickPressed;

	enum DragButton
	{
		None, Left, Right
	}

	private DragButton GetDragButton( bool left, bool right )
	{
		if ( left )
			return DragButton.Left;

		if ( right )
			return DragButton.Right;

		return DragButton.None;
	}

	DragButton dragButton;
	bool pressIsClick;
	bool pressIsLeftClick;
	bool pressIsRightClick;
	private const float pressFuzz = 5.0f;
	Vector2 pressStart;
	Vector2 lastMousePosition;

	public void OnMousePress( MouseEvent e )
	{
		if ( e.LeftMouseButton )
			leftClickPressed = true;
		if ( e.RightMouseButton )
			rightClickPressed = true;

		if ( dragButton == DragButton.None )
		{
			bool startPress = true;

			if ( e.LeftMouseButton )
			{
				if ( StartEditDrag() )
					startPress = false;
			}

			if ( startPress )
			{
				pressStart = e.LocalPosition;
				pressIsClick = true; // assume it's a click
				pressIsLeftClick = e.LeftMouseButton;
				pressIsRightClick = e.RightMouseButton;
			}
		}
	}

	public void OnMouseMove( MouseEvent e )
	{
		var delta = lastMousePosition - e.LocalPosition;

		if ( pressIsClick && ( leftClickPressed || rightClickPressed ) )
		{
			// we went out of range, it's not a click
			if ( e.LocalPosition.Distance( pressStart ) > pressFuzz )
			{
				pressIsClick = false;
				dragButton = GetDragButton( pressIsLeftClick, pressIsRightClick );

				if ( pressIsRightClick && !Gizmo.IsDragging )
					StartCameraRotation();
			}

			// we don't want to do anything until we're sure it's not a click
			return;
		}
		else
		{
			if ( pressIsRightClick && CameraRotating )
			{
				ApplyCameraMoveMouse( delta );
			}
		}

		EditDragMove();

		lastMousePosition = e.LocalPosition;
	}

	public void OnMouseReleased( MouseEvent e )
	{
		if ( e.LeftMouseButton )
			leftClickPressed = false;
		if ( e.RightMouseButton )
			rightClickPressed = false;

		if ( pressIsClick )
		{
			EditMouseClick( e );
		}
		else
		{
			if ( dragButton == GetDragButton( e.LeftMouseButton, e.RightMouseButton ) )
				dragButton = DragButton.None;
		}

		if ( e.LeftMouseButton )
		{
			StopEditDrag();
		}

		if ( e.RightMouseButton )
		{
			if ( CameraRotating )
				StopCameraRotation();
		}
	}

	public void OnWheel( WheelEvent e )
	{
		ApplyCameraScrollWheel( e );
	}

	public void OnKeyPress( KeyEvent e )
	{
		ApplyCameraKeyPress( e );
		EditHandleKeyPress( e );
	}

	public void OnKeyRelease( KeyEvent e )
	{
		ApplyCameraKeyRelease( e );
	}

	public Ray GetCursorRay()
	{
		var direction = Screen.GetDirection( Application.CursorPosition - Widget.ScreenPosition, FOV, Camera.Rotation, Widget.ContentRect.Size );
		return new Ray( Camera.Position, direction );
	}

	public void SnapMouseToWorld( Vector3 position )
	{
		var screenPos = Screen.ProjectFromWorld( position, FOV, new Transform( Camera.Position, Camera.Rotation ), Widget.ContentRect.Size );

		if ( screenPos is Vector2 pos )
		{
			Application.CursorPosition = Widget.ScreenPosition + pos;
		}
	}

	public bool ShouldInteract()
	{
		return !CameraRotating;
	}
}
