using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.Internal;
using Manipulator.Extensions;

namespace Manipulator;

public class Outlines : IValid
{
	public static Material Material = Material.FromShader( "manipulator_line.vfx" );

	public bool IsValid => LocalBBoxVertices is not null;

	public Vertex[] LocalBBoxVertices;
	public ushort[] LocalBBoxIndices;

	public Vertex[] XAxisLine;
	public Vector3 XAxisTextPoint;

	public Vertex[] YAxisLine;
	public Vector3 YAxisTextPoint;

	public Vertex[] ZAxisLine;
	public Vector3 ZAxisTextPoint;

	public Vertex[] TotalBBoxVertices;
	public ushort[] TotalBBoxIndices;

	public readonly Color LocalBBoxColor;
	public readonly Color TotalBBoxColor;

	public Outlines( Color localBBoxColor, Color totalBBoxColor )
	{
		LocalBBoxColor = localBBoxColor;
		TotalBBoxColor = totalBBoxColor;
	}

	public void UpdateFrom( IEnumerable<IEntity> entities )
	{
		Clear();

		if ( !entities.Any() )
			return;

		var allPoints = new List<Vector3>();
		List<Vertex> localBBoxVertices = new();
		List<Vertex> totalBBoxVertices = new();
		List<ushort> localBBoxIndices = new();
		List<ushort> totalBBoxIndices = new();

		foreach ( var entity in entities )
		{
			if ( entity is null )
				continue;

			var sceneModel = entity.ExposedGetSceneObject();
			var model = sceneModel?.Model;
			if ( model is not null )
			{
				var localCorners = model.Bounds.Corners
					.Select( p => sceneModel.Position + sceneModel.Rotation * p )
					.ToArray();

				BuildLines( localCorners, ref localBBoxVertices, ref localBBoxIndices, LocalBBoxColor );
			}

			allPoints.AddRange( entity.WorldSpaceBounds.Corners );
		}

		var totalBBox = allPoints.GetAABB();
		BuildLines( totalBBox.Corners, ref totalBBoxVertices, ref totalBBoxIndices, TotalBBoxColor );

		LocalBBoxVertices = localBBoxVertices.ToArray();
		TotalBBoxVertices = totalBBoxVertices.ToArray();
		LocalBBoxIndices = localBBoxIndices.ToArray();
		TotalBBoxIndices = totalBBoxIndices.ToArray();
	}

	public void UpdateFrom( IEntity entity )
	{
		Clear();

		if ( entity is null )
			return;

		List<Vertex> localBBoxVertices = new();
		List<Vertex> totalBBoxVertices = new();
		List<ushort> localBBoxIndices = new();
		List<ushort> totalBBoxIndices = new();

		var sceneModel = entity.ExposedGetSceneObject();
		var model = sceneModel?.Model;
		if ( model is not null )
		{ 
			var localCorners = model.Bounds.Corners
				.Select( p => sceneModel.Position + sceneModel.Rotation * p )
				.ToArray();

			BuildLines( localCorners, ref localBBoxVertices, ref localBBoxIndices, LocalBBoxColor );
		}

		var totalBBox = entity.WorldSpaceBounds;
		BuildLines( totalBBox.Corners, ref totalBBoxVertices, ref totalBBoxIndices, TotalBBoxColor );
		
		LocalBBoxVertices = localBBoxVertices.ToArray();
		TotalBBoxVertices = totalBBoxVertices.ToArray();
		LocalBBoxIndices = localBBoxIndices.ToArray();
		TotalBBoxIndices = totalBBoxIndices.ToArray();
	}

	public void Render()
	{
		Graphics.Draw( LocalBBoxVertices, LocalBBoxVertices.Length,
			LocalBBoxIndices, LocalBBoxIndices.Length,
			Material, primitiveType: Graphics.PrimitiveType.Lines );

		Graphics.Draw( TotalBBoxVertices, TotalBBoxVertices.Length,
			TotalBBoxIndices, TotalBBoxIndices.Length,
			Material, primitiveType: Graphics.PrimitiveType.Lines );
	}

	private void Clear()
	{
		LocalBBoxVertices = null;
		LocalBBoxIndices = null;
		XAxisLine = null;
		YAxisLine = null;
		ZAxisLine = null;
		TotalBBoxVertices = null;
		TotalBBoxIndices = null;
	}

	private static ushort[] LineIndices = new ushort[]
	{
		0,		1,
		1,		2,
		2,		3,
		3,		0,
			
		4 + 0,	4 + 1,
		4 + 1,	4 + 2,
		4 + 2,	4 + 3,
		4 + 3,	4 + 0,

		0,		4 + 0,
		1,		4 + 1,
		2,		4 + 2,
		3,		4 + 3,
	};

	public static void BuildLines( IEnumerable<Vector3> corners, ref List<Vertex> vertices, ref List<ushort> indices, Color color )
	{
		// corners are expected to be in a particular order
		var baseVertex = new Vertex { Color = color };
		var baseCount = vertices.Count;

		foreach ( var point in corners )
		{
			vertices.Add( baseVertex with { Position = point } );
		}

		for ( var i = 0; i < LineIndices.Length; i++ )
		{
			indices.Add( (ushort) (baseCount + LineIndices[i]) );
		}
	}
}
