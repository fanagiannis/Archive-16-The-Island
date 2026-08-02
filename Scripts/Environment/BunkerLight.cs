using Godot;
using System;

public partial class BunkerLight : Node3D
{
	[Export]OmniLight3D omnilight;
	[Export] MeshInstance3D lightMesh;
	[Export]Material lightOnMaterial;
	[Export]Material lightOffMaterial;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TurnOffLight();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void TurnOnLight()
	{
		lightMesh.SetSurfaceOverrideMaterial(1, lightOnMaterial);
		omnilight.Visible=true;
	}

	public void TurnOffLight()
	{
		lightMesh.SetSurfaceOverrideMaterial(1, lightOffMaterial);
		omnilight.Visible=false;
	}
}
