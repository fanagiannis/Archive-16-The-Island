using Godot;
using System;

public partial class PowerManagement : Node
{
	[Export]Node3D Lights;
	[Export] PowerInteract powerTerminalA;
	[Export] PowerInteract powerTerminalB;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		powerTerminalA.HasInteracted+=CheckLights;
		powerTerminalB.HasInteracted+=CheckLights;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void TurnOnLights()
	{
		foreach(BunkerLight light in Lights.GetChildren())
		{
			light.TurnOnLight();
		}
	}

	public void CheckLights()
	{
		if(powerTerminalA.CheckActivated()&&powerTerminalB.CheckActivated())
		{
			TurnOnLights();
		}
	}
}
