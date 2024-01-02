using Godot;
using System;
using static Godot.CameraFeed;

public partial class Draw3D : Node3D
{
    private Node3D parentNode;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void point(Vector3 position, Color color, float radius = 0.05f)
    {
        MeshInstance3D mesh = new MeshInstance3D();
        SphereMesh sphere = new SphereMesh();
        OrmMaterial3D material = new OrmMaterial3D();

        mesh.Mesh = sphere;
        mesh.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
        mesh.Position = position;

        sphere.Radius = radius;
        sphere.Height = radius * 2;
        sphere.Material = material;

        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        material.AlbedoColor = color;

        AddChild(mesh);
    }

    public void line(Vector3 startPosition, Vector3 endPosition, Color color)
    {
        MeshInstance3D mesh = new MeshInstance3D();
        ImmediateMesh immediateMesh = new ImmediateMesh();
        OrmMaterial3D material = new OrmMaterial3D();

        mesh.Mesh = immediateMesh;
        mesh.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;

        immediateMesh.SurfaceBegin(Mesh.PrimitiveType.Lines, material);
        immediateMesh.SurfaceAddVertex(startPosition);
        immediateMesh.SurfaceAddVertex(endPosition);
        immediateMesh.SurfaceEnd();

        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        material.AlbedoColor = color;

        AddChild(mesh);
    }

    public void sector(Vector3 startPosition, Vector3 direction, float distance, float angle, Color color)
    {
        MeshInstance3D mesh = new MeshInstance3D();
        ImmediateMesh immediateMesh = new ImmediateMesh();
        OrmMaterial3D material = new OrmMaterial3D();

        int i = 0;
        int numberOfPoints = Mathf.RoundToInt(angle / 10.0f) + 1;

        mesh.Mesh = immediateMesh;
        mesh.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;

        immediateMesh.SurfaceBegin(Mesh.PrimitiveType.LineStrip, material);
        immediateMesh.SurfaceAddVertex(startPosition);
        for (i = 0; i < numberOfPoints; i++)
        {
            immediateMesh.SurfaceAddVertex(startPosition + direction.Normalized().Rotated(Vector3.Up, Mathf.DegToRad(-angle * 0.5f + 10.0f*i)) * distance);
        }
        immediateMesh.SurfaceAddVertex(startPosition);


        immediateMesh.SurfaceEnd();

        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        material.AlbedoColor = color;

        AddChild(mesh);
    }
}
