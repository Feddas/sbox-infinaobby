using Sandbox;

public sealed class Incoming : Component
{
	public Color Color
	{
		get { return renderer.Tint; }
		set { renderer.Tint = value; }
	}

	/// <summary> units this platform will traverse per second </summary>
	public float Speed { get; set; }

	[RequireComponent] private ModelRenderer renderer { get; set; }

	protected override void OnStart()
	{
		base.OnStart();
	}

	protected override void OnUpdate()
	{
		this.WorldPosition += Vector3.Backward * Time.Delta * Speed;

		// Show the platform is about to disappear (by running into gameobject Finished's PlatformFinished.cs collider.
		if ( this.WorldPosition.x < 0 )
		{
			renderer.Tint = renderer.Tint.WithAlpha( 0.3f );
		}
	}
}
