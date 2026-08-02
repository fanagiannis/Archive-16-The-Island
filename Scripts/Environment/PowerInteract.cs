using Godot;
using System;

public partial class PowerInteract : Interactable
{
	
	bool _activated=false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void Interact()
    {
        if(Interacted==false)
		{
			//TurnOnLights();
			Interacted=true;
			_activated=true;
			EmitSignal(SignalName.HasInteracted);
		}
	}

	public bool CheckActivated()
	{
		return _activated;
	}
	
}
