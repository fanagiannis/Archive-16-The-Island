using Godot;
using System;

public partial class Door : Interactable
{

	[Export]bool Locked = false;
	bool isOpen=false;
	Godot.AnimationPlayer animation;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		animation = GetNode<Godot.AnimationPlayer>("Animation");
	}
    public override void Interact()
    {
        base.Interact();
		if(animation!=null)
		{
			Open();
		}
    }

	void Open()
	{
		if(Locked)
		{
			GD.Print("Locked");
		}
		else
		{
			if(isOpen)
			{
				animation.PlayBackwards("Door");
				isOpen=false;
			}
			else
			{
				animation.Play("Door");
				isOpen=true;
			}

		}
	}
}
