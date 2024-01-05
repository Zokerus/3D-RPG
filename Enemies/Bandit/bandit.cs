using Godot;
using System;
using System.Diagnostics;

public partial class bandit : CharacterBody3D
{
	private const float m_speed = 5.0f;
	private const float m_jumpVelocity = 4.5f;
    private const float m_characterRotationRate = 3 * Mathf.Pi;

    private NavigationAgent3D m_navigationAgent;

	private Node3D m_target;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
        m_navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = m_jumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		//Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		//Vector3 direction = (Transform.Basis * new Vector3(-inputDir.X, 0, -inputDir.Y)).Normalized();

		if (m_target != null)
		{
			Vector3 direction = (m_navigationAgent.GetNextPathPosition() - this.GlobalPosition).Normalized();
			if (direction != Vector3.Zero)
			{
                velocity.X = direction.X * m_speed;
				velocity.Z = direction.Z * m_speed;
			}
		}
		//else
		//{
		//	velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		//	velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		//}

		m_navigationAgent.Velocity = velocity;
		Velocity = velocity;
		MoveAndSlide();
	}

	public void TargetDetected(Node3D target)
	{
		m_target = target;
		m_navigationAgent.TargetPosition = target.GlobalPosition;
	}
}
