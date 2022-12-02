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

	private void CreateUI()
	{
		SetLayout( LayoutMode.TopToBottom );

		Layout.Margin = UIMargin;

		var top = new Widget( this );
		top.SetLayout( LayoutMode.LeftToRight );
		{
			GizmoSelector = new GizmoSelector( top, this );

			top.Layout.Add( GizmoSelector );
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

		DeactivateUI();
	}

	private void ActivateUI()
	{
		if ( Session.IsValid() && GizmoSelector is not null )
		{
			Session.OnGizmoUpdated += GizmoSelector.OnGizmoSet;
			GizmoSelector.Show();
		}
	}

	private void DeactivateUI()
	{
		GizmoSelector?.Hide();
	}
}
