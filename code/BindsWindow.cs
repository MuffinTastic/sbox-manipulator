using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using static Tools.Label;

namespace Manipulator;

public class BindsWindow : BaseWindow
{
	public BindsWindow( Binds binds ) : base()
	{
		SetWindowIcon( "info" );
		WindowTitle = "Manipulator Binds";
		SetModal( true, true );

		SetLayout( LayoutMode.TopToBottom );
		Layout.Margin = 15;
		Layout.Spacing = 10;

		MinimumWidth = 300;
		MinimumHeight = 400;

		var properties = Binds.GetProperties();


		var propList = new ScrollArea( this );
		var propListCanvas = new Widget( propList );
		propList.Canvas = new Widget( propList );
		propList.Canvas.SetLayout( LayoutMode.TopToBottom );
		propList.Canvas.SetSizeMode( SizeMode.Default, SizeMode.Expand );
		propList.Canvas.Layout.Spacing = 10;
		{
			foreach ( var prop in properties )
			{
				var widget = new Widget( this );
				widget.SetLayout( LayoutMode.LeftToRight );

				widget.Layout.Add( new Label( prop.info.Name, widget ) );
				widget.Layout.AddStretchCell();
				widget.Layout.Add( new Label( "aaa", widget ) );

				propList.Canvas.Layout.Add( widget );
			}
		}

		Layout.Add( propList, 1 );
		Layout.AddSeparator();

		var buttonArea = new Widget( this );
		buttonArea.SetLayout( LayoutMode.LeftToRight );
		buttonArea.Layout.Add( new Button( "mhm", buttonArea ) );
		buttonArea.Layout.AddStretchCell();
		buttonArea.Layout.Add( new Button( "yeah", buttonArea ) );
		Layout.Add( buttonArea );

		Show();
	}
}
