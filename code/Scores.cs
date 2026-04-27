using Sandbox;
using System.Threading.Tasks;

public sealed class Scores : Component
{
	private readonly string keyScore = "score";// Don't use "score_num" + DaysSince.ReleaseDate;, instead (eventually when it works) use https://sbox.game/feddas/infinaobby/stats/leaderboards/help/api

	/// <summary> The maximum z value the player has obtained this game session. Shown to the player with Code/ui/UiHud.razor </summary>
	[ReadOnly, Property] public float MaxScore { get; private set; } = float.MinValue;

	/// <summary> The maxium z value the player has obtained since last dying.  </summary>
	[ReadOnly, Property] public float MaxHeight { get; private set; } = float.MinValue;

	/// <summary> This value is shown to the player with Code/ui/UiHud.razor </summary>
	public string Instructions { get; private set; } = "Use cubes to jump higher than anyone else on the leaderboards.";

	protected override void OnStart()
	{
		MaxScore = (float)Sandbox.Services.Stats.GetLocalPlayerStats( "infinaobby" ).Get( keyScore ).Value;
		Log.Info( "stats has MaxScore of " + MaxScore );
		ScoreAchievement.MultiBox( MaxScore ); // retroactively unlock achievements created after player already satisfied them.

		// Hide instructions after game starts
		_ = RemoveInstructions();
	}

	private async Task RemoveInstructions()
	{
		await Task.Frame(); // Waiting for next frame needed for Time.Now to be set to something other than 0, so DelaySeconds will work

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
		if ( this.WorldPosition.z > MaxHeight )
		{
			MaxHeight = this.WorldPosition.z;

			if ( MaxHeight > MaxScore )
			{
				MaxScore = MaxHeight;

				// update leaderboard
				Sandbox.Services.Stats.SetValue( keyScore, MaxScore );
				//Log.Info( "set stats MaxScore to " + MaxScore );
				//DebugOverlay.Text( WorldPosition + Vector3.Up * 80f, MaxScore.ToString() );

				// update achievements that are height-score based
				ScoreAchievement.MultiBox( MaxHeight );
			}

		}
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
		GameManager.Instance?.OnReseting += OnReseting;
	}

	protected override void OnDisabled()
	{
		base.OnDisabled();
		GameManager.Instance?.OnReseting -= OnReseting;
	}

	private void OnReseting()
	{
		MaxHeight = float.MinValue;
	}
}
