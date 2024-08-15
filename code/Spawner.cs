using Sandbox;
using Sandbox.Diagnostics;
using System;
using System.Threading.Tasks;

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

	private Random rand;
	private Vector3 lastSpawnStart;

	protected override void OnStart()
	{
		base.OnStart();
		if ( ContainerForIncoming == null )
		{
			ContainerForIncoming = this.GameObject.Parent;
		}

		// setup first obstacle
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
		Log.Info( $"{this.GameObject.Name} knows that player died" );
		IsSpawning = false;
		await spawnTask; //spawnTask.Dispose();
		Log.Info( $"{this.GameObject.Name} finished spawning due to player died" );
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
		rand = new Random( 42 + 1 );
		ObstacleSpeed = 3;
		spawnArea = BBox.FromPositionAndSize( this.Transform.Position, thickness * this.Transform.LocalScale );
		lastSpawnStart = rand.VectorInCube( spawnArea );
		spawnTask = spawnEvery( 1.5f );
	}

	private async Task spawnEvery( float waitSeconds )
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

		// create
		Incoming obstacle = IncomingPrefab.Clone( new Transform( lastSpawnStart ), ContainerForIncoming ).Components.Get<Incoming>();
		obstacle.SecondsToTraverse = ObstacleSpeed;
		obstacle.Color = RandomColor();

		// setup next
		//ObstacleSpeed-=.5f;
		float clampedY = lastSpawnStart.y.Clamp( -120, 120 );
		int newYvalue = rand.Next( (int)(clampedY - thickness), (int)(clampedY + thickness) );
		int newZvalue = (int)lastSpawnStart.z + rand.Next( 20, 50 );
		lastSpawnStart = new Vector3( lastSpawnStart.x, newYvalue, newZvalue );

		// TODO: delete all obstacles on death
	}

	public Color RandomColor()
	{
		switch ( rand.Next( 6 ) )
		{
			case 0: return Color.White;
			case 1: return Color.Red;
			case 2: return Color.Green;
			case 3: return Color.Blue;
			case 4: return Color.Yellow;
			case 5: return Color.Cyan;
			case 6: return Color.Magenta;
			default: return Color.Black;
		}
	}

}
