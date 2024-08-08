using Sandbox;

public sealed class Incoming : Component
{
	private float speed = 10;
	private ModelRenderer renderer;
	protected override void OnStart()
	{
		base.OnStart();
		renderer = this.Components.Get<ModelRenderer>();
		renderer.Tint = Color.Random;
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
