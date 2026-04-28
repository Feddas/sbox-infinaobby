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

	[RequireComponent] private Health health { get; set; }
	private Transform startPostion;

	protected override void OnStart()
	{
		startPostion.Position = this.LocalPosition;
		startPostion.Rotation = this.LocalRotation;
		startPostion.Scale = this.LocalScale;
	}

	//protected override void OnUpdate()
	//{
	//  // show x-velocity as UI over players head.
	//	DebugOverlay.Text( WorldPosition + Vector3.Up * 80f, $"{Controller.Velocity.x.ToString("F0")}" );
	//}

	protected override void OnFixedUpdate()
	{
		if ( GameManager.Instance.CurrentState != GameManager.GameState.PlayerAlive )
		{
			return;
		}

		var scaledWish = ScaleWishVelocity * Controller.WishVelocity;
		if ( WorldPosition.x < 0 ) // gravitate back towards centerline
		{
			scaledWish.x = 1; // 1 is how fast the player returns to the 0,0,0 line
		}
		Controller.WishVelocity = scaledWish; // modify WishVelocity as suggested on https://github.com/Facepunch/sbox-public/issues/2777

		// kill the player if they're not in the restricted area
		health.ShouldDie(); // if this raises OnDeath, then PlatformSpawner.OnReset triggers teleportHome
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
		GameManager.Instance?.OnReseting += OnReseting;
		GameManager.Instance?.OnReset += teleportHome;
	}

	protected override void OnDisabled()
	{
		base.OnDisabled();
		GameManager.Instance?.OnReseting -= OnReseting;
		GameManager.Instance?.OnReset -= teleportHome;
	}

	private void OnReseting()
	{
		Controller.UseInputControls = false;
		Controller.WishVelocity = 0; // Remove velocity from input.
	}

	private void teleportHome()
	{
		Controller.WishVelocity = 0; // If player died from a push, they may have continued to be pushed after OnReseting starting.
		Controller.LocalPosition = startPostion.Position;
		Controller.LocalRotation = startPostion.Rotation;
		Controller.LocalScale = startPostion.Scale;
		Controller.UseInputControls = true;

		GameManager.Instance.ChangeState( GameManager.GameState.PlayerAlive );
	}
}
