using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exposed;
internal static class Util
{
	internal static Type[] ClientTypes => AppDomain.CurrentDomain.GetAssemblies().Where( a => a.FullName.StartsWith( "Sandbox.Game" ) && !IsServerAssembly( a ) ).LastOrDefault().GetTypes();
	internal static Type[] ServerTypes => AppDomain.CurrentDomain.GetAssemblies().Where( a => a.FullName.StartsWith( "Sandbox.Game" ) && IsServerAssembly( a ) ).LastOrDefault().GetTypes();

	internal static bool IsServerAssembly( Assembly a )
	{
		var hostTypes = a.GetTypes().Where( t => t.FullName == "Sandbox.Host" );

		foreach ( var hostType in hostTypes )
		{
			var aa = hostType.GetProperty( "IsClient" );
			var b = aa?.GetValue( null );
			if ( (bool)b )
			{
				return false;
			}
		}

		return true;
	}
}
