using Godot;
using PolarBears.PlayerControllerAddon;
using System;

public partial class DamagableArea : Area3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnEnter(Node3D Other)
	{
		
		if(Other is CharacterBody3D)
		{
			GD.Print("aaaa");
		}
	}
}
