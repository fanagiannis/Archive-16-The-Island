using Godot;
using System;
using System.Collections.Generic;

public partial class ChunkLoader : Node3D
{
	[Export] public Node3D Player;
    [Export] public float ActivationDistance = 30f;
	int count=0;

    public override void _Process(double delta)
    {
        foreach (var body in GetTree().GetNodesInGroup("Tree"))
        {
			
            if (body is CollisionObject3D obj)
            {
				count++;
				GD.Print(count);
                float dist = obj.GlobalPosition.DistanceTo(Player.GlobalPosition);
                obj.SetDeferred("disabled", dist > ActivationDistance);
            }
        }
    }
}
