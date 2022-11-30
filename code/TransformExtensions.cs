using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manipulator;

public static class TransformExtensions
{
	public static Ray RayToLocal( this Transform transform, Ray ray )
	{
		var origin = transform.PointToLocal( ray.Origin );
		var direction = transform.NormalToLocal( ray.Direction );
		return new Ray( origin, direction );
	}
}
