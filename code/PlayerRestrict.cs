using Sandbox;

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

	protected override void OnFixedUpdate()
	{
		var scaledWish = ScaleWishVelocity * Controller.WishVelocity;
		Controller.WishVelocity = scaledWish; // modify WishVelocity as suggested on https://github.com/Facepunch/sbox-public/issues/2777

		System.Action goHome = teleportHome;
		health.ShouldDie( goHome );
	}

	private void teleportHome()
	{
		Controller.LocalPosition = startPostion.Position;
		Controller.LocalRotation = startPostion.Rotation;
		Controller.LocalScale = startPostion.Scale;
	}
}
