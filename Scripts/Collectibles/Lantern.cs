using Godot;
using System;

public partial class Lantern : Collectible
{
    [Export]private Light3D light;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        light.Visible=false;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void Interact()
    {
        Log.Instance.SetLog("Lantern Lit",5);
        if(Interacted==false)
        {
            light.Visible=true;
            AudioStreamPlayer3D audio = new AudioStreamPlayer3D();
            audio.Stream = itemAudio;
            AddChild(audio);
            audio.Play();
            Interacted=true;
            LanternCollectionManager.Instance.CheckLanterns();
        }
    }

    public void EnableLantern()
    {
        if(Interacted==false)
        {
            light.Visible=true;
            Interacted=true;
        }
    }

	public void CollectLantern() 
    {
        // 1. Put your gameplay mechanics here (e.g., adding fuel, incrementing total lanterns)
        GD.Print($"Picked up lantern: {Name}");

        // 2. Record to the lantern list & save JSON file
        if (!string.IsNullOrEmpty(UniqueId))
        {
            CollectibleManager.Instance.RecordLantern(UniqueId);
        }

        // 3. Remove the physical item from the map
        //QueueFree();
    }
}
