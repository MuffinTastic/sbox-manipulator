using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manipulator.Gizmo;
using Editor;

namespace Manipulator.SubWidgets;
public class GizmoSelectorButton : SelectorButton
{
	public GizmoUIAttribute Attribute { get; init; }

	public GizmoSelectorButton( GizmoSelector parent, GizmoUIAttribute attribute ) : base( parent )
	{
		Attribute = attribute;

		Icon = attribute.Icon;
		ToolTip = $"Switch to the {attribute.Name} gizmo";
		StatusTip = ToolTip;
	}
}
