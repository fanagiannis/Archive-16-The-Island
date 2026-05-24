using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class Equipment : Node3D
{
	[Export]public PackedScene equipmentscene;
	[Export]protected AudioEffect equipmentsound;
	[Export]protected string name;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public virtual void Use()
	{
		GD.Print("Equipment Used");
	}

	public string GetName()
	{
		return name;
	}
}
