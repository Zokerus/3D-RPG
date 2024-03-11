using Godot;
using System;

public partial class UI : Control
{
	private TextureRect m_lockOnDot;
	private Area3D m_lockedTarget;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		m_lockOnDot = GetNode<TextureRect>("LockOnDot");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (m_lockOnDot.Visible) 
		{
            m_lockOnDot.Position = GetViewport().GetCamera3D().UnprojectPosition(m_lockedTarget.GlobalPosition);
        }
	}

	public void LockOnTarget(Area3D target)
	{
		m_lockedTarget = target;
		if (target != null) 
		{
			m_lockOnDot.Position = GetViewport().GetCamera3D().UnprojectPosition(m_lockedTarget.GlobalPosition);
			m_lockOnDot.Show();
		}
		else
		{
			m_lockOnDot.Hide();
		}
	}
}
