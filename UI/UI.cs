using Godot;
using System;

public partial class UI : Control
{
	private TextureRect m_lockOnDot;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		m_lockOnDot = GetNode<TextureRect>("LockOnDot");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void LockOnTarget(Area3D target)
	{
		if (target == null) 
		{
			m_lockOnDot.Position = GetViewport().GetCamera3D().UnprojectPosition(target.GlobalPosition);
			m_lockOnDot.Show();
		}
		else
		{
			m_lockOnDot.Hide();
		}
	}
}
