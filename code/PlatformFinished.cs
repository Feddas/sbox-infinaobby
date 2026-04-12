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
		//Log.Info( $"{other.GameObject.Name} entered {this.GameObject.Name}" ); // Note: player doesn't have a collider on them. They never trigger this

		// Disposed Incoming platform sets the MinZValue. If player is under MinZValue, they are killed.
		if ( other.Components.Get<Incoming>() != null )
		{
			MinZValue = other.WorldPosition.z;
		}
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		//Log.Info( $"{other.GameObject.Name} left {this.GameObject.Name}" );
	}
}
