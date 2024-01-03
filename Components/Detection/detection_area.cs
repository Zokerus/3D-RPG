using Godot;
using System;
using System.Diagnostics;

public partial class detection_area : Area3D
{
	[Export]
	public float detectionRadius = 10;
	[Export]
	public float detectionAngle = 100;

	Signal bodyEntered;
	Signal bodyExited;

	private CollisionShape3D collisionShape;
	private Node3D parentBody;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		parentBody = GetParentNode3D();
        Draw3D draw = new Draw3D();
        AddChild(draw);
        draw.sector(Vector3.Zero, -Vector3.Forward, detectionRadius, detectionAngle, Colors.Red);
		collisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
        SphereShape3D sphere = collisionShape.Shape as SphereShape3D;
		sphere.Radius = detectionRadius;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	/// <summary>
	/// Body entered area3D, yet not visible or detected
	/// </summary>
	/// <param name="body">Entering Body</param>
	public void _On_Body_Entered(Node3D body)
	{
		Vector3 toBody = body.GlobalPosition - this.GlobalPosition;
		//Debug.Print(toBody.ToString());
		//Debug.Print(new Vector3(0, 0, 1).Dot(toBody.Normalized()).ToString());
		//Debug.Print(Mathf.Cos(Mathf.DegToRad(detectionAngle * 0.5f)).ToString());
		if (new Vector3(0,0,1).Dot(toBody.Normalized()) > Mathf.Cos(Mathf.DegToRad(detectionAngle * 0.5f)))
		{
			Debug.Print("Body entered");
		}
	}
	/// <summary>
	/// Body left the area3D
	/// </summary>
	/// <param name="body"></param>
	public void _On_Body_Exited(Node3D body) 
	{
        Debug.Print("Body exited");
    }
}
