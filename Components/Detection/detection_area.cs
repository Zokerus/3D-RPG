using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class detection_area : Area3D
{
	[Export]
	public float detectionRadius = 10;
	[Export]
	public float detectionAngle = 100;

	[Signal]
	public delegate void TargetDetectedEventHandler(Node3D taget);

	private CollisionShape3D m_collisionShape;
	private Node3D m_parentBody;
	private RayCast3D m_rayCast;

	private List<Node3D> bodyList;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		m_parentBody = GetParentNode3D();
		bodyList = new List<Node3D>();
		m_rayCast = GetNode<RayCast3D>("RayCast3D");
        Draw3D draw = new Draw3D();
        AddChild(draw);
        draw.sector(Vector3.Zero, -Vector3.Forward, detectionRadius, detectionAngle, Colors.Red);
		m_rayCast.TargetPosition = -Vector3.Forward * detectionRadius;
		m_collisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
        SphereShape3D sphere = m_collisionShape.Shape as SphereShape3D;
		sphere.Radius = detectionRadius;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		for(int i = 0;  i < bodyList.Count; i++) 
		{
            Vector3 toBody = bodyList[i].GetNode<CollisionShape3D>("CollisionShape3D").GlobalPosition - this.GlobalPosition;
            if (GlobalTransform.Basis.Z.Dot(toBody.Normalized()) > Mathf.Cos(Mathf.DegToRad(detectionAngle * 0.5f)))
            {
				m_rayCast.Enabled = true;
				m_rayCast.TargetPosition = m_parentBody.Transform.Basis * toBody;
				if (m_rayCast.IsColliding()) 
				{
					Node3D body = (Node3D)m_rayCast.GetCollider();
					if (body == bodyList[i])
					{
						EmitSignal(SignalName.TargetDetected, body);
                    }
				}
				
            }
        }
	}

	/// <summary>
	/// Body entered area3D, yet not visible or detected
	/// </summary>
	/// <param name="body">Entering Body</param>
	public void _On_Body_Entered(Node3D body)
	{
		// add only enemies to the list
		if (body != null && body != m_parentBody)
			bodyList.Add(body);
	}
	/// <summary>
	/// Body left the area3D
	/// </summary>
	/// <param name="body"></param>
	public void _On_Body_Exited(Node3D body) 
	{
        bodyList.Remove(body);
    }
}
