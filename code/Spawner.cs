using Sandbox;
using Sandbox.Diagnostics;
using System.Threading.Tasks;

public sealed class Spawner : Component
{
	[Property] GameObject IncomingPrefab { get; set; }
	[Property] bool IsSpawning { get; set; } = true;

	// what number localscale is multiplied by. 50 matches a default box collider.
	private float thickness = 50;
	private Task spawnTask;
	private BBox spawnArea;

	protected override void OnStart()
	{
		base.OnStart();
		spawnArea = BBox.FromPositionAndSize( this.Transform.Position, thickness * this.Transform.LocalScale );
		spawnTask = spawnEvery( 2 );
	}

	protected override void OnUpdate() { }

	//protected override void OnValidate()
	//{
	//	base.OnValidate();
	//	spawnTask.Dispose();
	//	if ( false == gamemode.isrunning ) return;
	//	spawnTask = spawnEvery( 2 );
	//}

	protected override void DrawGizmos()
	{
		// show, in editor, the spawn area
		Gizmo.Draw.LineBBox( BBox.FromPositionAndSize( Vector3.Zero, thickness ) );
	}

	private async Task spawnEvery(float waitSeconds)
	{
		while ( IsSpawning )
		{
			// wait for this amount of seconds
			await Task.DelaySeconds( waitSeconds );

			spawnIncoming();
		}
	}

	/// <summary>
	/// spawn <see cref="IncomingPrefab"/> at a random position inside this cube and hurtle it towards the player.
	/// </summary>
	private void spawnIncoming()
	{
		Assert.NotNull( IncomingPrefab ); // throw an error if wasn't defined

		Incoming obsticale = IncomingPrefab.Clone(new Transform( spawnArea.RandomPointInside), this.GameObject.Parent ).Components.Get<Incoming>();
	}
}
