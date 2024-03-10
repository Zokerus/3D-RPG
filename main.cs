using Godot;
using System;

public partial class main : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Input.MouseMode = Input.MouseModeEnum.Captured;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double _delta)
    {
        if (Input.IsPhysicalKeyPressed(Key.Escape)) 
        {
            GetTree().Quit();
        }
        base._PhysicsProcess(_delta);
    }
}
