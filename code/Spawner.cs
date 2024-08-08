using Sandbox;

public sealed class Spawner : Component
{
	// what number localscale is multiplied by. 50 matches a default box collider.
	private float thickness = 50;

	protected override void OnUpdate()
	{
		// TODO: spawn a prefab at a random position inside this cube and hurtle it towards the player.
	}

	protected override void DrawGizmos()
	{
		// show, in editor, the spawn area
		Gizmo.Draw.LineBBox( BBox.FromPositionAndSize( Vector3.Zero, thickness ) );
	}
}
