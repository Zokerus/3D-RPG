using Godot;
using System.Diagnostics;

public partial class camera_controller : Node3D
{
    [Export]
    public float sensitity = 0.005f;

    private SpringArm3D m_springArm3D;
    private Camera3D m_camera3D;
    private LockOnComponent m_target = null;
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
    }

    public void UnlockTarget() 
    {
        m_target = null;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(m_target != null)
        {
            this.LookAt(m_target.GlobalPosition, Vector3.Up, false);
            //float rotationAngle = this.Transform.Basis.Z.SignedAngleTo(m_target.GlobalPosition - this.GlobalPosition, Vector3.Up);
            //Debug.Print(rotationAngle.ToString());
            //this.RotateY(rotationAngle+Mathf.Pi);
            //this.RotateY(Mathf.Sign(rotationAngle) * Mathf.Min(m_characterRotationRate * (float)delta, Mathf.Abs(rotationAngle)));
        }
    }
}
