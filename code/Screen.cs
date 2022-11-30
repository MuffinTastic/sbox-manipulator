using System;
using Sandbox;

namespace Manipulator;
public static class Screen
{
	// From Sandbox.Game
	// Summary:
	//     Gives a direction vector based on the position of the point on the screen. The
	//     vector represents the direction from the camera's position you would travel if
	//     you could pass through the screen in that direction.
	public static Vector3 GetDirection( Vector2 pos, float fov, Rotation lookAngle, Vector2 screenSize )
	{
		float aspect = screenSize.x / screenSize.y;
		float newFov = MathF.Atan( MathF.Tan( fov.DegreeToRadian() * 0.5f ) * (aspect * 0.75f) ).RadianToDegree() * 2f;
		Vector2 vector = new Vector2( 2f * pos.x / screenSize.x - 1f, 2f * pos.y / screenSize.y - 1f ) * -1f;
		float num3 = MathF.Tan( newFov * MathF.PI / 360f );
		float num4 = num3 / aspect;
		return (new Vector3( 1f, vector.x / (1f / num3), vector.y / (1f / num4) ) * lookAngle).Normal;
	}

	public static Vector2? ProjectFromWorld( Vector3 world, float fov, Transform cameraTransform, Vector2 screenSize )
	{
		var direction = cameraTransform.Position - world;

		float aspect = screenSize.x / screenSize.y;
		float newFov = MathF.Atan( MathF.Tan( fov.DegreeToRadian() * 0.5f ) * (aspect * 0.75f) ).RadianToDegree() * 2f;
		float num3 = MathF.Tan( newFov * MathF.PI / 360f );
		float num4 = num3 / aspect;

		var inv = cameraTransform.Rotation.Inverse;
		var o = direction * inv;

		// this is reversed? weird
		if ( o.x > 0.0f )
			return null;

		// how the hell did i figure this out
		o.y /= o.x;
		o.z /= o.x;
		
		var vector = new Vector2( o.y * (1f / num3), o.z * (1f / num4) );
		vector *= -1.0f;
		vector += new Vector2( 1.0f, 1.0f );
		vector /= 2.0f;
		vector *= screenSize;

		return vector;
	}
}
