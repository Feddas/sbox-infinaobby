using Sandbox;

public sealed class WasdMovement : Component
{
	public float speed = 100;
	protected override void OnUpdate()
	{
		Vector3 direction = Vector3.Zero;
		if ( Input.Down( "forward" ) )
		{
			direction += Vector3.Forward;
		}
		if ( Input.Down( "backward" ) )
		{
			direction += Vector3.Backward;
		}
		if ( Input.Down( "left" ) )
		{
			direction += Vector3.Left;
		}
		if ( Input.Down( "right" ) )
		{
			direction += Vector3.Right;
		}


		Transform.Position += direction.Normal * Time.Delta * speed;
	}
}
