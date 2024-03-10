using Godot;
using System;
using System.Diagnostics;


public partial class bandit : CharacterBody3D
{
	private const float m_speed = 2.0f;
	private const float m_jumpVelocity = 4.5f;
    private const float m_characterRotationRate = 3 * Mathf.Pi;

    private NavigationAgent3D m_navigationAgent;

	private Vector3 m_target;

	private Vector3[] m_waypoints = new Vector3[3];
	private int m_index = 0;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
        m_navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		m_waypoints[0] = new Vector3(11, 0, -10);
		m_waypoints[1] = new Vector3(-10, 0, -10);
		m_waypoints[2] = new Vector3(0, 0, 13);
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

		if (m_target != Vector3.Zero)
		{
			Vector3 tempPos = m_navigationAgent.GetNextPathPosition();

            Vector3 direction = (tempPos - this.Position);
			direction = direction.Normalized();
            //Debug.Print(direction.ToString());
            if (direction != Vector3.Zero)
			{
                float rotationAngle = this.Transform.Basis.Z.SignedAngleTo(direction, Vector3.Up);
                this.RotateY(Mathf.Sign(rotationAngle) * Mathf.Min(m_characterRotationRate * (float)delta, Mathf.Abs(rotationAngle)));
                velocity.X = direction.X * m_speed;
				velocity.Z = direction.Z * m_speed;
				//Debug.Print(rotationAngle.ToString() + " : " + direction.ToString());
			}
		}
		else
		{
			OnTargetReached();
        }
		//else
		//{
		//	velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		//	velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		//}

		//m_navigationAgent.Velocity = velocity;
		//Velocity = velocity;
		MoveAndSlide();
	}

	public void TargetDetected(Node3D target)
	{
		//m_target = target;
		//m_navigationAgent.TargetPosition = target.GlobalPosition;
	}

	public void OnTargetReached()
	{
        m_target = m_waypoints[m_index];
        m_navigationAgent.TargetPosition = m_target;
		m_index++;
		m_index %= m_waypoints.Length;
    }
}
