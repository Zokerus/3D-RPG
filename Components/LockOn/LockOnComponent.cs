using Godot;
using System;

public partial class LockOnComponent : Area3D
{
	[Export]
	public HighlightComponent highlightComponent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void LockTarget(bool state)
	{
		if (highlightComponent != null) 
		{ 
			highlightComponent.Enable(state);
		}
	}
}
