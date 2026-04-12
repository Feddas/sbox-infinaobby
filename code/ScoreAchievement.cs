/// <summary>
/// Manages achievement progress for InfinaObby
/// InfinaObby achievements: https://sbox.game/feddas/infinaobby/service/achievements & https://services.facepunch.com/sbox/achievement/list?package=feddas.infinaobby
/// </summary>
public class ScoreAchievement
{
	/// <summary> matches name on https://sbox.game/feddas/infinaobby/service/achievements </summary>
	private const string namedMultiBox = "multi_box";

	/// <summary> If it's the players first time reaching a height over 400, give the player the "multi_box" achievement. </summary>
	/// <remarks> "multi_box" is a manual achievement. It's a better fit for a progression based achievement.
	/// Shawn was unable to figure out how to get progression based achievements to work.
	/// Shawn was unable to figure out how to modify the achievement to add a description or score as shown on https://services.facepunch.com/sbox/achievement/list?package=facepunch.testbed / https://sbox.game/dev/doc/services/achievements </remarks>
	public static void MultiBox( float height )
	{
		// verify player is eligible for the achievement
		if ( height < 400 ) // this is the only place the height of 400 is specified. other than the name on
		{
			return;
		}

		// unlock
		unlock( namedMultiBox );
	}

	private static void unlock( string achievementName )
	{
		var achievement = Sandbox.Services.Achievements.All.First( a => a.Name == achievementName );

		if ( achievement.IsUnlocked )
		{
			Log.Info( $"Achievement {achievementName} already unlocked." );
			return;
		}
		else
		{
			Sandbox.Services.Achievements.Unlock( achievementName );
			Log.Info( $"Achievement {achievementName} unlocked? {achievement.IsUnlocked}/{Sandbox.Services.Achievements.All.First( a => a.Name == achievementName ).IsUnlocked}" );
		}
	}
}
