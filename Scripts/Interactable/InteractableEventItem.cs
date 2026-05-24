using Godot;
using System;
using System.Threading.Tasks;

public partial class InteractableEventItem : Interactable
{
	[Export] AnimatedEvent assosiatedEvent;
    public override async void Interact()
    {
        base.Interact();
        SetInteracted(true);
        SetOutline(false);
		if(assosiatedEvent!=null)
			assosiatedEvent.Execute();
        await Task.Delay(4000);
        SetInteracted(false);
        return;
    }

}
