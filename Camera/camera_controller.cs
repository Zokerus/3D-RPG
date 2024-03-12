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
        this.LookAt(m_target.GlobalPosition, Vector3.Up, false);
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
            Vector3 directionToTarget = this.GlobalPosition.DirectionTo(m_target.GlobalPosition);
            
            float rotationAngle = this.Transform.Basis.Z.SignedAngleTo(directionToTarget, Vector3.Up) + Mathf.Pi;
            this.RotateY(Mathf.Sign(rotationAngle) * Mathf.Min(4.0f * (float)delta, Mathf.Abs(rotationAngle)));
            //this.LookAt(m_target.GlobalPosition, Vector3.Up, false);
            Debug.Print(this.GlobalPosition.ToString() + " : " + m_target.GlobalPosition.ToString());
            //Debug.Print(directionToTarget.ToString() + " : " + rotationAngle.ToString());
        }
    }

    public void OnTimerTimeout()
    {
        m_locked = true;
    }
}
