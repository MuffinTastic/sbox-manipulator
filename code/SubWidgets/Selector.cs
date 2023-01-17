using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manipulator.Gizmo;
using Sandbox;
using Editor;

namespace Manipulator.SubWidgets;

public class Selector : Widget
{
	protected List<SelectorButton> Buttons { get; set; }
	private SelectorButton CurrentlySelected;

	public Selector( Widget parent ) : base( parent )
	{

	}

	public void BuildUI()
	{
		Buttons = new List<SelectorButton>();

		if ( Layout is null )
		{
			SetLayout( LayoutMode.LeftToRight );
			Layout.Spacing = ManipulatorWidget.UISpacing;
		}

		BuildButtons();
	}

	protected virtual void BuildButtons()
	{

	}

	public virtual void OnButtonClicked( SelectorButton button )
	{
		SetButtonActive( button );
	}

	protected void AddButton( SelectorButton button )
	{
		Layout.Add( button );

		button.Index = Buttons.Count;
		Buttons.Add( button );

		if ( CurrentlySelected is null )
			CurrentlySelected = button;
	}

	protected void SetButtonActive( SelectorButton setButton )
	{
		foreach ( var button in Buttons )
		{
			button.Checked = button == setButton;
			if ( button.Checked )
				CurrentlySelected = button;
		}
	}

	protected void SetButtonActiveFunc( Func<SelectorButton, bool> selected )
	{
		foreach ( var button in Buttons )
		{
			button.Checked = selected( button );
			if ( button.Checked )
				CurrentlySelected = button;
		}
	}

	protected void SetButtonActiveIndex( int set )
	{
		for ( int i = 0; i < Buttons.Count; i++ )
		{
			Buttons[i].Checked = i == set;
			if ( Buttons[i].Checked )
				CurrentlySelected = Buttons[i];
		}
	}

	Vector2 backgroundPosition;

	protected override void OnPaint()
	{
		var col = Theme.ControlText;

		var r = CurrentlySelected.LocalRect;

		backgroundPosition = Vector2.Lerp( backgroundPosition, CurrentlySelected.Position, RealTime.Delta * 20.0f, clamp: false );

		r.Position = backgroundPosition;

		Paint.ClearPen();
		Paint.SetBrush( Theme.Primary.WithAlpha( 0.75f ) );
		Paint.DrawRect( r, ManipulatorWidget.UIRounding );
	}
}
