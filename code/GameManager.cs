using Sandbox;
using System;

public sealed class GameManager : Component
{
	public static GameManager Instance
	{
		get
		{
			return instance;
		}
	}
	private static GameManager instance;

	public event Action OnPaused, OnPlayerAlive, OnPlayerDied, OnReseting, OnReset;

	public enum GameState
	{
		/// <summary> Player is in a menu </summary>
		Paused,

		/// <summary> Player has at least 1 hit point </summary>
		PlayerAlive,

		/// <summary> Player is at 0 hit points </summary>
		PlayerDied,

		/// <summary> Cleaning up the scene to prepare for the player to respawn </summary>
		Reseting,

		/// <summary> Scene is ready for the player to be respawned </summary>
		Reset,
	}
	[Property] public GameState CurrentState { get; private set; }

	protected override void OnAwake()
	{
		OnEnabled();
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
		if ( instance == null )
		{
			instance = this;
		}
		else if ( instance != this )
		{
			new System.Exception( this.GameObject.Name + " violated the GameManager singleton. Already on " + instance.GameObject.Name );
			Destroy();
		}
	}

	protected override void OnDisabled()
	{
		base.OnDisabled();
		instance = null;
	}

	public void ChangeState( GameState newState )
	{
		// guard clause. There was no change in the state.
		if ( newState == CurrentState )
		{
			return;
		}

		// transition to new state
		if ( CurrentState == GameState.Paused ) // game will be changed out of paused state
		{
			Scene.TimeScale = 1;
		}
		Log.Info( Time.Now.ToString( "F2" ) + $" {CurrentState} -> {newState}" );
		CurrentState = newState;

		// notify subscribers of new state
		switch ( newState )
		{
			case GameState.Paused:
				Scene.TimeScale = 0;
				OnPaused?.Invoke();
				break;
			case GameState.PlayerAlive:
				OnPlayerAlive?.Invoke();
				break;
			case GameState.PlayerDied:
				OnPlayerDied?.Invoke();
				break;
			case GameState.Reseting:
				OnReseting?.Invoke();
				break;
			case GameState.Reset:
				OnReset?.Invoke();
				break;
			default:
				break;
		}
	}
}
