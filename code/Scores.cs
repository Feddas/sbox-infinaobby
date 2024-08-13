using Sandbox;

public sealed class Scores : Component
{
	/// <summary> The maximum z value the player has obtained. </summary>
	[ReadOnly, Property] private float maxHeight = float.MinValue;

	[RequireComponent] private Health Health { get; set; }

	protected override void OnUpdate()
	{
		if ( this.Transform.Position.z > maxHeight )
		{
			maxHeight = this.Transform.Position.z;

			// TODO: update UI and leaderboard
		}
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
		Health.OnDeath += CanDie_OnDeath;
	}

	protected override void OnDisabled()
	{
		base.OnDisabled();
		Health.OnDeath -= CanDie_OnDeath;
	}

	private void CanDie_OnDeath()
	{
		maxHeight = float.MinValue;
	}
}
