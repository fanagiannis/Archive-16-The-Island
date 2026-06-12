using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class Consumable : PickableItem
{
	[Export] protected int ConsumeAmmount=0;
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void Interact()
    {
		Log.Instance.SetLog("Picked Up "+name,1);
		SetInteracted(true);
		AudioStreamPlayer3D Audio = new AudioStreamPlayer3D();
		Audio.Stream = itemAudio;
		AddChild(Audio);
		Audio.Play();
		Hide();
		Audio.Finished += () =>
		{
			Audio.QueueFree(); // Crucial! This deletes the node once the sound is done.
			QueueFree();
		};
		

	}

	public virtual void Consume()
	{
		GD.Print("Consumed");
		
	}
}
