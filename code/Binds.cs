using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sandbox;
using Tools;

namespace Manipulator;

public class Binds
{
	public KeyCode MoveForward { get; set; }
	public KeyCode MoveBackward { get; set; }
	public KeyCode MoveLeft { get; set; }
	public KeyCode MoveRight { get; set; }
	public KeyCode MoveUp { get; set; }
	public KeyCode MoveDown { get; set; }
	public KeyCode BoostSpeed { get; set; }
	public KeyCode TranslationGizmo { get; set; }
	public KeyCode RotationGizmo { get; set; }
	public KeyCode ScaleGizmo { get; set; }
	public KeyCode ToggleLocalManipulation { get; set; }

	public void ResetToDefaults()
	{
		MoveForward = KeyCode.W;
		MoveBackward = KeyCode.S;
		MoveLeft = KeyCode.A;
		MoveRight = KeyCode.D;
		MoveUp = KeyCode.Q;
		MoveDown = KeyCode.Z;
		BoostSpeed = KeyCode.Shift;

		TranslationGizmo = KeyCode.T;
		RotationGizmo = KeyCode.R;
		ScaleGizmo = KeyCode.E;
		ToggleLocalManipulation = KeyCode.C;
	}

	public static Binds Load()
	{
		var binds = FileSystem.Root.ReadJsonOrDefault<Binds>( "config/manipulator.json", null );
		
		if ( binds is null )
		{
			binds = new Binds();
			binds.ResetToDefaults();
			binds.Save();
		}

		return binds;
	}

	public void Save()
	{
		FileSystem.Root.WriteJson( "config/manipulator.json", this );
	}

	public static List<(PropertyInfo prop, DisplayInfo info)> GetProperties()
	{
		var properties = typeof( Binds ).GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy )
								.Select<PropertyInfo, (PropertyInfo prop, DisplayInfo info)>( x => new( x, DisplayInfo.ForMember( x ) ) )
								.ToList();

		return properties;
	}
}
