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

	private List<Widget> Hideables;

	private GizmoSelector GizmoSelector;
	private LocalSelector LocalSelector;

	[Event.Hotload]
	private void BuildUI()
	{
		if ( Hideables is null )
			Hideables = new List<Widget>();

		DeactivateUI();

		Hideables.Clear();

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
				leftBackground.Layout.Spacing = UISpacing;
				leftBackground.Layout.Add( GizmoSelector );
				leftBackground.Layout.Add( new UIBackgroundSeperator() );
				LocalSelector = new LocalSelector( top, this );
				leftBackground.Layout.Add( LocalSelector );
			}

			Hideables.Add( leftBackground );
			top.Layout.Add( leftBackground );
			top.Layout.AddStretchCell();

			var rightBackground = new UIBackground( this );
			rightBackground.Layout.Spacing = UISpacing;
			{
				var hideable = new Widget( this );
				hideable.SetLayout( LayoutMode.LeftToRight );
				hideable.Layout.Spacing = UISpacing;
				{
					var bootan = new ManipulatorButton( this );
					bootan.Icon = "celebration";
					hideable.Layout.Add( bootan );
					hideable.Layout.Add( new UIBackgroundSeperator() );
				}
				Hideables.Add( hideable );
				rightBackground.Layout.Add( hideable );

				var settingsButton = new ManipulatorButton( this );
				settingsButton.Icon = "settings";
				settingsButton.Clicked += () => Log.Info( "mhm " );
				rightBackground.Layout.Add( settingsButton );
			}

			top.Layout.Add( rightBackground );

		}

		Layout.Add( top );
		Layout.AddStretchCell();

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
			Session.OnLocalManipulationUpdated += LocalSelector.OnLocalManipulationSet;
			GizmoSelector.OnActivate();

			foreach ( var widget in Hideables )
			{
				widget.Show();
			}
		}
	}

	private void DeactivateUI()
	{
		foreach ( var widget in Hideables )
		{
			widget?.Hide();
		}

		Session?.ResetUIListeners();
	}

	public bool IsUIHovered()
	{
		return Children.Any( w => w.IsUnderMouse );
	}
}
