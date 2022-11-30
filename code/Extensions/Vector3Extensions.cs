using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manipulator.Extensions;

public static class Vector3Extensions
{
	public static BBox GetAABB( this IEnumerable<Vector3> vectors, Vector3 offset = default )
	{
		var count = vectors.Count();
		if ( count == 0 )
			return new BBox();

		var bbox = new BBox( vectors.ElementAt( 0 ) + offset );

		for ( int i = 1; i < count; i++ )
		{
			bbox = bbox.AddPoint( vectors.ElementAt( i ) + offset );
		}

		return bbox;
	}
}
