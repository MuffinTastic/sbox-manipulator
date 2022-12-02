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

	private static List<GizmoUIAttribute> GetGizmoTypes()
	{
		var pairs = TypeLibrary.GetTypesWithAttribute<GizmoUIAttribute>().ToList();

		foreach ( var pair in pairs )
		{
			pair.Attribute.Type = pair.Type.TargetType;
		}

		var all = pairs
			.Select( p => p.Attribute )
			.OrderBy( a => a.Order )
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
