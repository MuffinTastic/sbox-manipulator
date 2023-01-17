using System;
using Sandbox;

using static Manipulator.Gizmo.Gizmo;

namespace Manipulator.Gizmo;

public abstract class SubGizmo : IDisposable
{
	protected static Material Override = Material.Load( "materials/mnp_override.vmat" );
	protected static Material OverrideNoCull = Material.Load( "materials/mnp_override_nocull.vmat" );

	protected float ModelSize;
	protected float ModelSizeScaleAdjuster;
	protected float ModelSizeHalfScaleAdjuster;

	protected readonly Gizmo Parent;
	protected readonly Axis Axis;

	protected Plane plane;

	protected SceneModel sceneModel;

	public SubGizmo( Gizmo parent, Axis axis, string modelName )
	{
		Parent = parent;
		Axis = axis;

		sceneModel = new SceneModel( parent.Session.SceneWorld, modelName, Transform.Zero );
		sceneModel.RenderingEnabled = false;
		sceneModel.Batchable = false;

		var boundsSize = sceneModel.Model.RenderBounds.Size;
		ModelSize = MathF.Max( boundsSize.x, MathF.Max( boundsSize.y, boundsSize.z ) );
		ModelSizeScaleAdjuster = 1.0f / (ModelSize);
		ModelSizeHalfScaleAdjuster = 1.0f / (ModelSize * 2.0f);
	}

	public void Dispose()
	{
		sceneModel.Delete();
	}

	public abstract void Update();
	public abstract void StartDrag( Ray ray );
	public abstract void UpdateDrag( Ray ray );
	public abstract void PreRender( Session session );
	public abstract bool Intersects( Ray ray, out float distance );

	public void Render()
	{
		Graphics.Attributes.Set( "Color", GetGizmoColor() );
		Graphics.Render( sceneModel, material: Override );
	}

	public abstract Plane GetAppropriatePlaneForAxis( Vector3 origin );

	public virtual Color GetGizmoColor()
	{
		var ray = Parent.Session.GetCursorRay();
		var intersects = Parent.IsHovering( ray, this );

		if ( Parent.Dragged == this || (Parent.Dragged is null && intersects) )
			return Color.Yellow;

		var color = Color.Black;

		switch ( Axis )
		{
			case Axis.X: color = new Vector3( 1.0f, 0.0f, 0.0f ); break;
			case Axis.Y: color = new Vector3( 0.0f, 1.0f, 0.0f ); break;
			case Axis.Z: color = new Vector3( 0.0f, 0.0f, 1.0f ); break;
			case Axis.Camera: color = new Vector3( 1.0f, 1.0f, 1.0f ); break;
		}

		if ( Parent.Session.LocalManipulation )
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
