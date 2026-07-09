using Godot;
using System;

public partial class PuzzleInteractable : Interactable
{
	
	private bool _canReact = false;
	public override void _Ready()
	{
		_canReact = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void Interact()
    {
        if(_canReact)
			GD.Print("COMPLETED");
		else
			GD.Print("NOT COMPLETE");
    }

	public void SetReact()
	{
		_canReact=true;
	}
}
