using Sandbox;

/// <summary>
/// Disposes of platforms
/// https://wiki.facepunch.com/sbox/Triggers
/// </summary>
public sealed class PlatformFinished : Component, Component.ITriggerListener
{
	/// <summary> Height of lowest current cube. Kills the player if they fall off the cube. </summary>
	[Property] public float MinZValue { get; set; }

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if ( other.Components.Get<Incoming>() != null )
		{
			//Log.Info( $"{other.GameObject.Name} entered {this.GameObject.Name}" );
			MinZValue = other.Transform.Position.z;
		}
		// TODO: player doesn't have a collider on them. They never trigger this
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		//Log.Info( $"{other.GameObject.Name} left {this.GameObject.Name}" );
	}
}
