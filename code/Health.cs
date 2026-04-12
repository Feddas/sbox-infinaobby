using Sandbox;
using Sandbox.Citizen;
using System;

/// <summary> This object can die </summary>
public sealed class Health : Component
{
	public event Action OnDeath;

	/// <summary> If the gameobject's x value is less, kill the object </summary>
	[Property, Range( -400, 400 ), Step( 25 )] public float minXValue { get; set; } = -25f;

	/// <summary> Height of lowest current cube. Kills the player if they fall off the cube. </summary>
	[Property] public PlatformFinished platforms { get; set; }

	/// <summary> Checks if health should be changed to a death state. </summary>
	public async void ShouldDie( Action teleportHome )
	{
		// if hasn't died, return
		if ( this.WorldPosition.x >= minXValue && this.WorldPosition.z >= platforms.MinZValue )
		{
			return;
		}

		// else, do death
		string deathReason = WorldPosition.x < minXValue
			? $"Pushed {WorldPosition.x.ToString( "F2" )} < {minXValue}"
			: $"Fell {WorldPosition.z.ToString( "F2" )} < {platforms.MinZValue.ToString( "F2" )}";
		Log.Info( $"Player DIED. {deathReason}" );
		teleportHome();

		platforms.MinZValue = -10; // -10 to account for jumping causing players Z position to go beneath 0.

		// wait 2 frames for teleport to finish
		await Task.Frame();
		await Task.Frame();

		OnDeath?.Invoke();
	}
}
