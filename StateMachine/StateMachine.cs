using Godot;
using System;

public partial class StateMachine : Node
{
	[Export]
	public GenericState InitialState;

	private GenericState m_activeState;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void ParentReady()
	{
		foreach(GenericState state in this.GetChildren())
		{
			state.m_stateMachine = this;
		}
		m_activeState.Enter();
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
		m_activeState.HandleInput(@event);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		m_activeState.PhysicsUpdate((float)delta);
    }

	public override void _Process(double delta)
	{
		base._Process(delta);
		m_activeState.Update((float)delta);
	}

	public void TransitionTo(string stateName)
	{
		if(!HasNode(stateName))
		{  
			return; 
		}
		
		m_activeState.Exit();
		m_activeState = GetNode<GenericState>(stateName);
		m_activeState.Enter();
	}
}
