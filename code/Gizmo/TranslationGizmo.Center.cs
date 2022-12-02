using Sandbox;
using Manipulator.Extensions;

namespace Manipulator.Gizmo;

public partial class TranslationGizmo
{
	private class CenterGizmo : SubGizmo
	{
		public CenterGizmo( Gizmo parent, Axis axis ) : base( parent, axis, "models/gizmo_center.vmdl" )
		{

		}

		public override void Update()
		{
			Vector3 corner1, corner2;

			corner1 = new Vector3( -Width );
			corner2 = new Vector3( Width );

			var scale = Parent.GetCameraAdjustedScale();
			corner1 *= scale;
			corner2 *= scale;

			Vector3.Sort( ref corner1, ref corner2 );
			bbox = new BBox( corner1, corner2 );
		}

		public override void StartDrag()
		{
			var origin = Parent.Selection.Position;
			plane = GetAppropriatePlaneForAxis( origin );

			Parent.Session.SnapMouseToWorld( Parent.GetSelectionTransform().Position );
		}

		public override void UpdateDrag( Ray ray )
		{
			var transform = Parent.GetDragTransform();

			Vector3 point = plane.Trace( ray, twosided: true ).GetValueOrDefault();

			var newPosition = transform.PointToWorld( point );
			Parent.Move( point, transform.Rotation );
		}

		public override void Render( Session session )
		{
			var color = GetGizmoColor();
			var transform = Parent.GetSelectionTransform();
			var scale = Parent.GetCameraAdjustedScale();

			var renderTransform = new Transform(
				transform.Position,
				transform.Rotation,
				scale * RenderSize
			);

			sceneModel.Transform = renderTransform;

			Graphics.Render( sceneModel, null, color, Override );
		}

		public override bool Intersects( Ray ray )
		{
			var transform = Parent.GetSelectionTransform();
			ray = transform.RayToLocal( ray );
			return bbox.Intersection( ray, out var _, out var _ );
		}

		public override Plane GetAppropriatePlaneForAxis( Vector3 origin )
		{
			return new Plane( origin, Parent.AxisToVector( Axis.Camera ) );
		}
	}
}
