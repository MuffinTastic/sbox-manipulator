using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sandbox;
using Editor;

namespace Manipulator;

public struct Binds
{
	public KeyCode MoveForward { get; set; }
	public KeyCode MoveBackward { get; set; }
	public KeyCode MoveLeft { get; set; }
	public KeyCode MoveRight { get; set; }
	public KeyCode MoveUp { get; set; }
	public KeyCode MoveDown { get; set; }
	public KeyCode BoostSpeed { get; set; }
	public KeyCode SelectTranslationGizmo { get; set; }
	public KeyCode SelectRotationGizmo { get; set; }
	public KeyCode SelectScaleGizmo { get; set; }
	public KeyCode ToggleLocalManipulation { get; set; }
	public KeyCode TogglePivotManipulation { get; set; }


	public static Binds Defaults()
	{
		return new Binds
		{
			MoveForward = KeyCode.W,
			MoveBackward = KeyCode.S,
			MoveLeft = KeyCode.A,
			MoveRight = KeyCode.D,
			MoveUp = KeyCode.Q,
			MoveDown = KeyCode.Z,
			BoostSpeed = KeyCode.Shift,
			SelectTranslationGizmo = KeyCode.T,
			SelectRotationGizmo = KeyCode.R,
			SelectScaleGizmo = KeyCode.E,
			ToggleLocalManipulation = KeyCode.C,
			TogglePivotManipulation = KeyCode.X
		};
	}

	public static Binds Load()
	{
		Binds? binds = FileSystem.Root.ReadJsonOrDefault<Binds?>( "config/manipulator.json", null );
		
		if ( binds is null )
		{
			binds = Defaults();
			Save( binds.Value );
		}

		return binds.Value;
	}

	public static void Save( Binds binds )
	{
		FileSystem.Root.WriteJson( "config/manipulator.json", binds );
	}

	public static List<(PropertyInfo prop, DisplayInfo info)> GetProperties()
	{
		var properties = typeof( Binds ).GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy )
								.Select<PropertyInfo, (PropertyInfo prop, DisplayInfo info)>( x => new( x, DisplayInfo.ForMember( x ) ) )
								.ToList();

		return properties;
	}
}
