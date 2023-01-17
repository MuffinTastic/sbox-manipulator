using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manipulator.Extensions;

public static class TransformExtensions
{
	public static Ray RayToLocal( this Transform transform, Ray ray )
	{
		var origin = transform.PointToLocal( ray.Position );
		var direction = transform.NormalToLocal( ray.Forward );
		return new Ray( origin, direction );
	}
}
