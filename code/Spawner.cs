using Sandbox;
using Sandbox.Diagnostics;
using System;
using System.Threading.Tasks;
using Rng = Sandbox.SeedRandom;


/// <summary>
/// Spawns gradually higher platforms the move increasingly faster
/// TODO: Redo class to generate based on player controller's air time in seconds and max height of jump. This will allow only generating do-able jumps.
/// </summary>
public sealed class Spawner : Component
{
	/// <summary> Current obstacle speed </summary>
	[ReadOnly, Property] public float ObstacleSpeed { get; private set; }

	/// <summary> What to spawn </summary>
	[Property] GameObject IncomingPrefab { get; set; }

	/// <summary> Where to put it in the object hierarchy </summary>
	[Property] GameObject ContainerForIncoming { get; set; }

	[Property] bool IsSpawning { get; set; } = true;

	[Property] Health PlayerLife { get; set; }

	// what number localscale is multiplied by. 50 matches a default box collider.
	private float thickness = 50;
	private Task spawnTask;
	private BBox spawnArea;

	private Vector3 lastSpawnStart;

	protected override void OnStart()
	{
		base.OnStart();
		if ( ContainerForIncoming == null )
		{
			ContainerForIncoming = this.GameObject.Parent;
		}

		// setup first obstacle
		Rng.Initialize( 42 + 1 ); // DaysSince.Release();
		resetSpawner();
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
		PlayerLife.OnDeath += OnPlayerDied;
	}

	protected override void OnDisabled()
	{
		base.OnDisabled();
		PlayerLife.OnDeath -= OnPlayerDied;
	}

	private async void OnPlayerDied()
	{
		IsSpawning = false;
		await spawnTask; //spawnTask.Dispose();
		if ( false == spawnTask.IsCompleted )
		{
			return; // This should never happen... but it does. I need breakpoints to understand why. :(
		}
		Log.Info( $"{this.GameObject.Name} finished spawning due to player died. completed? {spawnTask.IsCompleted}" );
		var toDelete = ContainerForIncoming.Children.Where( c => c.Components.Get<Incoming>() != null );
		foreach ( var c in toDelete )
		{
			c.Destroy();
		}

		IsSpawning = true;
		resetSpawner();
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

	private void resetSpawner()
	{
		// TODO: change Random seed to change based on the UTC day of the year.
		Rng.Reset();
		ObstacleSpeed = 3;
		spawnArea = BBox.FromPositionAndSize( this.Transform.Position, thickness * this.Transform.LocalScale );
		lastSpawnStart = Rng.VectorInCube( spawnArea );
		spawnTask = spawnEvery( 1.5f );
	}

	private async Task spawnEvery( float waitSeconds )
	{
		float slowestSpeed = ObstacleSpeed;
		while ( IsSpawning )
		{
			//Log.Info( $"using {waitSeconds} at {Time.Now}" );
			// wait for this amount of seconds
			await Task.DelaySeconds( waitSeconds );

			spawnIncoming();

			// frequency of platforms is directly related to ObstacleSpeed
			waitSeconds = Remap( ObstacleSpeed, 1, slowestSpeed, 1.2f, 1.6f ); // 1.2 = fastest platforms will come, 1.6 slowest platforms will come.
		}
	}

	/// <summary>
	/// spawn <see cref="IncomingPrefab"/> at a random position inside this cube and hurtle it towards the player.
	/// </summary>
	private void spawnIncoming()
	{
		Assert.NotNull( IncomingPrefab ); // throw an error if wasn't defined

		// create
		Incoming obstacle = IncomingPrefab.Clone( new Transform( lastSpawnStart ), ContainerForIncoming ).Components.Get<Incoming>();
		obstacle.SecondsToTraverse = ObstacleSpeed;
		obstacle.Color = Rng.RandomColor();

		// setup next
		ObstacleSpeed-=.2f;
		if ( ObstacleSpeed < 1)
		{
			ObstacleSpeed = 1f;
		}
		float clampedY = lastSpawnStart.y.Clamp( -120, 120 );
		int newYvalue = Rng.Next( (int)(clampedY - thickness), (int)(clampedY + thickness) );
		int newZvalue = (int)lastSpawnStart.z + Rng.Next( 20, 50 );
		lastSpawnStart = new Vector3( lastSpawnStart.x, newYvalue, newZvalue );

		// TODO: delete all obstacles on death
	}

	/// <summary>
	/// exponential varation of https://youtu.be/NzjF1pdlK7Y
	/// </summary>
	private float Remap( float input, float inMin, float inMax, float outMin, float outMax )
	{
		float t = input.LerpInverse( inMin, inMax );
		return outMin.LerpTo( outMax, (float)Math.Sqrt(t) );
	}
}
