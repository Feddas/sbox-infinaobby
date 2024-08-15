using Sandbox;

public sealed class Incoming : Component
{
	public Color Color
	{
		get { return renderer.Tint; }
		set { renderer.Tint = value; }
	}

	public float SecondsToTraverse
	{
		get { return secondsToTraverse; }
		set
		{
			secondsToTraverse = value;
			speed = distanceTraversed / secondsToTraverse;
		}
	}
	private float secondsToTraverse = 4;

	[RequireComponent] private ModelRenderer renderer { get; set; }

	private float speed;
	private float distanceTraversed = 200;

	protected override void OnStart()
	{
		base.OnStart();
	}

	protected override void OnUpdate()
	{
		this.Transform.Position += Vector3.Backward * Time.Delta * speed;
		if ( this.Transform.Position.x < 0 )
		{
			renderer.Tint = renderer.Tint.WithAlpha( 0.3f );
		}
	}
}
