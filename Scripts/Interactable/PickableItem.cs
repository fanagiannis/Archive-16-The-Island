using Godot;
using System;

public partial class PickableItem : Interactable
{
	[Export] PackedScene itemScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	public override void Interact()
    {
		base.Interact();
		Log.Instance.SetLog("Picked Up "+name,1);
		SetInteracted(true);
		//this.ExitInteraction();
        QueueFree();
    }

	public PackedScene GetItemInstance()
	{
		return itemScene;
	}
}
