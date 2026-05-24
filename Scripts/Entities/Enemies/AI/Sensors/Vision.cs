using Godot;
using System;

public partial class Vision : Area3D
{
	[Export]CollisionShape3D VisionSphere;
	bool PlayerSpotted = false;
	public override void _Ready()
	{
		//VisionSphere = GetNode<CollisionShape3D>("VisionCone");
		//if(VisionSphere!=null)
		//	GD.Print("VisionSphere OK");
		BodyEntered+=OnBodyEntered;
		BodyExited+=OnBodyExited;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print(PlayerSpotted);
	}

	public void OnBodyEntered(Node3D body)
    {
		//GD.Print("VisionSphere");
        if (body.IsInGroup("Player"))
        {
		   PlayerSpotted = true;
        }
    }

    public void OnBodyExited(Node3D body)
    {
    
    }
	public void ResetPlayerSpotted()
	{
		PlayerSpotted=false;
	}

	public bool GetPlayerSpotted()
	{
		return PlayerSpotted;
	}
}
