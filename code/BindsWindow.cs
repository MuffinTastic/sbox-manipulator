using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Editor;
using Manipulator.SubWidgets;

namespace Manipulator;

public class BindsWindow : BaseWindow
{
	private ManipulatorWidget Manipulator;

	private static string BindsTitle = "Manipulator Binds";
	public Action OnClose;

	public object UnsavedBinds { get; private set; }

	private List<BindInput> Inputs;

	public BindsWindow( ManipulatorWidget manipulator ) : base()
	{
		SetWindowIcon( "keyboard" );
		WindowTitle = BindsTitle;
		SetModal( true, true );

		SetLayout( LayoutMode.TopToBottom );
		Layout.Margin = 15;
		Layout.Spacing = 10;

		MinimumWidth = 300;
		MinimumHeight = 400;

		Manipulator = manipulator;

		UnsavedBinds = manipulator.Binds;

		Inputs = new List<BindInput>();

		var properties = Binds.GetProperties();

		var propList = new ScrollArea( this );
		var propListCanvas = new Widget( propList );
		propList.Canvas = new Widget( propList );
		propList.Canvas.SetLayout( LayoutMode.TopToBottom );
		propList.Canvas.SetSizeMode( SizeMode.Default, SizeMode.Expand );
		propList.Canvas.Layout.Spacing = 5;
		{
			foreach ( var prop in properties )
			{
				var widget = new Widget( this );
				widget.SetLayout( LayoutMode.LeftToRight );

				var input = new BindInput( prop.prop, this );
				input.MinimumWidth = 50.0f;
				input.MinimumHeight = 25.0f;
				Inputs.Add( input );

				widget.Layout.AddSpacingCell( 15.0f );
				widget.Layout.Add( new Label( prop.info.Name, widget ) );
				widget.Layout.AddStretchCell();
				widget.Layout.Add( input );
				widget.Layout.AddSpacingCell( 15.0f );

				propList.Canvas.Layout.Add( widget );
			}
		}

		Layout.Add( propList, 1 );
		Layout.AddSeparator();

		var buttonArea = new Widget( this );
		buttonArea.SetLayout( LayoutMode.LeftToRight );
		buttonArea.Layout.Spacing = 5;

		var cancelButton = new Button( "Cancel", buttonArea );
		cancelButton.Clicked += Close;

		var resetButton = new Button( "Reset", buttonArea );
		resetButton.Clicked += ResetBinds;

		var applyButton = new Button( "Apply", buttonArea );
		applyButton.Clicked += SaveBinds;

		var acceptButton = new Button( "Accept", buttonArea );
		acceptButton.Clicked += SaveAndClose;

		buttonArea.Layout.Add( cancelButton );
		buttonArea.Layout.AddStretchCell();
		buttonArea.Layout.Add( resetButton );
		buttonArea.Layout.Add( applyButton );
		buttonArea.Layout.Add( acceptButton );
		Layout.Add( buttonArea );

		CheckForConflicts();

		Show();
	}

	public void ChangeBind( PropertyInfo property, KeyCode key )
	{
		property.SetValue( UnsavedBinds, key, null );
		OnBindsChanged();
	}

	protected override void OnClosed()
	{
		OnClose?.Invoke();
	}

	private void OnBindsChanged()
	{
		WindowTitle = BindsTitle + "*";
		CheckForConflicts();
	}

	private void ResetBinds()
	{
		UnsavedBinds = Binds.Defaults();

		OnBindsChanged();

		foreach ( var input in Inputs )
		{
			input.Reset();
		}
	}

	private void SaveBinds()
	{
		WindowTitle = BindsTitle;
		Manipulator.SaveBinds( (Binds) UnsavedBinds );
	}

	private void SaveAndClose()
	{
		SaveBinds();
		Close();
	}

	private void CheckForConflicts()
	{

		// Build a dictionary of all used keys
		var usedKeys = new Dictionary<KeyCode, List<BindInput>>();
		foreach ( var input in Inputs )
		{
			var key = (KeyCode)input.Property.GetValue( UnsavedBinds );

			if ( !usedKeys.TryGetValue( key, out var inputList ) )
			{
				inputList = new List<BindInput>();
				usedKeys.Add( key, inputList );
			}

			inputList.Add( input );
		}

		// Set inputs to valid/invalid based on whether or not any other inputs use their key
		foreach ( var key in usedKeys.Keys )
		{
			usedKeys.TryGetValue( key, out var inputList );

			var invalid = inputList.Count > 1;

			foreach ( var input in inputList )
			{
				input.Invalid = invalid;
			}
		}

		Update();
	}
}

