using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace Manipulator.Gizmo;

public class GizmoUIAttribute : Attribute
{
	public required string Name { get; set; }
	public required string Icon { get; set; }
	public int Order { get; set; } = 10;

	// --------------------- //

	public int Index { get; private set; }
	public Type Type { get; private set; }

	public Gizmo CreateGizmo( Session session, Selection selection )
	{
		return TypeLibrary.Create<Gizmo>( Type, new object[] { session, selection } );
	}

	// --------------------- //

	private static List<GizmoUIAttribute> AllInternal = null;

	public static ReadOnlyCollection<GizmoUIAttribute> All
	{
		get
		{
			var types = AllInternal ?? GetGizmoTypes();
			return types.AsReadOnly();
		}
	}

	public static GizmoUIAttribute For( Type gizmoType )
	{
		return All.Where( a => a.Type == gizmoType ).FirstOrDefault();
	}

	private static List<GizmoUIAttribute> GetGizmoTypes()
	{
		var pairs = TypeLibrary.GetTypesWithAttribute<GizmoUIAttribute>()
			.OrderBy( p => p.Attribute.Order ).ToList();

		for ( int i = 0; i < pairs.Count; i++ )
		{
			var pair = pairs[i];
			pair.Attribute.Index = i;
			pair.Attribute.Type = pair.Type.TargetType;
		}

		var all = pairs
			.Select( p => p.Attribute )
			.ToList();

		AllInternal = all;
		return AllInternal;
	}

	[Event.Hotload]
	private static void ClearGizmos()
	{
		AllInternal?.Clear();
		AllInternal = null;
		GetGizmoTypes();
	}
}
