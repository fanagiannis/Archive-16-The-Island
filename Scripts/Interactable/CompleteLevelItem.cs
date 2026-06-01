using Godot;
using System;
using System.Dynamic;

public partial class CompleteLevelItem : Interactable
{
	bool canInteract = false;
	public override void _Ready()
	{
		base._Ready();
		
		SetOutline(false);
	}

	public override void _PhysicsProcess(double delta)
    {
        
    }

	public void SetCanInteract(bool set)
	{
		canInteract = set;
	}

	public override void Interact()
	{
		base.Interact();
		
		if(canInteract)
		{
			Interacted = true;
			SceneManager.Instance.victoryScreen.Visible=true;
			SceneManager.Instance.GetPlayer().EnableController(false);
			SceneManager.Instance.GetPlayer().EnableUI(true);
		}
			
		//GD.Print("Interact");
	}

	public override void EnterInteraction()
	{
		if(itemLabel==null) return;
		else
			//itemLabel.Visible = true;
			SetOutline(true);
	}
	public override void ExitInteraction()
	{	
		if(itemLabel==null) return;
		else
			//itemLabel.Visible = false;
			SetOutline(false);
	}
	

	
}


