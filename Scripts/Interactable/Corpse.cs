using Godot;
using System;
using System.Dynamic;

public partial class Corpse : Interactable
{

	public override void _Ready()
	{
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
    {
        
    }

	public override void Interact()
	{
		if(!Interacted)
		{
			base.Interact();
			Interacted=true;
			SceneManager.Instance.GetQuestManager().TrackProgress("The Stage",1);
			
		}
	}
	
}
