using Godot;
using System;
using System.Net.Http;
using System.Collections.Generic;

public partial class SmState : Node
{
	private bool _hasBeenInitialized = false;
	private bool _onUpdateHasFired = false;
	public StateMachine Sm;

	[Signal] public delegate void StateStart();

	[Signal] public delegate void StateUpdated();

	[Signal] public delegate void StateExit();

	public virtual void OnStart(Dictionary<string, object> message)
	{
		EmitSignal(nameof(StateStart));
		_hasBeenInitialized = true;
	}

	public virtual void OnUpdate()
	{
		if (!_hasBeenInitialized) return;

		EmitSignal(nameof(StateUpdated));
		_onUpdateHasFired = true;
	}
	
	public virtual void OnExit(string nextState)
	{
		if (!_hasBeenInitialized) return;
		
		EmitSignal(nameof(StateExit));
		_hasBeenInitialized = false;
		_onUpdateHasFired = false;
	}
}
