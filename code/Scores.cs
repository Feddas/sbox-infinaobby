using Sandbox;
using System.Threading.Tasks;

public sealed class Scores : Component
{
	private readonly string keyScore = "score";// Don't use "score_num" + DaysSince.ReleaseDate;, instead (eventually when it works) use https://sbox.game/feddas/infinaobby/stats/leaderboards/help/api

	/// <summary> The maximum z value the player has obtained. </summary>
	[ReadOnly, Property] public float MaxHeight { get; private set; } = float.MinValue;
	[ReadOnly, Property] public float MaxScore { get; private set; } = float.MinValue;

	// sbox require component: https://github.com/Facepunch/sbox-issues/issues/4659
	[RequireComponent] private Health Health { get; set; }

	public string Instructions { get; private set; } = "Use cubes to jump higher than anyone else on the leaderboards.";

	protected override void OnStart()
	{
		MaxScore = (float)Sandbox.Services.Stats.GetLocalPlayerStats( "infinaobby" ).Get( keyScore ).Value;
		Log.Info( "stats has MaxScore of " + MaxScore );
		_ = RemoveInstructions( 1600 );
	}

	private async Task RemoveInstructions( float waitSeconds )
	{
		await Task.Frame(); // needed for Time.Now to be set to something other than 0, so DelaySeconds will work

		Instructions = "Use cubes to jump higher than anyone else on the leaderboards.";
		await Task.DelaySeconds( 4f );
		while ( Instructions.Length > 1 )
		{
			await Task.DelaySeconds( .02f );

			Instructions = Instructions.Remove( 0, 1 );
		}
		Instructions = " ";
	}

	protected override void OnUpdate()
	{
		if ( this.Transform.Position.z > MaxHeight )
		{
			MaxHeight = this.Transform.Position.z;

			if ( MaxHeight > MaxScore )
			{
				MaxScore = MaxHeight;

				// update leaderboard
				Sandbox.Services.Stats.SetValue( keyScore, MaxScore );
				Log.Info( "set stats MaxScore to " + MaxScore );
			}

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
