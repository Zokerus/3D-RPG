using Godot;

public partial class camera_controller : Node3D
{
    [Export]
    public float sensitity = 0.005f;

    private SpringArm3D m_springArm3D;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        m_springArm3D = GetNode<SpringArm3D>("SpringArm3D");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (@event is InputEventMouseMotion mouseMotion)
        {
            this.RotateY(-mouseMotion.Relative.X * sensitity);
            m_springArm3D.RotateX(-mouseMotion.Relative.Y * sensitity);
            m_springArm3D.Rotation = new Vector3(Mathf.Clamp(m_springArm3D.Rotation.X, -Mathf.Pi * 0.25f, Mathf.Pi * 0.25f), m_springArm3D.Rotation.Y, m_springArm3D.Rotation.Z);
        }
    }
}
