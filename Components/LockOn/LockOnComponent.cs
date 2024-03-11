using Godot;
using System;
using static Godot.WebSocketPeer;

public partial class LockOnComponent : Area3D
{
	[Export]
	public HighlightComponent highlightComponent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void LockTarget()
	{
		if (highlightComponent != null) 
		{ 
			highlightComponent.Enable(true);
		}
	}

	public void UnlockTarget()
	{
        if (highlightComponent != null)
        {
            highlightComponent.Enable(false);
        }
    }
}
