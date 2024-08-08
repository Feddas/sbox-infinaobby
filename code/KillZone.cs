using Sandbox;

/// <summary>
/// Kill the player if they touch this gameobject
/// https://wiki.facepunch.com/sbox/Triggers
/// </summary>
public sealed class KillZone : Component, Component.ITriggerListener
{
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		Log.Info( $"{other.GameObject.Name} entered {this.GameObject.Name}" );

		// TODO: player doesn't have a collider on them. They never trigger this
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		Log.Info( $"{other.GameObject.Name} left {this.GameObject.Name}" );
	}
}
