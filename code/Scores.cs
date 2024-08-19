using Sandbox;

public sealed class Scores : Component
{
	/// <summary> The maximum z value the player has obtained. </summary>
	[ReadOnly, Property] public float MaxHeight { get; private set; } = float.MinValue;
	[ReadOnly, Property] public float MaxScore { get; private set; } = float.MinValue;

	[RequireComponent] private Health Health { get; set; }

	protected override void OnUpdate()
	{
		if ( this.Transform.Position.z > MaxHeight )
		{
			MaxHeight = this.Transform.Position.z;

			if ( MaxHeight > MaxScore )
			{
				MaxScore = MaxHeight;
			}

			// TODO: update leaderboard
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
		MaxHeight = float.MinValue;
	}
}
