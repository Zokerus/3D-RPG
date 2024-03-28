using Godot;
using System.Diagnostics;

public partial class camera_controller : Node3D
{
    [Export]
    public float sensitity = 0.005f;
    [Export]
    public float lockOnMinDistance = 2.0f;
    [Export]
    public float lockOnMaxDistance = 20.0f;
    [Export]
    public float lockOnMinAngle = 8.0f;
    [Export]
    public float lockOnMaxAngle = 10.0f;

    private SpringArm3D m_springArm3D;
    private Camera3D m_camera3D;
    private Timer m_recenterTimer;
    private LockOnComponent m_target = null;
    private bool m_locked = false;
    private RayCast3D m_raycast = null;

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
        m_recenterTimer = GetNode<Timer>("RecenterTimer");
        m_raycast = GetNode<RayCast3D>("SpringArm3D/Camera3D/RayCast3D");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            this.RotateY(-mouseMotion.Relative.X * sensitity);
            m_springArm3D.RotateX(-mouseMotion.Relative.Y * sensitity);
            m_springArm3D.Rotation = new Vector3(Mathf.Clamp(m_springArm3D.Rotation.X, -Mathf.Pi * 0.25f, Mathf.Pi * 0.25f), m_springArm3D.Rotation.Y, m_springArm3D.Rotation.Z);
            m_locked = false;
            m_recenterTimer.Stop();
            m_recenterTimer.Start();
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
            float rotationAngleY = Mathf.Atan2(-directionToTarget.X, -directionToTarget.Z);
            
            float clampedDistance = Mathf.Clamp(this.GlobalPosition.DistanceTo(m_target.GlobalPosition), lockOnMinDistance, lockOnMaxDistance);
            float normalizedDistance = (clampedDistance - lockOnMinDistance) / (lockOnMaxDistance - lockOnMinDistance);
            normalizedDistance = Mathf.SmoothStep(0.0f, 1.0f, normalizedDistance);
            float rotationAngleX = Mathf.DegToRad(-Mathf.Lerp(lockOnMaxAngle, lockOnMinAngle, normalizedDistance));

            if (Mathf.Abs((this.Rotation.Y - rotationAngleY) % Mathf.Pi) > 0.087f)
            {
                this.Rotation = new Vector3(this.Rotation.X, Mathf.LerpAngle(this.Rotation.Y, rotationAngleY, 0.2f), this.Rotation.Z);
            }
            else
            {
                this.Rotation = new Vector3(this.Rotation.X, rotationAngleY, this.Rotation.Z);
            }
                m_springArm3D.Rotation = new Vector3(Mathf.Clamp(Mathf.LerpAngle(m_springArm3D.Rotation.X, rotationAngleX, 0.1f), -Mathf.Pi * 0.25f, Mathf.Pi * 0.25f), m_springArm3D.Rotation.Y, m_springArm3D.Rotation.Z);
        }
    }

    public void OnTimerTimeout()
    {
        if (m_target != null)
        {
            m_locked = true;
        }
    }

    public void LockCamera()
    {
        if (m_target != null)
        {
            m_recenterTimer.Stop();
            m_recenterTimer.Start();
        }
        else
        {
            m_locked = false;
            m_recenterTimer.Stop();
        }
    }

    public void UnlockCamera()
    {
        m_locked = false;
        m_recenterTimer.Stop();
    }

    public bool IsTargetVisible(LockOnComponent target)
    {
        Vector2 screenPosition = this.m_camera3D.UnprojectPosition(target.GlobalPosition);
        this.m_raycast.TargetPosition = this.m_camera3D.ProjectLocalRayNormal(screenPosition) * this.m_camera3D.GlobalPosition.DistanceTo(target.GlobalPosition);
        this.m_raycast.Enabled = true;
        this.m_raycast.ForceRaycastUpdate();

        if(this.m_raycast.IsColliding())
        {
            if(this.m_raycast.GetCollider() == target || this.m_raycast.GetCollider() == target.GetParent())
            {
                this.m_raycast.Enabled=false;
                return true;
            }
        }
        this.m_raycast.Enabled = false;
        return false;
    }
}
