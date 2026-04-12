using Sandbox;

public sealed class MatchPosition : Component
{
	[Property] public GameObject Target { get; set; }
	[Property] public Vector3 Multiplier { get; set; } = Vector3.One;
	[Property] public Vector3 Smoothing { get; set; } = Vector3.Zero;

	protected override void OnUpdate()
	{
		Vector3 finalPosition = this.WorldPosition;

		finalPosition.x = matchUsing( this.WorldPosition.x, this.Target.WorldPosition.x, Multiplier.x, Smoothing.x );
		finalPosition.y = matchUsing( this.WorldPosition.y, this.Target.WorldPosition.y, Multiplier.y, Smoothing.y );
		finalPosition.z = matchUsing( this.WorldPosition.z, this.Target.WorldPosition.z, Multiplier.z, Smoothing.z );

		this.WorldPosition = finalPosition;
	}

	private float matchUsing( float currentPosition, float targetPosition, float multiplier, float smoothing )
	{
		if ( multiplier != 0 )
		{
			if ( smoothing == 0 )
			{
				return targetPosition * multiplier;
			}
			else
			{
				return currentPosition.LerpTo( targetPosition * multiplier, Time.Delta / smoothing, clamp: true );
			}
		}

		return currentPosition;
	}
}
