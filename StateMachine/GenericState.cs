using Godot;
using System;

public abstract partial class GenericState : Node
{
	public StateMachine m_stateMachine = null;
	protected CharacterBody3D m_characterBody;
    protected AnimationTree m_animationNode;

    public void Init(CharacterBody3D characterBody, AnimationTree animationTree)
	{  
		m_characterBody = characterBody;
		m_animationNode = animationTree;
	}
	public abstract void Enter();
	public abstract void Update(float delta);
	public abstract void PhysicsUpdate(float delta);
	public abstract void HandleInput(InputEvent @event);
	public abstract void Exit();

}
