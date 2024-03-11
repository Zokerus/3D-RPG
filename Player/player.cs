using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class player : CharacterBody3D
{
	[Export]
	public camera_controller mainCamera;
    [Export]
    public UI GUI;

	private AnimationTree m_animationTree;

    private Vector3 m_movementDirection = Vector3.Zero;

	private const float m_characterRotationRate = 4*Mathf.Pi;
	private const float m_jumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float m_gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private bool m_walkFactor = false;
    private bool m_completeTurn = false;
    private Vector3 m_targetLookDirection = Vector3.Zero;
    private List<Area3D> m_targetList = new List<Area3D>();
    private Area3D m_lockedTarget = null;

    public override void _Ready()
    {
        if (mainCamera == null) 
		{
            GetTree().Quit();
        }

		m_animationTree = GetNode<AnimationTree>("AnimationTree");
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
		Vector3 velocity = Velocity;

		// Handle Jump.
		if (@event.IsActionPressed("Jump") && IsOnFloor())
			velocity.Y = m_jumpVelocity;


        if (@event.IsActionPressed("Run", true) && IsOnFloor() && m_movementDirection != Vector3.Zero)
        {
            m_walkFactor = true;
        }
        else
        {
            m_walkFactor = false;
        }
		
        if(@event.IsActionPressed("LockTarget"))
        {
            LockOnTarget();
        }

        Velocity = velocity;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= m_gravity * (float)delta;

        CalculateMovement((float)delta);

        // Get position change based an animation (root motion)
        Quaternion currentRotation = Transform.Basis.GetRotationQuaternion();
        velocity = DivideByFloat(currentRotation.Normalized() * m_animationTree.GetRootMotionPosition(), (float)delta);

        Velocity = velocity;
		MoveAndSlide();
		mainCamera.GlobalPosition = this.GlobalPosition;
	}

	private void CalculateMovement(float delta)
	{
        // Get the input direction and handle the movement/deceleration.
        Vector2 inputDir = Input.GetVector("Left", "Right", "Forward", "Backward");
        m_movementDirection = (mainCamera.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        //Rotate the Player according to camera rotation and movement direction (input)
        if (m_movementDirection != Vector3.Zero)
        {
            m_targetLookDirection = m_movementDirection;
            float rotationAngle = this.Transform.Basis.Z.SignedAngleTo(m_targetLookDirection, Vector3.Up);
            this.RotateY(Mathf.Sign(rotationAngle) * Mathf.Min(m_characterRotationRate * (float)delta, Mathf.Abs(rotationAngle)));
            m_completeTurn = true;
        }
        else
        {
            if (m_completeTurn) 
            {
                float rotationAngle = this.Transform.Basis.Z.SignedAngleTo(m_targetLookDirection, Vector3.Up);
                this.RotateY(Mathf.Sign(rotationAngle) * Mathf.Min(m_characterRotationRate * 2 * (float)delta, Mathf.Abs(rotationAngle)));
                m_animationTree.Set("parameters/TurnOnSpot/blend_amount", 1);
                if (Mathf.Abs(rotationAngle) < 0.01f)
                { 
                    m_completeTurn = false;
                    m_animationTree.Set("parameters/TurnOnSpot/blend_amount", 0);
                }
            }
        }

        if (m_movementDirection != Vector3.Zero && m_walkFactor && IsOnFloor())
        {
            m_animationTree.Set("parameters/Movement/blend_amount", 1); //Blend Value of 1 equals walking
        }
        else
        {
            m_animationTree.Set("parameters/Movement/blend_amount", -1 + inputDir.Length()); //Input.GetVector is by default limited to the length of 1
        }
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

    private void LockOnTarget()
    {
        float closestToHorizontalCenter = float.MaxValue;
        m_lockedTarget = null;
        for (int i = 0; i < m_targetList.Count; i++)
        {
            if (mainCamera.Camera.IsPositionInFrustum(m_targetList[i].GlobalPosition))
            {
                Vector2 screenCoords = mainCamera.Camera.UnprojectPosition(m_targetList[i].GlobalPosition);
                if (Mathf.Abs((GetViewport().GetVisibleRect().Size.X / 2.0f) - screenCoords.X) < closestToHorizontalCenter)
                {
                    closestToHorizontalCenter = Mathf.Abs((GetViewport().GetVisibleRect().Size.X / 2.0f) - screenCoords.X);
                    m_lockedTarget = m_targetList[i];
                }
                Debug.Print((GetViewport().GetVisibleRect().Size.X / 2.0f).ToString());
            }
        }
        
        GUI.LockOnTarget(m_lockedTarget);
    }

    public void AddBanditToList(Area3D enemy)
    {
        m_targetList.Add(enemy);
    }

    public void RemoveBanditFromList(Area3D enemy)
    {
        m_targetList.Remove(enemy);
    }
}
