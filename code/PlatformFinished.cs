using Sandbox;

/// <summary>
/// Disposes of platforms
/// https://wiki.facepunch.com/sbox/Triggers
/// </summary>
public sealed class PlatformFinished : Component, Component.ITriggerListener
{
	/// <summary> Height of lowest current cube. Kills the player if they fall off the cube. </summary>
	[Property] public float MinZValue { get; set; } = -10; // -10 to account for jumping causing players Z position to go beneath 0.

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		//Log.Info( $"{other.GameObject.Name} entered {this.GameObject.Name}" );

		// Do final tasks of the platform. platforms are the only objects that should contain <Incoming>
		if ( other.Components.Get<Incoming>() != null )
		{
			// If player is under the MinZValue of the last disposed platform, they are killed.
			MinZValue = other.WorldPosition.z;

			// dispose of the platform
			other.GameObject.Destroy();
		}
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		//Log.Info( $"{other.GameObject.Name} left {this.GameObject.Name}" );
	}
}
