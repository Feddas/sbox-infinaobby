using Sandbox;

/// <summary> Restricts player movement to be along a line. </summary>
public sealed class PlayerRestrict : Component
{
	[Property]
	public Vector3 ScaleWishVelocity = new Vector3( 0, 1, 1 );

	/// <summary> Player input and enviroment interaction </summary>
	[Property]
	[Category( "Components" )]
	public PlayerController Controller { get; set; }

	/// <summary> Player input and enviroment interaction </summary>
	[Property]
	[Category( "Components" )]
	public Spawner PlatformSpawner { get; set; }

	[RequireComponent] private Health health { get; set; }
	private Transform startPostion;

	protected override void OnStart()
	{
		startPostion.Position = this.LocalPosition;
		startPostion.Rotation = this.LocalRotation;
		startPostion.Scale = this.LocalScale;
	}

	protected override void OnFixedUpdate()
	{
		var scaledWish = ScaleWishVelocity * Controller.WishVelocity;
		Controller.WishVelocity = scaledWish; // modify WishVelocity as suggested on https://github.com/Facepunch/sbox-public/issues/2777

		// kill the player if they're not in the restricted area
		health.ShouldDie(); // if this raises OnDeath, then PlatformSpawner.OnReset triggers teleportHome
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
		PlatformSpawner.OnReset += teleportHome;
	}

	protected override void OnDisabled()
	{
		base.OnDisabled();
		PlatformSpawner.OnReset -= teleportHome;
	}

	private void teleportHome()
	{
		Controller.LocalPosition = startPostion.Position;
		Controller.LocalRotation = startPostion.Rotation;
		Controller.LocalScale = startPostion.Scale;
	}
}
