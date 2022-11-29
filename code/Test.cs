using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.Internal;

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
