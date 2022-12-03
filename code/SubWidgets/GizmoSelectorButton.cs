﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manipulator.Gizmo;
using Tools;

namespace Manipulator.SubWidgets;
public class GizmoSelectorButton : SelectorButton
{
	public GizmoUIAttribute Attribute { get; init; }

	public GizmoSelectorButton( GizmoSelector parent, GizmoUIAttribute attribute ) : base( parent )
	{
		Attribute = attribute;

		Icon = attribute.Icon;
		StatusTip = attribute.Name;
		ToolTip = $"Switch to the {attribute.Name} gizmo";
		Cursor = CursorShape.Finger;
	}
}
