using System;
using Sandbox;

using static Manipulator.Gizmo.Gizmo;

namespace Manipulator.Gizmo;

public abstract class SubGizmo : IDisposable
{
	protected static Material Override = Material.Load( "materials/mnp_override.vmat" );
	protected static Material OverrideNoCull = Material.Load( "materials/mnp_override_nocull.vmat" );

	protected const float GizmoSize = 16.0f;
	protected const float FinalScale = 0.15f;
	protected const float RenderSize = 1.0f / GizmoSize * FinalScale;
	protected const float Gap = 0.2f * FinalScale;
	protected const float Width = 0.1f * FinalScale;
	protected const float Length = 1.0f * FinalScale;

	protected readonly Gizmo Parent;
	protected readonly Axis Axis;

	protected BBox bbox;
	protected Plane plane;

	protected SceneModel sceneModel;

	public SubGizmo( Gizmo parent, Axis axis, string modelName )
	{
		Parent = parent;
		Axis = axis;

		sceneModel = new SceneModel( parent.Session.SceneWorld, modelName, Transform.Zero );
		sceneModel.RenderingEnabled = false;
	}

	public void Dispose()
	{
		sceneModel.Delete();
	}

	public abstract void Update();
	public abstract void UpdateDrag( Ray ray );
	public abstract void Render( Session session );
	public abstract bool Intersects( Ray ray );

	public virtual void StartDrag()
	{
		var origin = Parent.Selection.Position;
		plane = GetAppropriatePlaneForAxis( origin );
	}

	public abstract Plane GetAppropriatePlaneForAxis( Vector3 origin );

	public virtual Color GetGizmoColor()
	{
		var ray = Parent.Session.GetCursorRay();
		var intersects = Parent.IsHovering( ray, this );

		if ( Parent.Dragged == this || intersects )
			return Color.Yellow;

		var color = Color.Black;

		switch ( Axis )
		{
			case Axis.X: color = new Vector3( 1.0f, 0.0f, 0.0f ); break;
			case Axis.Y: color = new Vector3( 0.0f, 1.0f, 0.0f ); break;
			case Axis.Z: color = new Vector3( 0.0f, 0.0f, 1.0f ); break;
			case Axis.Camera: color = new Vector3( 1.0f, 1.0f, 1.0f ); break;
		}

		if ( Parent.Local )
		{
			if ( Axis == Axis.Z )
				color = new Vector3( 0.5f, 0.0f, 1.0f );

			color = color.Desaturate( 0.1f );
		}

		return color;
	}

	public Vector3 SnapInQuadrant( Vector3 world )
	{
		var local = Parent.SnapToPlaneLocal( Axis, world );

		if ( !local.x.AlmostEqual( 0.0f ) )
		{
			if ( local.x > 0.0f )
				local.x = 1.0f;
			else
				local.x = -1.0f;
		}

		if ( !local.y.AlmostEqual( 0.0f ) )
		{
			if ( local.y > 0.0f )
				local.y = 1.0f;
			else
				local.y = -1.0f;
		}

		if ( !local.z.AlmostEqual( 0.0f ) )
		{
			if ( local.z > 0.0f )
				local.z = 1.0f;
			else
				local.z = -1.0f;
		}

		return local;
	}
}
