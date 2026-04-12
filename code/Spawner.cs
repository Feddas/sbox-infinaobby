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
	/// <summary> What to spawn </summary>
	[Property, Category( "Components" )] GameObject IncomingPrefab { get; set; }

	/// <summary> Where to put it in the object hierarchy </summary>
	[Property, Category( "Components" )] GameObject ContainerForIncoming { get; set; }

	[Property, Category( "Components" )] Health PlayerLife { get; set; }

	/// <summary> Current obstacle speed </summary>
	[ReadOnly, Property, Category( "Debug" )] public float ObstacleSpeed { get; private set; }

	[Property, Category( "Debug" )] bool IsSpawning { get; set; } = true;

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
		// TODO: change Random seed to change based on the UTC day of the year.
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
		//Log.Info( $"{this.GameObject.Name} finished spawning due to player died. completed? {spawnTask.IsCompleted}" );
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
		Rng.Reset();
		ObstacleSpeed = 2;
		spawnArea = BBox.FromPositionAndSize( this.WorldPosition, thickness * this.LocalScale );
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

			// seconds until next platform is spawned
			// frequency of platform spawning is directly related to ObstacleSpeed
			waitSeconds = Remap( ObstacleSpeed, 1, slowestSpeed,
				0.76f,   // platforms spawn more often to account for them moving faster
				1.3f ); // platforms spawn less often to account for them moving slower
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
		ObstacleSpeed *= .9f; // Every obstacle moves 10% faster
		if ( ObstacleSpeed < 1 ) // 10% faster with a max speed traversing the entire distance in 1 second
		{
			ObstacleSpeed = 1f;
		}
		float clampedY = lastSpawnStart.y.Clamp( -120, 120 );
		int newYvalue = Rng.Next( (int)(clampedY - thickness), (int)(clampedY + thickness) );
		int newZvalue = (int)lastSpawnStart.z + Rng.Next( 15, 45 ); // how much height is incremented
		lastSpawnStart = new Vector3( lastSpawnStart.x, newYvalue, newZvalue );
	}

	/// <summary>
	/// exponential varation of https://youtu.be/NzjF1pdlK7Y
	/// </summary>
	private float Remap( float input, float inMin, float inMax, float outMin, float outMax )
	{
		float t = input.LerpInverse( inMin, inMax );
		return outMin.LerpTo( outMax, (float)Math.Sqrt( t ) );
	}
}
