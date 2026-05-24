using Godot;
using System;

public partial class PickUp : Interactable
{
	[Export] PickableItem Item;

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

	public PickableItem GetPickableItem()
	{
		return Item;
	}
}
