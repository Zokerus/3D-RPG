using Godot;
using System;

public abstract partial class GenericState : Node
{
	public StateMachine m_stateMachine = null;

	public abstract void Enter();
	public abstract void Update(float delta);
	public abstract void PhysicsUpdate(float delta);
	public abstract void HandleInput(InputEvent @event);
	public abstract void Exit();

}
