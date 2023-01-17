using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manipulator.Extensions;

public static class BBoxExtensions
{
	public static bool Intersection( this BBox box, Ray r, out Vector3 point1, out Vector3 point2 )
	{
		var min = box.Center - box.Size / 2 - r.Position;
		var max = box.Center + box.Size / 2 - r.Position;
		float near = float.MinValue;
		float far = float.MaxValue;

		// X
		float t1 = min.x / r.Forward.x;
		float t2 = max.x / r.Forward.x;
		float tMin = Math.Min( t1, t2 );
		float tMax = Math.Max( t1, t2 );
		if ( tMin > near ) near = tMin;
		if ( tMax < far ) far = tMax;
		if ( near > far || far < 0 )
		{
			point1 = Vector3.Zero;
			point2 = Vector3.Zero;
			return false;
		}

		// Y
		t1 = min.y / r.Forward.y;
		t2 = max.y / r.Forward.y;
		tMin = Math.Min( t1, t2 );
		tMax = Math.Max( t1, t2 );
		if ( tMin > near ) near = tMin;
		if ( tMax < far ) far = tMax;
		if ( near > far || far < 0 )
		{
			point1 = Vector3.Zero;
			point2 = Vector3.Zero;
			return false;
		}

		// Z
		t1 = min.z / r.Forward.z;
		t2 = max.z / r.Forward.z;
		tMin = Math.Min( t1, t2 );
		tMax = Math.Max( t1, t2 );
		if ( tMin > near ) near = tMin;
		if ( tMax < far ) far = tMax;
		if ( near > far || far < 0 )
		{
			point1 = Vector3.Zero;
			point2 = Vector3.Zero;
			return false;
		}

		point1 = r.Position + r.Forward * near;
		point2 = r.Position + r.Forward * far;
		return true;
	}
}
