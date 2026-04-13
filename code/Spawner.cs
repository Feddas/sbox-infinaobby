using Sandbox;
using Sandbox.Diagnostics;
using System;
using System.Threading.Tasks;
using Rng = Sandbox.SeedRandom;

/// <summary>
/// Spawns gradually higher platforms that move increasingly faster.
/// Platforms generated based on player controller's air time in seconds and max height of jump. This enables generating only do-able jumps.
/// </summary>
public sealed class Spawner : Component
{
	// seconds it takes from jumping off a flat surface to landing back down on it, as calculated from testing jumping in Assets/scenes/measure.scene
	private const float playerSecondsFlatJump = 0.68f;

	// the full height delta of a jump. traversed starting at a surface to the apex of that jump.
	private const float playerJumpHeight = 51f;

	/// <summary> How often new platforms are spawned. Correlated with the players max jump distance in <seealso cref="spawnPlatforms"/>.
	/// 50 is staring difficulty, 200 is speed of max difficulty </summary>
	//[Property, Category( "Spawning" )]
	private Curve frequencyOverSeconds { get; set; } = new Curve( new List<Curve.Frame>
	{
		new Curve.Frame(0f, 50f, 0f, 0f),
		new Curve.Frame(60f, 200f, 0f, 0f)
	} );

	/// <summary> The increase in altitude of the next platform </summary>
	//[Property, Category( "Spawning" )] 
	private CurveRange heightVariance { get; set; } = new CurveRange( HeightVarianceMin, HeightVarianceMax );// Reaches RangedFloat(15,45) over 60 seconds
	private static readonly Curve HeightVarianceMin = new Curve( new List<Curve.Frame>
	{
		new Curve.Frame(0f, 5f, 0f, 0f),
		new Curve.Frame(60f, 15f, 0f, 0f)
	} );
	private static readonly Curve HeightVarianceMax = new Curve( new List<Curve.Frame>
	{
		new Curve.Frame(0f, 15f, 0f, 0f),
		new Curve.Frame(60f, 45f, 0f, 0f)
	} );

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
	private float secondThisRunStarted;

	/// <summary> How much time to allocate to allow for the player to rest on the platform. </summary>
	private float platformRestSeconds
	{
		get
		{
			float platformLength = 50;

			// if a platform is 50 units long and is going 100 units a second, it takes (units/100) = 0.5 second to traverse the entire length of the platform, 50 units.
			// if the player rests on 100% of the length of the platform, it will be just shy of impossible for the player to make it to the next platform that comes in playerSecondsFlatJump.
			// if the player rests on 0% of the length of the platform, they have to jump at the exact time they land, otherwise they will jump completely over the next platform that comes in playerSecondsFlatJump.
			// the 50% (.5f) below gives the player the maximum wiggle room to rest and recalibrate on the platform.
			float platformRest = platformLength * .5f;
			return platformRest / ObstacleSpeed;
		}
	}

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
		spawnArea = BBox.FromPositionAndSize( this.WorldPosition, thickness * this.LocalScale );
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
		await spawnTask;
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
		secondThisRunStarted = Time.Now;

		spawnTask = spawnPlatforms();
	}

	private async Task spawnPlatforms()
	{
		// spawn initial platform
		ObstacleSpeed = frequencyOverSeconds.Evaluate( 0 );
		Vector3 nextSpawnPosition = Rng.VectorInCube( spawnArea );
		spawnIncomingAt( nextSpawnPosition );

		// continue spawn until player is killed
		while ( IsSpawning )
		{
			float secondsOnThisRun = Time.Now - secondThisRunStarted;

			// determine speed of next platform
			ObstacleSpeed = frequencyOverSeconds.Evaluate( secondsOnThisRun );

			// determine next side position
			float nextYUnclamped = nextSpawnPosition.y + Rng.Next( -50, 50 ); // less than the distance a player can traverse in a jump
			nextSpawnPosition.y = nextYUnclamped.Clamp( spawnArea.Mins.y, spawnArea.Maxs.y ); // clamping it within the original spawn area also keeps it inside the walls

			// next height position must be less than playerJumpHeight
			float pointInRange = Rng.Next( 0, 100 ) / 100f;
			float nextHeightDelta = heightVariance.Evaluate( secondsOnThisRun, pointInRange );
			nextSpawnPosition.z += nextHeightDelta;

			// wait to ensure a jump to nextHeightDelta is possible
			float nextHeightDeltaPercentOfPlayerFall = (playerJumpHeight - nextHeightDelta) / playerJumpHeight; // if the nextHeightDelta == playerJumpHeight, then 0% of the fall can occur before the player is at the nextHeightDelta
			float playerSecondsToJumpToNext = (playerSecondsFlatJump / 2) // time to get to the apex of a jump
				+ (playerSecondsFlatJump / 2) * nextHeightDeltaPercentOfPlayerFall;
			await Task.DelaySeconds( playerSecondsToJumpToNext + platformRestSeconds );

			// spawn platform
			spawnIncomingAt( nextSpawnPosition );
		}
	}

	private void spawnIncomingAt( Vector3 spawnPostion )
	{
		// create
		Incoming obstacle = IncomingPrefab.Clone( new Transform( spawnPostion ), ContainerForIncoming ).Components.Get<Incoming>();
		obstacle.Speed = ObstacleSpeed;
		obstacle.Color = Rng.RandomColor();
	}

	/// <summary>
	/// exponential varation of https://youtu.be/NzjF1pdlK7Y
	/// </summary>
	//private float Remap( float input, float inMin, float inMax, float outMin, float outMax )
	//{
	//	float t = input.LerpInverse( inMin, inMax );
	//	return outMin.LerpTo( outMax, (float)Math.Sqrt( t ) );
	//}
}
