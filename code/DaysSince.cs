using System;

namespace Sandbox
{
	internal class DaysSince
	{
		/// <summary> Day the game was released </summary>
		static public readonly DateTime ReleaseDate = new DateTime( 2024, 8, 19 );

		/// <summary> Count of how many days from now since the game was released </summary>
		static public int Release()
		{
			// Calculate the difference in days
			TimeSpan difference = DateTime.Now.ToUniversalTime() - ReleaseDate;
			int daysSince = difference.Days;

			// Output the result
			Log.Info( $"Days since {ReleaseDate.ToShortDateString()}: {daysSince}" );

			return daysSince;
		}
	}
}
