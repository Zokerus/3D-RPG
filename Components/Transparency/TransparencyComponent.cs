using Godot;
using System;

public partial class TransparencyComponent : Node
{
    [Export]
    public float UpperDistance = 3.0f;
    [Export]
    public float LowerDistance = 1.5f;
    [Export]
    public MeshInstance3D[] meshes;

    /// <summary>
    /// Calculate the transparency value of the meshes in relation to the given distance
    /// </summary>
    /// <param name="distance">Distance between character und camera</param>
    public void CalculateTransparency(float distance)
    {
        float clampedDistance = Mathf.Clamp(distance, LowerDistance, UpperDistance);
        float normalizedDistance = (clampedDistance - LowerDistance) / (UpperDistance - LowerDistance);
        normalizedDistance = Mathf.SmoothStep(0.0f, 1.0f, normalizedDistance);
        this.Set(1.0f - normalizedDistance);
    }
    public void Set(float value)
    {
        foreach (MeshInstance3D mesh in meshes) 
        {
            mesh.Transparency = value;
        }
    }
}
