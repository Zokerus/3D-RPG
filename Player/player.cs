using Godot;
using System;
using System.Diagnostics;

public partial class player : CharacterBody3D
{
	[Export]
	public camera_controller mainCamera;

	private const float Speed = 5.0f;
	private const float characterRotationRate = 3*Mathf.Pi;
	private const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
        if (mainCamera == null) 
		{
            GetTree().Quit();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
	{
		Vector3 velocity = Velocity;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("Left", "Right", "Forward", "Backward");
		Vector3 direction = (mainCamera.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		if (velocity.X != 0.0f || velocity.Z != 0.0f)
		{
			float rotationAngle = this.Transform.Basis.Z.SignedAngleTo(mainCamera.Transform.Basis.Z, Vector3.Up);
			this.RotateY(Mathf.Sign(rotationAngle) * Mathf.Min(characterRotationRate * (float)delta, Mathf.Abs(rotationAngle)));
		}

        Velocity = velocity;
		MoveAndSlide();
		mainCamera.GlobalPosition = this.GlobalPosition;
	}
}
