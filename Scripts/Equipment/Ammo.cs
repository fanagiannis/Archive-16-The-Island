using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class Ammo : Consumable
{
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
        base.Interact();
    }

	public override void Consume()
	{
		Equipment equipment = EquipmentManager.Instance.GetEquippedWeapon();
		if(equipment==null)
		{
			Log.Instance.SetLog("No Equipped Item",5);
			return;
		}
		else  
		{
			if(equipment is Weapon weapon)
				weapon.AddMaxAmmo(ConsumeAmmount);
			else
			{
				Log.Instance.SetLog("No Weapon Equipped",5);
				return;
			}
		}
	}
}
