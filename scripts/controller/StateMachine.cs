using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class StateMachine : Node
{
    [Signal] public delegate void PreStart();
    [Signal] public delegate void PostStart();
    [Signal] public delegate void PreExit();
    [Signal] public delegate void PostExit();

    public Dictionary<string, SmState> States;

    public string CurrentStateName { get; private set; }
    public string PreviousStateName { get; private set; }
    
    protected SmState CurrentState;

    public override void _Ready()
    {
        base._Ready();
        States = new Dictionary<string, SmState>();
        foreach(var state in GetChildren().OfType<SmState>().ToList())
        {
            States.Add(state.GetType().ToString(), state);
            state.Sm = this;
        }
    }

    private void SetState(SmState newState, Dictionary<string, object> message)
    {
        if (newState is null) return;

        if (CurrentState is not null)
        {
            EmitSignal(nameof(PreExit));
            CurrentState.OnExit(newState.GetType().ToString());
            EmitSignal(nameof(PostExit));
        }

        PreviousStateName = CurrentStateName;
        CurrentStateName = newState.GetType().ToString();

        CurrentState = newState;
        EmitSignal(nameof(PreStart));
        CurrentState.OnStart(message);
        EmitSignal(nameof(PostStart));
        CurrentState.OnUpdate();
    }

    public void ChangeState(string stateName, Dictionary<string, object> message = null)
    {
        if (States.ContainsKey(stateName))
        {
            SetState(States[stateName], message);
        }
        else
        {
            GD.PrintErr("Could not find state {stateName} in StateMachine States list");
        }
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (CurrentState is null) return;
        
        CurrentState._PhysicsProcess(delta);
    }
}
