using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace Manipulator;
public partial class Session
{
	public static Material OutlineMaterial = Material.FromShader( "Shaders/ManipulatorOutline.vfx" );

	///
	/// For rendering things within the overlay scene 
	///
	public override void OnStage( SceneCamera _, Stage renderStage )
	{
		if ( renderStage == Stage.AfterOpaque )
		{
			foreach ( var sm in testmodels )
			{
				Graphics.Render( sm );
			}
		}

		if ( renderStage == Stage.AfterPostProcess )
		{
			if ( HoverOutlines.IsValid() )
			{
				HoverOutlines.Render();
			}

			if ( SelectionOutlines.IsValid() )
			{
				SelectionOutlines.Render();
			}

			//if ( Gizmo.IsValid() )
			{
				Gizmo.Render();
			}
		}
	}
}
