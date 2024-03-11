using Godot;
using System;
using System.Collections.Generic;

public partial class HighlightComponent : Node3D
{
	[Export]
	public MeshInstance3D[] meshes;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Enable(bool state) 
	{
		for(int i = 0; i < meshes.Length; i++)
		{
			if (meshes[i].MaterialOverlay != null) 
			{
				meshes[i].MaterialOverlay.Set("shader_parameter/enable", state);
			}
		}
	}
}
