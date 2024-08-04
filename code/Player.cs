
using Sandbox.Citizen;

/// <summary> Followed tutorial at https://www.youtube.com/watch?v=uBmCN1S8pFc&list=PLIcPBTNc7_9oFEEoHSCuPrdGQnU27yLuj&index=3 </summary>
public sealed class Player : Component
{
	[Property, Category( "component" )] public GameObject Camera { get; set; }
	[Property, Category( "component" )] public CharacterController Controller { get; set; }
	[Property, Category( "component" )] public CitizenAnimationHelper Animator { get; set; }

	/// <summary> How many units of length you can walk per second </summary>
	[Property, Category( "stats" ), Range(0, 400, 1)] public float WalkSpeed { get; set; } = 120f;
	/// <summary> How many units of length you can run per second </summary>
	[Property, Category( "stats" ), Range( 0, 800, 1 )] public float RunSpeed { get; set; } = 250f;
	/// <summary> How many up units you can jump per second </summary>
	[Property, Category( "stats" ), Range( 0, 1000, 10 )] public float JumpStrength { get; set; } = 400f;

	/// <summary> Where the camera rotates around and aim originates from </summary>
	[Property] public Vector3 EyePosition { get; set; }

	private Angles EyeAngles;
	private Transform initialCamera;

	protected override void OnStart()
	{
		initialCamera = Camera.Transform.Local;
		// Log.Info( " initialCameraPitch " + initialCamera.Rotation.Pitch() );
	}

	protected override void OnUpdate()
	{
		// get pointer movement delta; mouse or analog stick movement since last frame
		EyeAngles += Input.AnalogLook;
		EyeAngles.pitch = EyeAngles.pitch.Clamp(-80f, 80f);

		// rotate character rig
		Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );

		// move follow camera
		Camera.Transform.Local = initialCamera.RotateAround(EyePosition, EyeAngles.WithYaw(0));
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
		var wishSpeed = Input.Down( "Run" ) ? RunSpeed : WalkSpeed;
		var wishVelocity = Input.AnalogMove.Normal * wishSpeed * Transform.Rotation;
		Controller.Accelerate( wishVelocity );

		if ( Controller.IsOnGround )
		{
			Controller.Acceleration = 10f;
			Controller.ApplyFriction( 5f );

			if ( Input.Pressed("Jump"))
			{
				Controller.Punch( Vector3.Up * JumpStrength );
				Animator.TriggerJump();
			}
		}
		else
		{
			Controller.Acceleration = 5f;
			Controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
		}

		Controller.Move();
		Animator.IsGrounded = Controller.IsOnGround;
		Animator.WithVelocity( Controller.Velocity );
	}

	protected override void DrawGizmos()
	{
		// show, in editor, the position the follow camera will rotate around
		Gizmo.Draw.LineSphere( EyePosition, 10f );
	}
}
