using Sandbox;
using static Sandbox.Package;

/// <summary>
/// Log the time in seconds between an object exiting the trigger then re-entering.
/// </summary>
public sealed class ClockExitEnter : Component, Component.ITriggerListener
{
	private float timeStart;
	private string lastResult;

	protected override void OnUpdate()
	{
		DebugOverlay.Text( WorldPosition + Vector3.Up * 80f, lastResult );
	}

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		float timeToJump = Time.Now - timeStart;

		lastResult = $"`{nameWithParent( other )}` took {timeToJump.ToString( "F3" )} to jump";
		Log.Info( lastResult );
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		timeStart = Time.Now;
		lastResult = "";

		Log.Info( $"{nameWithParent( other )} left {this.GameObject.Name}" );
	}

	private string nameWithParent( Collider other )
	{
		string parent = "";
		if ( other.GameObject.Parent != null )
		{
			parent = other.GameObject.Parent.Name + ".";
		}

		return parent + other.GameObject.Name;
	}
}
