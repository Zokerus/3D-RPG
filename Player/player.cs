using Godot;
using System;
using System.Diagnostics;

public partial class player : CharacterBody3D
{
	[Export]
	public camera_controller mainCamera;

	private AnimationTree m_animationTree;

	private const float m_speed = 5.0f;
	private const float m_characterRotationRate = 3*Mathf.Pi;
	private const float m_jumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float m_gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
        if (mainCamera == null) 
		{
            GetTree().Quit();
        }

		m_animationTree = GetNode<AnimationTree>("AnimationTree");
    }

    public override void _UnhandledInput(InputEvent @event)
	{
		Vector3 velocity = Velocity;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = m_jumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("Left", "Right", "Backward", "Forward");
		m_animationTree.Set("parameters/Locomotion/blend_position", inputDir);
		Debug.Print(inputDir.ToString());
		//Vector3 direction = (mainCamera.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		//if (direction != Vector3.Zero)
		//{
		//	velocity.X = direction.X * m_speed;
		//	velocity.Z = direction.Z * m_speed;
		//}
		//else
		//{
		//	velocity.X = Mathf.MoveToward(Velocity.X, 0, m_speed);
		//	velocity.Z = Mathf.MoveToward(Velocity.Z, 0, m_speed);
		//}

		Velocity = velocity;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= m_gravity * (float)delta;

		//if (velocity.X != 0.0f || velocity.Z != 0.0f)
		if ((Vector2)m_animationTree.Get("parameters/Locomotion/blend_position") != Vector2.Zero)
		{
			float rotationAngle = this.Transform.Basis.Z.SignedAngleTo(mainCamera.Transform.Basis.Z, Vector3.Up);
			this.RotateY(Mathf.Sign(rotationAngle) * Mathf.Min(m_characterRotationRate * (float)delta, Mathf.Abs(rotationAngle)));
		}

        Velocity = velocity;
		MoveAndSlide();
		mainCamera.GlobalPosition = this.GlobalPosition;
	}
}
