using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manipulator.Gizmo;
using Tools;

namespace Manipulator.SubWidgets;

public class LocalSelector : Selector
{
	private ManipulatorWidget Manipulator { get; set; }

	private int globalButtonIndex;
	private int localButtonIndex;

	private SelectorButton globalButton;
	private SelectorButton localButton;

	public LocalSelector( Widget parent, ManipulatorWidget manipulator ) : base( parent )
	{
		Manipulator = manipulator;

		BuildUI();
	}

	protected override void BuildButtons()
	{
		globalButton = new SelectorButton( this );
		globalButton.Icon = "public";
		globalButton.ToolTip = "Switch to global manipulation";
		globalButton.StatusTip = globalButton.ToolTip;

		localButton = new SelectorButton( this );
		localButton.Icon = "navigation";
		localButton.ToolTip = "Switch to local manipulation";
		localButton.StatusTip = globalButton.ToolTip;

		AddButton( globalButton );
		AddButton( localButton );
	}

	public void OnActivate()
	{

	}

	public override void OnButtonClicked( SelectorButton button )
	{
		base.OnButtonClicked( button );

		Manipulator.Session?.SetLocalManipulation( button == localButton );
	}

	public void OnLocalManipulationSet( bool on )
	{
		SetButtonActiveIndex( on ? localButton.Index : globalButton.Index );
	}
}
