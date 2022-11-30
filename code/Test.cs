using Sandbox;

namespace Manipulator;

internal class Test
{
	[ConCmd.Engine]
	public static void GetMainSceneWorld()
	{
		var main = Exposed.SceneWorld.GetMainSceneWorld();
		Log.Info( $"Main SceneWorld: {main}" );

		if ( main is not null )
		{
			int k = 0;
			foreach ( var m in main.SceneObjects )
			{
				Log.Info( $"  {k++}: {m.Model}" );
			}
		}
	}
}
