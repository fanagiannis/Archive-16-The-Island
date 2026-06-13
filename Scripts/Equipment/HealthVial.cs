using Godot;
using PolarBears.PlayerControllerAddon;
using System;

public partial class HealthVial : Consumable
{
	PlayerController player ;//= SceneManager.Instance.GetPlayer();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlayerController player = SceneManager.Instance.GetPlayer();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

    public override void Interact()
    {
        base.Interact();
    }


    public override void Consume()
	{
		
		PlayerController player = SceneManager.Instance.GetPlayer();
		if(player!=null)
		{
			player.HealthSystem.Heal(ConsumeAmmount);
		}
		else
			GD.Print("NULL PLAYER");
	}
}
