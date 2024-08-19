using Sandbox;
using Sandbox.Citizen;
using System;

/// <summary> This object can die </summary>
public sealed class Health : Component
{
	public event Action OnDeath;

	/// <summary> If the gameobject's x value is less, kill the object </summary>
	[Property, Range( -400, 400, 25 )] public float minXValue { get; set; } = -25f;

	/// <summary> Height of lowest current cube. Kills the player if they fall off the cube. </summary>
	[Property] public PlatformFinished platforms { get; set; }

	public CitizenAnimationHelper Animator { get; set; }

	private Transform startPostion;

	protected override void OnStart()
	{
		startPostion.Position = this.Transform.LocalPosition;
		startPostion.Rotation = this.Transform.LocalRotation;
		startPostion.Scale = this.Transform.LocalScale;
	}

	/// <summary> Checks if health should be changed to a death state. </summary>
	public async void ShouldDie()
	{
		// if hasn't died, return
		if ( this.Transform.Position.x >= minXValue && this.Transform.Position.z >= platforms.MinZValue )
		{
			return;
		}

		// else, do death
		teleportHome();
		if ( this.Animator != null )
		{
			Animator.SpecialMove = CitizenAnimationHelper.SpecialMoveStyle.Slide;
		}

		platforms.MinZValue = 0;

		// wait 2 frames for teleport to finish
		await Task.Frame();
		await Task.Frame();

		OnDeath?.Invoke();
	}

	private void teleportHome()
	{
		this.Transform.LocalPosition = startPostion.Position;
		this.Transform.LocalRotation = startPostion.Rotation;
		this.Transform.LocalScale = startPostion.Scale;
	}
}
