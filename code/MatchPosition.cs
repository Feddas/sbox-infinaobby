using Sandbox;

public sealed class MatchPosition : Component
{
	[Property] public GameObject Target { get; set; }
	[Property] public Vector3 Multiplier { get; set; } = Vector3.One;
	[Property] public Vector3 Smoothing { get; set; } = Vector3.Zero;

	protected override void OnUpdate()
	{
		Vector3 finalPosition = this.Transform.Position;

		//if ( Multiplier.x != 0 )
		//{
		//	if ( Smoothing.x == 0 )
		//	{
		//		finalPosition.x = this.Target.Transform.Position.x * Multiplier.x;
		//	}
		//	else
		//	{
		//		finalPosition.x = finalPosition.x.LerpTo( this.Target.Transform.Position.x * Multiplier.x, Time.Delta / Smoothing.x, clamp: true );
		//	}
		//}
		//if ( Multiplier.y != 0 )
		//{
		//	if ( Smoothing.y == 0 )
		//	{
		//		finalPosition.y = this.Target.Transform.Position.y * Multiplier.y;
		//	}
		//	else
		//	{
		//		finalPosition.y = finalPosition.y.LerpTo( this.Target.Transform.Position.y * Multiplier.y, Time.Delta / Smoothing.y, clamp: true );
		//	}
		//}
		//if ( Multiplier.z != 0 )
		//{
		//	if ( Smoothing.z == 0 )
		//	{
		//		finalPosition.z = this.Target.Transform.Position.z * Multiplier.z;
		//	}
		//	else
		//	{
		//		finalPosition.z = finalPosition.z.LerpTo( this.Target.Transform.Position.z * Multiplier.z, Time.Delta / Smoothing.z, clamp: true );
		//	}
		//}
		finalPosition.x = matchUsing( this.Transform.Position.x, this.Target.Transform.Position.x, Multiplier.x, Smoothing.x );
		finalPosition.y = matchUsing( this.Transform.Position.y, this.Target.Transform.Position.y, Multiplier.y, Smoothing.y );
		finalPosition.z = matchUsing( this.Transform.Position.z, this.Target.Transform.Position.z, Multiplier.z, Smoothing.z );

		this.Transform.Position = finalPosition;
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
