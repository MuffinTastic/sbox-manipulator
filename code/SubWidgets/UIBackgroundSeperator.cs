using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Manipulator.SubWidgets;
public class UIBackgroundSeperator : Separator
{
	protected override void OnPaint()
	{
		var col = Theme.ControlText;

		var r = LocalRect;

		Paint.ClearPen();
		Paint.SetBrush( col.WithAlpha( 0.75f ) );
		Paint.DrawRect( r );
	}
}
