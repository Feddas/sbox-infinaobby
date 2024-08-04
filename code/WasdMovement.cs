//using Sandbox;

///// <summary> Created from official facepunch tutorial
//// https://docs.facepunch.com/s/sbox-dev/doc/dev-preview-first-steps-7IyiSplYmn
//// https://docs.facepunch.com/s/sbox-dev/doc/cheat-sheet-CH6MPz8N2j
//// https://wiki.facepunch.com/sbox/Coding-Cheat-Sheet 
//// https://wiki.facepunch.com/sbox/CSharp_Learning_Resources
//// https://wiki.facepunch.com/sbox/Games/Publish
//// https://sbox.game/~create
//// https://sbox.game/api
//// </summary>
//public sealed class WasdMovement : Component
//{
//	public float speed = 100;
//	protected override void OnUpdate()
//	{
//		Vector3 direction = Vector3.Zero;
//		if ( Input.Down( "forward" ) )
//		{
//			direction += Vector3.Forward;
//		}
//		if ( Input.Down( "backward" ) )
//		{
//			direction += Vector3.Backward;
//		}
//		if ( Input.Down( "left" ) )
//		{
//			direction += Vector3.Left;
//		}
//		if ( Input.Down( "right" ) )
//		{
//			direction += Vector3.Right;
//		}


//		Transform.Position += direction.Normal * Time.Delta * speed;
//	}
//}
