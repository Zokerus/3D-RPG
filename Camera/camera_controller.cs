using Godot;
using System.Diagnostics;

public partial class camera_controller : Node3D
{
    [Export]
    public float sensitity = 0.005f;

    private SpringArm3D m_springArm3D;
    private Camera3D m_camera3D;
    private Timer m_delayTimer;
    private LockOnComponent m_target = null;
    private bool m_locked = false;

    public Camera3D Camera 
    {
        get
        {
            return m_camera3D;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        m_springArm3D = GetNode<SpringArm3D>("SpringArm3D");
        m_camera3D = GetNode<Camera3D>("SpringArm3D/Camera3D");
        m_delayTimer = GetNode<Timer>("DelayTimer");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            this.RotateY(-mouseMotion.Relative.X * sensitity);
            m_springArm3D.RotateX(-mouseMotion.Relative.Y * sensitity);
            m_springArm3D.Rotation = new Vector3(Mathf.Clamp(m_springArm3D.Rotation.X, -Mathf.Pi * 0.25f, Mathf.Pi * 0.25f), m_springArm3D.Rotation.Y, m_springArm3D.Rotation.Z);
        }

    }

    public void LockTarget(LockOnComponent target) 
    {
        m_target = target;
        m_locked = true;
    }

    public void UnlockTarget() 
    {
        m_target = null;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._Process(delta);
        if(m_target != null && m_locked)
        {
            Vector3 directionToTarget = this.GlobalPosition.DirectionTo(m_target.GlobalPosition).Normalized();
            float rotationAngle = Mathf.Atan2(-directionToTarget.X, -directionToTarget.Z);
            this.Rotation = new Vector3(this.Rotation.X, Mathf.LerpAngle(this.Rotation.Y, rotationAngle, 0.1f), this.Rotation.Z);
        }
    }

    public void OnTimerTimeout()
    {
        m_locked = true;
    }
}
