using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manipulator.Gizmo;
using Manipulator.SubWidgets;
using Sandbox.UI;
using Sandbox;
using Tools;

namespace Manipulator;
public partial class ManipulatorWidget
{
	public const float UIMargin = 5.0f;
	public const float UISpacing = 5.0f;
	public const float UIRounding = 10.0f;

	private GizmoSelector GizmoSelector;

	[Event.Hotload]
	private void BuildUI()
	{
		DeactivateUI();

		DestroyChildren();
		GizmoSelector = null;

		if ( Layout is null )
		{
			SetLayout( LayoutMode.TopToBottom );
			Layout.Margin = UIMargin;
		}

		var top = new Widget( this );
		top.SetLayout( LayoutMode.LeftToRight );
		{
			var leftBackground = new UIBackground( this );
			{
				GizmoSelector = new GizmoSelector( top, this );
				leftBackground.Layout.Add( GizmoSelector );
			}

			top.Layout.Add( leftBackground );
			top.Layout.AddStretchCell();
		}

		var bottom = new Widget( this );
		bottom.SetLayout( LayoutMode.LeftToRight );
		{
			bottom.Layout.AddStretchCell();
		}

		Layout.Add( top );
		Layout.AddStretchCell();
		Layout.Add( bottom );

		if ( Session.IsValid() )
		{
			ActivateUI();
		}
		else
		{
			DeactivateUI();
		}
	}

	private void ActivateUI()
	{
		if ( Session.IsValid() && GizmoSelector is not null )
		{
			Session.OnGizmoUpdated += GizmoSelector.OnGizmoSet;
			GizmoSelector.OnActivate();
			GizmoSelector.Show();
		}
	}

	private void DeactivateUI()
	{
		GizmoSelector?.Hide();
		Session?.ResetUIListeners();
	}

	public bool IsUIHovered()
	{
		return GizmoSelector.IsUnderMouse /* ... */;
	}
}
