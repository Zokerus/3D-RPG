using Godot;
using System;
using System.Diagnostics;

public partial class player : CharacterBody3D
{
	[Export]
	public camera_controller mainCamera;

	private AnimationTree m_animationTree;

    private Vector3 m_movementDirection = Vector3.Zero;

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
        Vector2 inputDir = Input.GetVector("Left", "Right", "Forward", "Backward");
        m_movementDirection = (mainCamera.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        m_animationTree.Set("parameters/Movement/blend_amount", -1 + inputDir.Length()); //Input.GetVector ist by default limited to the length of 1

        Velocity = velocity;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= m_gravity * (float)delta;

		//Rotate the Player according to camera rotation and movement direction (input)
		if (m_movementDirection != Vector3.Zero)
		{
			float rotationAngle = this.Transform.Basis.Z.SignedAngleTo(m_movementDirection, Vector3.Up);
			this.RotateY(Mathf.Sign(rotationAngle) * Mathf.Min(m_characterRotationRate * (float)delta, Mathf.Abs(rotationAngle)));
		}

		// Get position change based an animation (root motion)
        Quaternion currentRotation = Transform.Basis.GetRotationQuaternion();
        velocity = DivideByFloat(currentRotation.Normalized() * m_animationTree.GetRootMotionPosition(), (float)delta);

        Velocity = velocity;
		MoveAndSlide();
		mainCamera.GlobalPosition = this.GlobalPosition;
	}

    /// <summary>
    /// Divide a Vector3 by a scalar of type float
    /// </summary>
    /// <param name="left">Vector3 parameter</param>
    /// <param name="right">float scalar parameter</param>
    /// <returns>New Vector3 left/right</returns>
    private Vector3 DivideByFloat(Vector3 left, float right)
    {
        return new Vector3(left.X / right, left.Y / right, left.Z / right);
    }
}
